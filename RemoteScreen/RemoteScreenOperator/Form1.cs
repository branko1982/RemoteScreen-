using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using TinyJSON;

namespace RemoteScreenOperator
{

    public partial class Form1 : Form
    {

        ClientInfo selectedClient;

        /*this method is extension of DataReader*/
        public void FurtherProcessTcpData(object data)
        {

            try
            {
                byte[] response = (byte[])data;
                //53 - textový formát dát
                //81 - snímok obrazovky
                if (response[0] == 53)
                {
                    string jsonString = Encoding.UTF8.GetString(response, 1, response.Length - 1);
                    logger.Log(jsonString);


                    DataJsonObjectTemplate receivedDataObject = JSON.Load(jsonString).Make<DataJsonObjectTemplate>();


                    if (receivedDataObject == null)
                    {
                        logger.Log("receivedDataObject == null.");
                        return;
                    }

                    switch (receivedDataObject.dataType)
                    {
                        case "auth_status_ok":
                                authentificated = true;
                                logger.Log("operator logged in");
                            break;
                        case "client_list":


                            if (receivedDataObject.clients == null)
                            {
                                logger.Log("client list returned empty");
                                listViewClients.Invoke(new MethodInvoker(delegate { listViewClients.Items.Clear(); }));
                                break;
                            }

                            listViewClients.Invoke(new MethodInvoker(delegate { listViewClients.Items.Clear(); }));


                            foreach (ClientInfo client in receivedDataObject.clients)
                            {
                                string[] row = { client.deviceName, client.ipAddress, Convert.ToString(client.portNumber) };
                                var listViewItem = new ListViewItem(row);
                                listViewClients.Invoke(new MethodInvoker(delegate { listViewClients.Items.Add(listViewItem); }));
                            }
                            break;

                        case "video_output_active":

                            logger.Log("video_output_active received!");
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                //možno trošku divný spôsob ako to vyriešiť
                                var formVideo = new FormVideo(selectedClient);

                                formVideo.logger = logger;
                                formVideos.Add(formVideo);
                            }));


                            break;
                        default:
                            logger.Log("received unknown text response : " + receivedDataObject.dataType);
                            break;
                    }
                }
                if (formVideos.Count > 0)
                {
                    if (response[0] == 81)
                    {
                        /*This way of reading images from server is very simillar to one used while reading images from client.
                         
                         Except here, the the deviceName is also read from received data
                         */
                        if (formVideos != null)
                        {
                            /*ku veľkosti obrázka ešte pridám bajty ktoré budú predstavovať dĺžku mena, a potom samotné meno klienta*/

                            /*potom sa urobí foreach loop ktorý prejde cez jednotlivé formy vo form video list, a updatuje ten, pre ktorý tá informácia došla*/

                            /*
                             * bajty v odpovedi:
                             * 0 - bajt ktorý informuje či to je obrázok alebo string
                               1,2,3,4 - veľkosť obrázku v bajtoch, 32 bitová hodnota
                               5,6,7,8 - dĺžka názvu zariadenia od ktorého prichádza táto odpoveď
                               XXXXX bajtov názov zariadenia. ˇKoľko je bajtov tu, už záleží od samotného názvu aké zariadenie má.

                                Dúfam že TCP/IP protokol pošle všetky tieto informácie, celkovo len pár bajtov naraz, a nie po častiach ako to robí s vačšími množšstvami dát.
                                To by znamenalo že sa obrázok nezobrazí správne


                             */


                            byte[] deviceNameSizeInbytes_b = new byte[4];
                            Array.Copy(response,5,deviceNameSizeInbytes_b, 0, 4);


                            if (!BitConverter.IsLittleEndian)
                            {
                                Console.WriteLine("Is not little endian");
                                Array.Reverse(deviceNameSizeInbytes_b);
                            }


                            int deviceNameSizeInbytes_i = BitConverter.ToInt32(deviceNameSizeInbytes_b, 0);

                            string deviceName = Encoding.UTF8.GetString(response, 9, deviceNameSizeInbytes_i);

                            /*check if performed whether such form exists.If it exists, it is saved into variable*/

                            FormVideo foundFormVideo = null;

                            foreach (FormVideo formVideo in formVideos)
                            {
                                if (formVideo.clientInfo.deviceName == deviceName)
                                {
                                    foundFormVideo = formVideo;
                                }
                            }

                            /*if actualVideoForm is still null, it means form was not found. At this point the function will stop. */
                            if(foundFormVideo == null)
                            {

                                /*function stops*/
                                return;
                            }

                            //logger.Log("image was meant for form with deviceName : " + deviceName); ;

                            byte[] imageSizeB = new byte[4];
                            /*Tu pošlem operátorovy informáciu že to je obrázok, a v dalších bajtov bude informácia aký je veľký.*/
                            Array.Copy(response, 1, imageSizeB, 0, 4);

                            if (!BitConverter.IsLittleEndian)
                            {
                                //Console.WriteLine("Is not little endian");
                                Array.Reverse(imageSizeB);
                            }




                            int imageSize = BitConverter.ToInt32(imageSizeB, 0);
                            //logger.Log("image size will be -> " + imageSize);

                            bool allBytesRead = false;
                            byte[] imageBuffer = new byte[imageSize];

                            int offset = 0;

                            bool continueMessageSent = false;

                            while (!allBytesRead && foundFormVideo.IsFormOpen)
                            {
                                //logger.Log("all bytes have not yet been read");

                                if (!continueMessageSent)
                                {

                                    /*the data is sent in json. But the goal is the same as when server receives image from client*/
                                    DataJsonObjectTemplate dataJsonObjectTemplate = new DataJsonObjectTemplate();
                                    dataJsonObjectTemplate.dataType = "continue";

                                    byte[] json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));
                                    socket.Send(json);

                                    //pošle sa klientovy správa že môže poslať screen obrazovky
                                    //logger.Log("sending continuation message to client");
                                    continueMessageSent = true;
                                }

                                //získam zvyšok obrázka
                                byte[] buffer = new byte[1000000];
                                int length = socket.Receive(buffer);

                                //logger.Log("received bytes : " + length);
                                if (length == 0)
                                {
                                    logger.Log("[ClientConnectionInstance.cs] TcpDataReader() received 0 bytes, therefore remote endpoint closed the socket : ");
                                    break;
                                }

                                //do offsetu sa musí zarátať fakt že index sa vačšinou začína v 0
                                Array.Copy(buffer, 0, imageBuffer, offset, length);

                                offset += length;
                                //logger.Log("read " + offset + "/" + imageSize + " bytes");


                                if (offset == imageSize)
                                {
                                    allBytesRead = true;
                                    //logger.Log("All bytes (" + offset + ") have been read");
                                }
                            }
                            if (allBytesRead && foundFormVideo.IsFormOpen)
                            {
                                //logger.Log("trying to update picture box");

                                var ms = new MemoryStream(imageBuffer);
                                receivedImage = new Bitmap(ms);

                                foreach(FormVideo formVideo in formVideos)
                                {
                                    if(formVideo.clientInfo.deviceName == deviceName)
                                    {
                                        formVideo.UpdatePictureBox(receivedImage);

                                        DataJsonObjectTemplate dataJsonObjectTemplate = new DataJsonObjectTemplate();
                                        dataJsonObjectTemplate.dataType = "continue";

                                        //logger.Log("continuation message send");
                                        byte[] json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));
                                        socket.Send(json);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.Log("[Form1.cs]FurtherProcessTcpData exception: " + e.Message);
                logger.Log("[Form1.cs]FurtherProcessTcpData InnerException: " + e.InnerException);

            }
        }

        public void TcpSocketDataReader()
        {
            logger.Log("[Form1.cs] TcpDataReader started");

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[500000];
                    int length = this.socket.Receive(buffer);
                    byte[] receivedData = new byte[length];
                    Array.Copy(buffer, 0, receivedData, 0, length);


                    /*prvý bajt v každom.. ako sa toto volá. paket?*/
                    //ak bude prvý bajt 53, bude to znamenať že bol poslaný string. Ak bude bajt 81, bude to znamenať že bol poslaný obrázok. atď.
                    //bajt môže mať 255 rôznych čísel. Takže ďalsie sa budú dať doplniť neskôr. Toto je moje riešenie.

                    //logger.Log("received data -> " + Encoding.UTF8.GetString(receivedData));

                    FurtherProcessTcpData(receivedData);

                }
                catch (Exception e)
                {
                    logger.Log("[Form1.cs] TcpDataReader " + e.Message);
                    break;
                }
            }
        }


        private bool authentificated = false;

        Socket socket;
        private Bitmap receivedImage;

        private List<FormVideo> formVideos; //objekt v ktorom budú uložené jednotlivé formuláre, používané pre zobrazenie výstupu od klientov.

        private Logger logger;
        public Form1()
        {
            /*inicializujú sa premenné*/
            InitializeComponent();
            logger = new Logger(richTextBox1);
            formVideos = new List<FormVideo>();
            this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }


       /* private void OnVideoFormClosed(object sender, System.EventArgs e)
        {
        chcel som event pre spracovanie zatvorenia Formu umiestniť sem odtiaľto získať názov klienta a to poslať na server. Ale neviem ako.
        Bol by som schopný získať referenciu na objekt ktorý bol rodičom controlky ktorá event spustila, čiže samotného formu, ale prístup ku KlientInfo objektu nie, nakoľko samotný Form je len členom objektu VideoForm ktorý potrebnú informáciu obsahuje.
        //možno by v C# bol spôsob ako to urobiť, ale na teraz budem zatvorenie formov spracovávať úplne iným neustále bežiacim vláknom, ktorého účelom bude sledovať formy, či sú zatvorené, ak hej, tak pošle informáciu o tom že sa form zatvoril na server, a ten to pošle klientovy.
            
            logger.Log("OnFormCLosed from Main form");
        } */


        private void ButtonConnect_Click(object sender, EventArgs e)
        { 
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(textBox_ip_address.Text), Int32.Parse(textBox_port.Text));
                socket.Connect(endPoint);
                logger.Log("tryint to connect to socket : " + endPoint.Address + ":" + endPoint.Port);
                if (socket.Connected)
                {

                    /*zapne sa vlákno ktoré bude načúvať pruch´ádzajúcim dátam zo servera */
                    Thread t1 = new Thread(new ThreadStart(TcpSocketDataReader));
                    t1.Start();

                    FormVideoChecker formVideoChecker = new FormVideoChecker(formVideos, logger, socket);
                    Thread t2 = new Thread(new ThreadStart(formVideoChecker.Main));
                    t2.Start();

                    string dataToBeSent = "operator";
                    dataToBeSent += "|";
                    dataToBeSent += "dev";

                    socket.Send(Encoding.UTF8.GetBytes(dataToBeSent));

                }
            }
            catch (Exception ex)
            {
                logger.Log("Connection to server failed " + ex.Message);
            }
        }

        private void ButtonGetClients_Click(object sender, EventArgs e)
        {
            try
            {
                if (!socket.Connected)
                {
                    logger.Log("Socket is not connected");
                    return;
                }
                if (!authentificated)
                {
                    logger.Log("You are not authenticated");
                    return;
                }
                if (socket.Connected)
                {
                    //mohol som použiť aj object alebo var, ale použil som dynamic :D chcel som použiť ten keyword
                    if (listViewClients.SelectedItems.Count == 0)
                    {
                        logger.Log("No client selected form ListView");
                        return;
                    }
                    dynamic selectedItem = listViewClients.SelectedItems[0];


                    selectedClient = new ClientInfo();
                    selectedClient.deviceName = selectedItem.SubItems[0].Text;
                    selectedClient.ipAddress = selectedItem.SubItems[1].Text;
                    selectedClient.portNumber = Int32.Parse(selectedItem.SubItems[2].Text);


                    var clients = new List<ClientInfo>();
                    clients.Add(selectedClient);
                    DataJsonObjectTemplate dataJsonObjectTemplate = new DataJsonObjectTemplate();
                    dataJsonObjectTemplate.dataType = "get_screen_output_from_client";
                    dataJsonObjectTemplate.clients = clients;

                    byte[] json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));


                    socket.Send(json);


                    /*continuation message must be also sent.*/
                    dataJsonObjectTemplate.dataType = "continue";
                    dataJsonObjectTemplate.clients = null;
                    json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));
                    socket.Send(json);

                }
            }
            catch (Exception ex)
            {
                logger.Log("[Form1.cs] ButtonGetClients_Click() Exception: " + ex.Message);
                logger.Log("[Form1.cs] ButtonGetClients_Click() InnerException: " + ex.InnerException);

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Config.UseLog = true;
            }
            if (!checkBox1.Checked)
            {
                Config.UseLog = false;
            }
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.ScrollToCaret();
        }

        private void ButtonGetClientList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!socket.Connected)
                {
                    logger.Log("Socket is not connected");
                    return;
                }
                if (!authentificated)
                {
                    logger.Log("You are not authenticated");
                    return;
                }
                if (socket.Connected)
                {
                    DataJsonObjectTemplate dataJsonObjectTemplate = new DataJsonObjectTemplate();
                    dataJsonObjectTemplate.dataType = "get_client_list";

                    byte[] json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));

                    socket.Send(json);
                }
            }
            catch(Exception ex)
            {
                logger.Log(ex.Message);
            }
        }

        private void OnFormClosed(object sender, System.EventArgs e)
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if (p.Id == curr.Id)
                {
                    p.Kill();
                }
            }
        }


    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using TinyJSON;

namespace RemoteScreenServer
{
    class OperatorConnectionInstance
    {
        IPEndPoint endpoint;
        public SocketEntry socketEntry;

        /// <summary>
        /// rozhodol som sa použiť konštruktor pre predanie argumentov triede
        /// </summary>

        private bool continueRunning;


        private void SendVideoOutputToOperator()
        {
            Logger.Log("[OperatorConnectionInstance.cs] SendVideoOutputToOperator() thread started");

            while (true)
            {
                try
                {
                    //nepomýliť si this.socketEntry a socketEntry. Sakra rozdiel
                    foreach (SocketEntry socketEntry in SocketEntryManager.GetSocketEntries())
                    {
                        if (socketEntry.type != "operator")
                        {
                            if (socketEntry.receivedScreenCaptures.Count > 0)
                            {
                                while (!continueRunning && socketEntry.videoStreamActive)
                                {
                                    Thread.Sleep(1);
                                }
                                if (socketEntry.videoStreamActive)
                                {
                                    continueRunning = false;

                                    //Logger.Log("SendVideoOutputToOperator - continuing, sending image meta data");

                                    byte[] dataToSend = socketEntry.receivedScreenCaptures[0];

                                    socketEntry.receivedScreenCaptures.Remove(socketEntry.receivedScreenCaptures[0]);

                                    //Logger.Log("[OperatorConnectionInstance.cs] socketEntry:" + socketEntry.deviceName + " has some video data");


                                    byte[] clientDeviceName = Encoding.UTF8.GetBytes(socketEntry.deviceName);  //názov zariadenia klienta, ktorý sa vloží do socketu


                                    byte[] imageSize = BitConverter.GetBytes(dataToSend.Length);
                                    byte[] clientDeviceNameSize = BitConverter.GetBytes(clientDeviceName.Length);
                                    /*1 bajt reprezentuje typ dát, 4 bajty predstavujú dĺžku obrázka, 4 bajty veľkost názvu zariadenia, a zvyšok bajtov je 
                                    samotný názov zariadenia*/
                                    byte[] imageBasicInformationBuffer = new byte[9 + clientDeviceName.Length];

                                    imageBasicInformationBuffer[0] = 81;
                                    Array.Copy(imageSize, 0, imageBasicInformationBuffer, 1, 4);
                                    Array.Copy(clientDeviceNameSize, 0, imageBasicInformationBuffer, 5, 4);
                                    Array.Copy(clientDeviceName, 0, imageBasicInformationBuffer, 9, clientDeviceName.Length);
                                    /*získam odpoveď zo servera odhľadm toho že môžem poslať dalšie dáta*/

                                    /*int count = 0;
                                    foreach(byte x in outputBuffer_bufferSize)
                                    {
                                        Console.WriteLine(count + ":" + x);
                                        count++;
                                    }*/

                                    //continueRunning... falseé? ??

                                    if (this.socketEntry.socket != null)
                                    {
                                        this.socketEntry.socket.Send(imageBasicInformationBuffer); //posiela to naozaj štyri bajty
                                    }


                                    //Console.WriteLine("data length : " +outputBuffer_bufferSize.Length);
                                    //Console.WriteLine("data sent to server -> " + Encoding.UTF8.GetString(outputBuffer_bufferSize));
                                    //klient odošle druhej strane informáciu o veľkosti dát
                                    //Logger.Log("SendVideoOutputToOperator meta data sent, waiting for another continuation message");

                                    while (!continueRunning && socketEntry.videoStreamActive)
                                    {
                                        Thread.Sleep(1);
                                    }
                                    if (socketEntry.videoStreamActive)
                                    {
                                        //Logger.Log("SendVideoOutputToOperator - continuing, sending actual image");
                                        //Logger.Log("[OperatorConnectionInstance.cs] SendVideoOutputToOperator() continuation message received");
                                        continueRunning = false;
                                        this.socketEntry.socket.Send(dataToSend); //musí tu byť this, pretože referujem na socketEntry v oejbkete, nie v cykle. SocketEntry v cykle referuje na iné socket entry. To sa nesmie pomýliť.
                                        break;
                                    }
                                }
                            }
                            else { 

                                //Logger.Log("no images to send");

                                Thread.Sleep(1);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logger.Log("[OperatorConnectionInstance.cs] SendVideoOutputToOperator() Exception : " + ex.Message + " StackTrace:" + ex.StackTrace);
                    Logger.Log("[OperatorConnectionInstance.cs] SendVideoOutputToOperator() InnerException" + ex.InnerException);
                    break;
                }
                Thread.Sleep(1);
            }
        }

        private SocketEntry selectedSocketEntry;

        public OperatorConnectionInstance(SocketEntry socketEntry)
        {
            this.socketEntry = socketEntry;
            this.continueRunning = true; //this will be set to true, as initial value
        }

        public void Main()
        {
            try
            {

                var dataJsonObjectTemplate = new DataJsonObjectTemplate();
                dataJsonObjectTemplate.dataType = "auth_status_ok";


                string jsonString = JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints);

                byte[] json = Encoding.UTF8.GetBytes(jsonString);




                byte[] dataToBeSent = new byte[json.Length + 1];
                dataToBeSent[0] = 53; //znamená že sa jedna´o dáta ktoré obsahujú stringovú hodnotu
                Array.Copy(json, 0, dataToBeSent, 1, json.Length);
                socketEntry.socket.Send(dataToBeSent); //pošle sa správa klientovy o tom že sa autentifikoval


                endpoint = socketEntry.socket.RemoteEndPoint as IPEndPoint;

                Logger.Log("OperatorConnectionInstance created, on socket : [" + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + "]{" + socketEntry.deviceName + "}");

                Thread t1 = new Thread(new ThreadStart(TcpDataReader));
                t1.IsBackground = true;
                t1.Start();

                Thread t2 = new Thread(new ThreadStart(SendVideoOutputToOperator));
                t2.IsBackground = true;
                t2.Start();

            }
            catch (Exception e)
            {
                Logger.Log("[OperatorConnectionInstance.cs] Main() exception caught: " + e.Message);
                Logger.Log("Inner exception: {0}" + e.InnerException);
                socketEntry.socket.Close();
            }
        }

        public void CommandHandler(object data)
        {
            try
            {

                string jsonString = Encoding.UTF8.GetString((byte[])data);
                /*neviem či je dobrý nápad takýmto spôsobom pristupovať k členskym vlastnostiam triedy. Táto metóda môže pristupovať k iným vlastnostiam triedy, pretože sa v nej nachádza, ale beží v inom vlákne.
                 Nieje to trochu.. divné?
                 */

                DataJsonObjectTemplate receivedDataObject = JSON.Load(jsonString).Make<DataJsonObjectTemplate>();

                switch (receivedDataObject.dataType)
                    {
                        case "get_client_list":
                            var dataJsonObjectTemplate = new DataJsonObjectTemplate();
                            dataJsonObjectTemplate.dataType = "client_list";
                            var clientDetailsToSerializelist = new List<ClientInfo>();

                            /*dúfam že C# si nepomíli socketEntry v cykle zo socketEntry ako členom objektu*/
                            foreach (SocketEntry socketEntry in SocketEntryManager.GetSocketEntries())
                            {
                                if (socketEntry.type == "client")
                                {
                                    var endpoint = socketEntry.socket.RemoteEndPoint as IPEndPoint;
                                    var clientInfo1 = new ClientInfo();
                                    clientInfo1.deviceName = socketEntry.deviceName;
                                    clientInfo1.ipAddress = endpoint.Address.ToString();
                                    clientInfo1.portNumber = endpoint.Port;
                                    clientDetailsToSerializelist.Add(clientInfo1);
                                }
                            }

                            dataJsonObjectTemplate.clients = clientDetailsToSerializelist;

                            byte[] json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));

                            byte[] dataToBeSent = new byte[json.Length + 1];
                            dataToBeSent[0] = 53;

                            Array.Copy(json, 0, dataToBeSent, 1, json.Length);
                            socketEntry.socket.Send(dataToBeSent);

                        break;

                        case "get_screen_output_from_client":
                        /*operator sends this message, along with selected client from which it wants a output*/

                        /* a command is added to commandList of client's socketEntry, if client exists of course, and ClientCommandHandler thread sends the command to server.
                          In this case, ClientCommandHandler thread sends command "send_screen_to_server" to client. Client meaning program which captures screen on some device.

                         */
                        var client = receivedDataObject.clients[0];

                            foreach(SocketEntry socketEntry in SocketEntryManager.GetSocketEntries())
                            {
                                var endpoint = socketEntry.socket.RemoteEndPoint as IPEndPoint;

                                if (socketEntry.deviceName == client.deviceName && endpoint.Address.ToString() == client.ipAddress && endpoint.Port == client.portNumber)
                                {
                                    socketEntry.commandsList.Add("send_screen_to_server"); // do príkazov pre klienta sa pridá príkaz ktorý hovorí že má streamovať svoju obrazovku
                                    
                                    selectedSocketEntry = socketEntry;
                                    Logger.Log("Selected socket : " + client.deviceName);
                                    break;

                                }
                            }

                            //staré objekty sa zrušia;.. neviem či to je zrovna nutné, ale tak prišlo mi to ako dobrý nápad
                            dataJsonObjectTemplate = null;
                            dataToBeSent = null;

                            dataJsonObjectTemplate = new DataJsonObjectTemplate();
                            dataJsonObjectTemplate.dataType = "video_output_active";

                        /*pošle sa naspať informácia že výstup je aktívny.. v jsone*/

                            byte[] json1 = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));

                            dataToBeSent = new byte[json1.Length + 1];
                            dataToBeSent[0] = 53;

                            Array.Copy(json1, 0, dataToBeSent, 1, json1.Length);
                            int bytesSent = socketEntry.socket.Send(dataToBeSent);

                        break;

                        case "tell_client_to_stop_streaming_vid": //chcel som použiť iný string pre príkaz ktorý pošle operátor serveru, a iný pre príkaz ktorý pošle server klientovy
                        //C# nekontorlujem zakaždým či objekt existuje, program nespadne
                            ClientInfo clientInfo = receivedDataObject.clients[0];

                            foreach (SocketEntry socketEntry in SocketEntryManager.GetSocketEntries())
                            {
                                var endpoint = socketEntry.socket.RemoteEndPoint as IPEndPoint;

                                if (socketEntry.deviceName == clientInfo.deviceName && endpoint.Address.ToString() == clientInfo.ipAddress && endpoint.Port == clientInfo.portNumber)
                                {
                                    socketEntry.commandsList.Add("stop_video"); // do príkazov pre klienta sa pridá príkaz ktorý hovorí že má streamovať svoju obrazovku

                                    Logger.Log("client should stop streaming screen : " + clientInfo.deviceName);

                                    break;

                                }
                            }

                        break;

                        case "continue":

                            continueRunning = true;
                        break;

                }
            }
            catch(Exception e)
            {
                Logger.Log("[OperatorConnectionInstance.cs] CommandHandler exception: " + e.Message);
                Logger.Log("[OperatorConnectionInstance.cs] CommandHandler inner exception:" + e.InnerException);

            }
        }

        public void TcpDataReader()
        {
            Logger.Log("[OperatorConnectionInstance.cs] TcpSocketDataReader created, on socket : [" + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + "]{" + this.socketEntry.deviceName + "}");
            while (true)
            {
                try
                {
                    //socket bude naraz schopný prijať maximálne 500 tisíc bajtov
                    byte[] buffer = new byte[500000];
                    int length = this.socketEntry.socket.Receive(buffer);

                    if(length == 0)
                    {
                        throw new Exception("socket.Receive returned 0 bytes. Socket is closed");
                    }

                    byte[] data = new byte[length];
                    Array.Copy(buffer, 0, data, 0, length);
                    /*nebudem tu vytvárať dalšie vlákno*/
                    CommandHandler(data);

                }
                catch (Exception e)
                {
                    Logger.Log("[OperatorConnectionInstance.cs] TcpSocketDataReader exception: " + e.Message);
                    break;
                }
            }
        }
    }
}

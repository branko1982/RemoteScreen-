using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteScreenServer
{
    class ClientConnectionInstance
    {

        IPEndPoint endpoint;
        public SocketEntry socketEntry;

        private bool clientConnectionInstanceActive = true;


        public void ProcessReceivedData(byte[] data)
        {

            /*Možno by bolo dobré v tomto bode zaviesť nejakú komunikáciu medzi klientom a serverom. Server pošle klientovy správu že môže poslať dalšie dáta a až vtedy ich pošle*/
            try
            {
                if (socketEntry.videoStreamActive)
                {
                    if (data[0] == 81)
                    {

                        //Logger.Log("Image is being received");
                        byte[] imageSizeB = new byte[4];
                        Array.Copy(data, 1, imageSizeB, 0, 4);

                        /*I read on stackoverflow that this is nessecary in order to convert bytes properly to int.   
                         * The check of endiannes
                         */
                        if(!BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(imageSizeB);
                        }


                        int imageSize = BitConverter.ToInt32(imageSizeB, 0);
                        //Logger.Log("image size will be -> " + imageSize);

                        bool allBytesRead = false;
                        byte[] imageBuffer = new byte[imageSize];

                        //Logger.Log("created image buffer with size of : " + imageBuffer.Length);
                        int offset = 0;

                        bool continueMessageSent = false;

                        while (!allBytesRead)
                        {
                            //Logger.Log("All bytes have not been yet read");

                            if (!continueMessageSent)
                            {
                                socketEntry.socket.Send(Encoding.UTF8.GetBytes("continue"));
                                //pošle sa klientovy správa že môže poslať screen obrazovky
                                //Logger.Log("sending continuation message to client");
                                continueMessageSent = true;
                            }

                            byte[] buffer = new byte[1000000];
                            int length = socketEntry.socket.Receive(buffer);

                            //Logger.Log("received part of image: " + length + " bytes long");


                            if (length == 0)
                            {
                                //Logger.Log("[ClientConnectionInstance.cs] ProcessReceivedData() received 0 bytes, therefore remote endpoint closed the socket : ");
                                break;
                            }

                            //do offsetu sa musí zarátať fakt že index sa vačšinou začína v 0
                            //Logger.Log("part of the image (" + length + " bytes long) was copied to image buffer");
                            //Logger.Log("Trying to copy : " + length + "  byttes into imageBuffer from offset : " + offset);

                            Array.Copy(buffer, 0, imageBuffer, offset, length);
                            //Logger.Log("part of images was copied to imageBuffer");


                            offset += length;


                            if (offset == imageSize)
                            {
                                allBytesRead = true;
                                //Logger.Log("All bytes (" + offset + ") have been read");

                            }

                        }
                        if (allBytesRead)
                        {
                            //Logger.Log("image added do captured images in socket entry");


                            /*images will be added to List only if the number of images in list is less than 4.
                                The image will simply be discarded if there is no space for it in List.
                                This check exists in order to prevent received images eating all RAM
                             */
                            if (socketEntry.receivedScreenCaptures.Count < 4)
                            { 
                                socketEntry.receivedScreenCaptures.Add(imageBuffer);
                            }
                            socketEntry.socket.Send(Encoding.UTF8.GetBytes("continue"));

                        }
                    }
                    else if(data[0] == 51)
                    {
                        /*if client tell the server that he enćountered exception.. which should happen in piece of code where client is trying to capture image*/
                        socketEntry.socket.Send(Encoding.UTF8.GetBytes("continue"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("[ClientConnectionInstance.cs] ProcessReceivedData() :" + ex.Message);
            }
            
            
        }

        /// <summary>
        /// rozhodol som sa použiť konštruktor pre predanie argumentov triede
        /// </summary>


        public void Main()
        {
            /*Toto je vlákno ktoré sa používa na komunikáciu s programom ktorý beží niekde na pracovnej ploche a prenáša obraz. Dáta z neho */
            try
            {
                endpoint = socketEntry.socket.RemoteEndPoint as IPEndPoint;

                Logger.Log("ClientConnectionInstance created, on socket : [" + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + "]{" + socketEntry.deviceName + "}");

                socketEntry.socket.Send(Encoding.UTF8.GetBytes("auth_ok"));


                Thread t1 = new Thread(new ThreadStart(TcpDataReader));
                t1.IsBackground = true;
                t1.Start();


                /*Thread t2 = new Thread(new ThreadStart(CommandHandler));
                t2.IsBackground = true;
                t2.Start();*/
                //na serveri bude bežať len jedno vlákno ktoré bude všetkým klientom posielať príkazy. je to lepšie ako by mal každý klient mať pre tento účel vytvorené vlastné vlákno


            }
            catch (Exception e)
            {
                Logger.Log("[ClientConnectionInstance.cs] Main() exception caught: "+ e.Message);
            }
        }


        public void TcpDataReader()
        {
            /*stala sa tu zvláštžna vec. Socket bol zatvorený, ale ako keby bol stále shopný prijímať 0 bajtov.  REceive() neblokovalo postup vlákna.*/
            Logger.Log("[ClientConnectionInstance.cs] TcpSocketDataReader created, on socket : [" + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + "]{" + this.socketEntry.deviceName + "}");
            while (this.clientConnectionInstanceActive) {
                try
                {
                    byte[] buffer = new byte[500000];
                    int length = socketEntry.socket.Receive(buffer);

                    if(length == 0)
                    {
                        throw new Exception("ClientConnectionInstance.cs] TcpDataReader() received 0 bytes, therefore remote endpoint closed the socket : ");
                    }


                    byte[] receivedData = new byte[length];

                    Array.Copy(buffer, 0, receivedData, 0, length);
                    ProcessReceivedData(receivedData);
                }
                catch (Exception e)
                {
                    this.clientConnectionInstanceActive = false;
                    Logger.Log("[ClientConnectionInstance.cs] TcpSocketDataReader except1ion: " + e.Message);
                }
             }
        }
    }
}

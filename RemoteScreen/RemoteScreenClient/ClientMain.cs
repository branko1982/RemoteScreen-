/*
súbor naposledy editovaný 26.4.2019
*/

using System;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;

namespace RemoteScreenTransmiter
{
    /*dôvod prečo v tejto aplikácií nemám pre socket vytvorenú samostatnú triedu ako to je v Serveri je takýto.
     Tu bol potrebný len jeden socket, jeden 
        */
    class ClientMain
    {
        [DllImport("gdi32.dll")]
        /* Get Device Caps, funkcia z gdi32.dll používaná v GetDisplayUiScaleConstant()  */
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }
        /*
         *
         ** metóda používaná pre zmenšenie veľkosti zachyteného snímku obrazovky
         * Rozmery na ktoré sa má obrázok upraviť sú definované v triede Config
         * .*/
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            try
            {
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RemoteScreenMain.ResizeImage() exception caught: " + ex.Message);
            }
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                /*
                 determines whether pixels from a source image overwrite or are combined with background pixels.
                 SourceCopy specifies that when a color is rendered, it overwrites the background color.
                 */
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                /* determines the rendering quality level of layered images*/

                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                /*
                 determines how intermediate values between two endpoints are calculated
                 */
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                /*
                 specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing) -- probably only works on vectors    
                 */
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                /*
                 affects rendering quality when drawing the new image
                 */
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    /*
                     prevents ghosting around the image borders -- naïve resizing will sample transparent pixels beyond the image boundaries,
                     but by mirroring the image we can get a better sample (this setting is very noticeable)
                     */
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        /*
         * metóda GetDisplayUiScaleConstant pre zistí číslo ktoré predstavuje na koľko, ak vôbec, je škálované rozhranie v systéme. Napríklad keď je UI škálovaná na 150%, metóda navráti hodnotu 1.5.
         metóda je potrebná pretože bez by program nebol schopný správne zistiť rozlíšenie displeja, ak by mal displej nastavené škálovanie užívateľského rozhrania na inú hodnotu ako 100%.
         */
        private float GetDisplayUiScaleConstant()
        {
            float ScreenScalingFactor = 0;
            bool successfullyRetrievedScale = true;
            try
            {
                var g = Graphics.FromHwnd(IntPtr.Zero);
                var desktop = g.GetHdc();
                int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
                int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

                ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("[RemoteScreenMain.GetScalingFactor()] unable to get proper scaling factor of display. Scale is Set to 1.");
                successfullyRetrievedScale = false;
            }
            if(!successfullyRetrievedScale)
            {
                /*v prípade že sa nedokáže zistiť nastavenie, navráti sa 1. */
                ScreenScalingFactor = 1;
            }
            return ScreenScalingFactor; 

        }

        private bool continueRunning;
        private Socket socket;

        public ClientMain()
        {
            this.continueRunning = false;
        }

        private void TcpDataReader()
        {
            Console.WriteLine("[ClientMain.cs] TcpDataReader() thread created");

            while (socket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[100];

                    /*bytes are read. Maximum possible count of read bytes will be 100. Because data sent to client do not need to be big at all. Data sent here are just small string messages*/
                    int length = this.socket.Receive(buffer);
                    string response = Encoding.UTF8.GetString(buffer, 0, length);


                    switch (response)
                    {
                        case "continue":
                            continueRunning = true;
                            Console.WriteLine("continuing running");
                            break;
                        case "send_video":
                            Config.SendingScreenOutputActive = true;
                            continueRunning = true; /*continue running is also set to true. Initially. It just makes sense to do it here, this way.*/
                            Console.WriteLine("send video message received");
                            break;

                        case "stop_video":
                            Config.SendingScreenOutputActive = false;
                            Console.WriteLine("stop_video message received");

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            //Console.WriteLine("[ClientMain.cs] TcpDataReader() socket is not connected. closing thread");
        }

        private void TcpVideoOutputSender()
        {
            while (socket.Connected)
            {
                try
                {
                    if (Config.SendingScreenOutputActive)
                    {
                        while (!continueRunning)
                        {
                            //Console.WriteLine("continue running is set to false");
                            Thread.Sleep(1);
                        }
                        if (Config.SendingScreenOutputActive)
                        {
                            Console.WriteLine("requested another image");
                            /*Naozaj som sa snažil optimalizovať kód aby zaberal čo najmenej zdrojov ktoré zariadenie poskytuje (hlavne RAMka), aby bol v task manageri, "neviditeľný"*/
                            /**/
                            continueRunning = false;


                            var bounds = Screen.GetBounds(Point.Empty);
                            float scaleFactor = GetDisplayUiScaleConstant();
                            int realHeight = Convert.ToInt32(bounds.Height * scaleFactor);
                            int realWidth = Convert.ToInt32(bounds.Width * scaleFactor);
                            var bitmap = new Bitmap(realWidth, realHeight);
                            var size = new Size();
                            size.Height = realHeight;
                            size.Width = realWidth;
                            var g = Graphics.FromImage(bitmap);
                            g.CopyFromScreen(Point.Empty, Point.Empty, size);
                            bitmap = ResizeImage(bitmap, Config.ScreenMaxWidth, Config.ScreenMaxHeight);
                            var stream = new MemoryStream();
                            bitmap.Save(stream, ImageFormat.Jpeg);
                            bitmap = null;
                            byte[] outputBuffer = stream.ToArray();
                            stream.Close();
                            stream = null;
                            Console.WriteLine("buffer size: " + outputBuffer.Length);

                            /*first, the length of the image is sent to server, and then the actual image.*/
                            
                            byte[] imageSize = BitConverter.GetBytes(outputBuffer.Length);
                            byte[] outputBuffer_bufferSize = new byte[5];

                            outputBuffer_bufferSize[0] = 81;
                            Array.Copy(imageSize, 0, outputBuffer_bufferSize, 1, 4);
                            /*získam odpoveď zo servera odhľadm toho že môžem poslať dalšie dáta*/

                            /* 
                            int count = 0;
                            foreach(byte x in outputBuffer_bufferSize)
                            {
                                Console.WriteLine(count + ":" + x);
                                count++;
                            }*/

                            Console.WriteLine("sent size of image to server" + outputBuffer_bufferSize);

                            socket.Send(outputBuffer_bufferSize); //pošle 5 bajtov

                            Console.WriteLine("image length sent to server -> " + Encoding.UTF8.GetString(outputBuffer_bufferSize));
                            //klient odošle druhej strane informáciu o veľkosti dát

                            while (!continueRunning && Config.SendingScreenOutputActive)
                            {
                                Thread.Sleep(1);
                            }

                            if (Config.SendingScreenOutputActive)
                            {
                                Console.WriteLine("continue message received, sending rest of image");
                                //odošlú sa obrázok (v prípade tejto aplikácie je to okolo 100-200kb
                                //Console.WriteLine("sending another image");
                                continueRunning = false; //toto má svoje opodstatenie že to je tu.
                                //Nemôžem túto premennú priradiť za socket.Send, pretože by sa mohlo stať ako sa aj stávalo, 
                                //že Mi došiel od servera "continue" správa, ešte predtým ako sa continueRunning nastavil na false,
                                //aby program fungoval continueRuning hodnota sa musí nastaviť najprv na false, a prepísať na true,
                                //ju má spomínaná continue správa od servera. Nemôže to byť naopak. Teda continueRunning nemôže byť ttrue,
                                //a program ju nemá prepísať na false až po tom čo je prijatá "continue" správa. Pretože v tom préípade program zamrzne
                                socket.Send(outputBuffer);
                            }
                            g = null;
                            outputBuffer = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    /*If exception will be caught, which sometimes happens if image is being captured while user enters admin password, single byte will be sent to server, which will tell the server that exception
                     has occured on client side, and server will simply request another image from client. This was needed, because if exception happened, client stuck while being in loop.
                    
                    After the exception, the while loop inside TcpVideoOutputSender continued from start, but then it encountered condition where bool of continueRunning was checked. While loop continueys only if value of continueRunning is true. Else it waits.
                    The problem is, the value of continueRunning was set to true only if client send a image to server sucesfully, without exception, and then server said: hey, you sent me me image which is okay, now you must send me another iamge.
                    But that was not happening, as ... the image, because of exception was never sent. And because the image was never sent, value of bool variable was set to false,and because it was set to false, the loop was stuck and sending of images did not continue.

                    Thats why this was needed. To tell the server and error occured while sending the image. there will be of course some wait time. Like... 1 second. Before another try.
                     
                     */
                    Thread.Sleep(1000); //thread waits for 1 second..

                    byte[] data = new byte[1];
                    data[0] = 51;
                    socket.Send(data);
                    Console.WriteLine("Exception caught: " + ex.Message);
                }
                Thread.Sleep(Config.WaitTimeBeforeSendingImages);
            }
        }
        public void Main() {
            Console.WriteLine("[ClientMain.cs] Main()");

            //zavináč pretože som keyword použil ako názov premennej
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(Config.RemoteServerAddress), Config.RemoteServerPort);

                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);

                /*po tom čo sa socket pripojí na server pošle serveru dáta ktoré budú hovoriť to že je to klient, a tiež sa serveru spolu s tým pošle názov zariadenia*/

                string dataToBeSent = "client";
                dataToBeSent += "|";
                dataToBeSent += Environment.UserName;
                dataToBeSent += "@";
                dataToBeSent += Environment.UserDomainName;


                socket.Send(Encoding.UTF8.GetBytes(dataToBeSent));


                byte[] receivedData = new byte[10]; // 10 bajtov stačí
                int length = this.socket.Receive(receivedData);

                string receivedText = Encoding.UTF8.GetString(receivedData, 0, length);

                //tu bola chyba. Klient nedostal správu od servera že sa úspešne pripojil
                if (receivedText == "auth_ok")
                {
                    //Console.WriteLine("auth_ok message received, program can continue");
                    Thread t1 = new Thread(new ThreadStart(this.TcpVideoOutputSender));
                    t1.IsBackground = true;
                    t1.Start();

                    Thread t2 = new Thread(new ThreadStart(this.TcpDataReader));
                    t2.IsBackground = true;
                    t2.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ClientMain.cs exception caught" + ex.Message);
            }   
        }
    }
}

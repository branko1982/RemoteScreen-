using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Generic;


namespace RemoteScreenServer
{
    class Program
    {


        static void Main(string[] args)
        {
            int port = 0; //dať tu 0
            var ipAddress = IPAddress.Any;
            TcpListener tcpListener;

            if (args.Length == 1)
            {
                if(args[0] == "-h" || args[0] == "help")
                {
                    Console.WriteLine("Remote_Screen_Server.exe -p [PORT]");
                    return;
                }
            }
            //inicializujem premenné
            if (args.Length != 2)
            {
                Console.WriteLine("wrong startup parameters");
                return;
            }
            if(args[0] != "-p")
            {
                Console.WriteLine("wrong startup parameters");
            }

            try
            {
                port = Int32.Parse(args[1]);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            try
            {
                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
                Logger.Log("TcpListener created on port ->" + port);

            }
            catch (Exception e)
            {
                Logger.Log("Exception caught ->" + e);
                return;
            }
            

            /*Here the thread is created which will continuously iterate trough all SocketEntry objects in the List, and if object will contain closed socket, it will be removed from List<> */






            SocketConnectionChecker socketConnectionChecker = new SocketConnectionChecker();

            Thread socketChecker = new Thread(new ThreadStart(socketConnectionChecker.Main));
            socketChecker.IsBackground = false; //Program.Main metóda bude musieť počkať kým vlákno dokončí svoj chod, predtým než bude postupovať ďalej;
            socketChecker.Start();



            TcpSocketListener tcpSocketListener = new TcpSocketListener();
            tcpSocketListener.tcpListener = tcpListener;

            Thread t1 = new Thread(new ThreadStart(tcpSocketListener.Main));
            t1.IsBackground = false; //Program.Main metóda bude musieť počkať kým vlákno dokončí svoj chod, predtým než bude postupovať ďalej;
            t1.Start();


            ClientCommandHandler clientCommandHandler = new ClientCommandHandler();

            Thread t2 = new Thread(new ThreadStart(clientCommandHandler.Main));
            t2.IsBackground = true;
            t2.Start();
        }
    }
}

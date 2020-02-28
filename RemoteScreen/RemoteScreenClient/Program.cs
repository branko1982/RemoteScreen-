using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace RemoteScreenTransmiter
{
    static class Program
    {

        public static void KillOtherInstancesOfProgram()
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) && (p.MainModule.FileName == curr.MainModule.FileName))
                {
                    p.Kill();
                }
            }
        }



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {


            if (args.Length == 1)
            {
                if (args[0] == "-h" || args[0] == "help")
                {
                    Console.WriteLine("Remote_Screen_Server.exe -ip [IP_ADDRESS:string] -p [PORT:int] -t [milliseconds:int]");
                    return;
                }
            }
            //inicializujem premenné
            if (args.Length != 6)
            {
                Console.WriteLine("wrong startup parameters");
                return;
            }
            if (args[0] != "-ip" || args[2] != "-p" || args[4] != "-t")
            {
                Console.WriteLine("wrong startup parameters");
            }
            int port = 0;
            string ipAddress = args[1];
            int waitTimeBeforeSendingImages = 0;
            try
            {
                port = Int32.Parse(args[3]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            try
            {
                waitTimeBeforeSendingImages = Int32.Parse(args[5]);
                if(waitTimeBeforeSendingImages == 0)
                {
                    throw new Exception("waiting time cannot be null");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }


            Config.WaitTimeBeforeSendingImages = waitTimeBeforeSendingImages;
            Config.RemoteServerPort = port;
            Config.RemoteServerAddress = ipAddress;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

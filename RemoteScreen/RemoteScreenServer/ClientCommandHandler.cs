using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteScreenServer
{
    class ClientCommandHandler
    {
        private bool runningAllowed = true;


        public void Main()
        {
            Logger.Log("[ClientCommandHandler.cs] thread started");
            while (this.runningAllowed) //
            {
                try
                {

                    foreach (SocketEntry socketEntry in SocketEntryManager.GetSocketEntries())
                    {
                        if (socketEntry.type != "operator")
                        {
                            if (socketEntry.commandsList.Count > 0)
                            {
                                switch (socketEntry.commandsList[0])
                                {
                                    /*dáta stačí posielať v stringovom formáte. Klient neprijíma žiadne json stringy ani video*/
                                    case "send_screen_to_server":
                                        /*Naštastie klient môže správy získavať */
                                        socketEntry.videoStreamActive = true;
                                        socketEntry.socket.Send(Encoding.UTF8.GetBytes("send_video"));
                                        break;

                                    case "stop_video":
                                        //Logger.Log("[ClientCommandHandler.cs] telling a client to stop sending video");
                                        socketEntry.videoStreamActive = false;
                                        socketEntry.receivedScreenCaptures.Clear(); //vyčistia sa zachytené screenshoty
                                        socketEntry.socket.Send(Encoding.UTF8.GetBytes("stop_video"));
                                        break;
                                }

                                socketEntry.commandsList.Remove(socketEntry.commandsList[0]); // príkaz sa vymaže zo socketu, pretože už je spracovaný
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("CommandHandler exception" + e.Message);
                }
                Thread.Sleep(1); //musí tu byť sleep, Aby proces príliš nepoužíval CPU. Ak ho nespomalím, použitie CPU pôjde strašne vysoko
            }

        }

    }
}

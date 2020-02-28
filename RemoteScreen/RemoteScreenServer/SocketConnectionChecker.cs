using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteScreenServer
{
    class SocketConnectionChecker
    {
        public void Main()
        {
            Logger.Log("[SocketConnectionChecker.cs] SocketConnectionChecker thread started: ");

            while (true)
            {
                foreach (SocketEntry socketEntry in SocketEntryManager.GetSocketEntries())
                {
                    try
                    {
                        if(!socketEntry.socket.Connected || socketEntry.socket == null)
                        {
                            if(socketEntry.type == "operator")
                            {
                                foreach(SocketEntry socketEntry1 in SocketEntryManager.GetSocketEntries())
                                {
                                    if(socketEntry1.videoStreamActive)
                                    {
                                        Logger.Log("found socket entry with active video output");
                                        socketEntry1.videoStreamActive = false;
                                        socketEntry1.receivedScreenCaptures.Clear();
                                        socketEntry1.socket.Send(Encoding.UTF8.GetBytes("stop_video"));
                                    }
                                }
                                
                            }
                            Logger.Log("[SocketConnectionChecker.cs] SocketEntry: " + socketEntry.deviceName + " removed");
                            SocketEntryManager.GetSocketEntries().Remove(socketEntry);
                            break;
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.Log("[SocketConnectionChecker.cs] " + e.Message);
                    }
                }
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}

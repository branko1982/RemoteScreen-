using System.Collections.Generic;
using System.Net.Sockets;

namespace RemoteScreenServer
{
    class SocketEntry
    {
        public bool videoStreamActive;

        public List<byte[]> dataToBeSent;
        public List<byte[]> receivedScreenCaptures;

        public string deviceName;
        public Socket socket;

        public string type;

        /// <summary>
        /// CommandList je list príkazov ktoré spracuje príslušné vlákno ktoré socket používa. Je to List cez ktorý môžem z OperatorConnectionInstance triedy predávať vláknam v ClientConnecitonInstance príkazy.
        /// </summary>
        public List<string> commandsList;

        public SocketEntry(string deviceName, Socket socket, string type)
        {
            receivedScreenCaptures = new List<byte[]>();
            dataToBeSent = new List<byte[]>();
            this.deviceName = deviceName;
            this.socket = socket;
            this.type = type;
            videoStreamActive = false;
            commandsList = new List<string>();
        }
    }
}

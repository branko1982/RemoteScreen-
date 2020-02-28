using System;
using System.Net.Sockets;
using System.Text;

namespace RemoteScreenServer
{
    class SingleConnectionInstanceHandler
    {
        public Socket socket;
        public void Main()
        {
            try
            {
                /*táto časť funguje dobre a nepotrebuje debugovanie*/
                byte[] receivingBuffer = new byte[300];
                int receivedDataLength = this.socket.Receive(receivingBuffer);

                string receivedText = Encoding.UTF8.GetString(receivingBuffer, 0, receivedDataLength);

                string[] parts = receivedText.Split(Convert.ToChar("|"));

                if (parts.Length != 2)
                {
                    Logger.Log("Data from socket received in wrong format. Closing socket: [" + this.socket.RemoteEndPoint.ToString() + "] ");
                    return;
                }

                if (parts[0] == "client")
                {
                    SocketEntry socketEntry = new SocketEntry(parts[1],socket,"client");

                    SocketEntryManager.GetSocketEntries().Add(socketEntry);

                    ClientConnectionInstance clientConnectionInstance = new ClientConnectionInstance();
                    clientConnectionInstance.socketEntry = socketEntry;
                    clientConnectionInstance.Main();
                }
                if (parts[0] == "operator")
                {

                    SocketEntry socketEntry = new SocketEntry("", socket, "operator");

                    SocketEntryManager.GetSocketEntries().Add(socketEntry);

                    OperatorConnectionInstance operatorConnectionInstance = new OperatorConnectionInstance(socketEntry);
                    operatorConnectionInstance.Main();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

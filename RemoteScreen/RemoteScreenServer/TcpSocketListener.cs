using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemoteScreenServer
{
    class TcpSocketListener
    { 
        /// <summary>
        /// tcpListener objekt, vytvorený v hlavnom vlákne, a z neho predaný do TcpSocketListener objektu
        /// </summary>
        public TcpListener tcpListener;

        public void Main()
        {
            Logger.Log("[TcpSocketListener.cs]  thread started");

            while (true)
            {
                Socket socket = tcpListener.AcceptSocket();
                IPEndPoint endpoint = socket.RemoteEndPoint as IPEndPoint;
                Logger.Log("[TcpSocketListener.cs] connection established with " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString());

                SingleConnectionInstanceHandler singleConnectionInstanceHandler = new SingleConnectionInstanceHandler();
                singleConnectionInstanceHandler.socket = socket;

                Thread t2 = new Thread(new ThreadStart(singleConnectionInstanceHandler.Main));
                t2.IsBackground = true;
                t2.Start();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomNetworking;
using System.Net.Sockets;

namespace SS
{
    class ClientSocketStuff
    {
        private string ipAddress = "localhost";
        private string userName;
        private string password;
        private StringSocket socket;

        public ClientSocketStuff()
        {
            TcpClient client = new TcpClient(ipAddress, 1984);
            Socket sock = client.Client;
            socket = new StringSocket(sock, new UTF8Encoding());

        }

        private void SendCallback(Exception e, object o) { }

        private void ReceiveMessage(String message, Exception e, object o)
        {

        }

    }
}

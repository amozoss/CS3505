using SS;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using CustomNetworking;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace ConsoleApplication1
{
        public class ChangeCellTest1
        {
            static void Main(string[] args)
            {
            }
            private ClientSocketStuff ssClient;
            private StringSocket serverSocket;
            private string messagesFromClient;
            private TcpListener server;
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;
            private int messageReceivedCount;
            private List<String> clientMessages;


            // Timeout used in test case
            private static int timeout = 2000;


            public ChangeCellTest1(string ipAddress, int port)
            {
                // the listener
                this.server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                this.server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;
                //ClientSocketStuff 

                this.server.BeginAcceptSocket(ConnectionReceived, null);
                ssClient = new SS.ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port);
            }

            // setup connection to client
            private void ConnectionReceived(IAsyncResult ar)
            {
                Socket socket = server.EndAcceptSocket(ar);
                serverSocket = new StringSocket(socket, UTF8Encoding.Default);
                serverSocket.BeginReceive(ReceiveStuff, null);
                server.BeginAcceptSocket(ConnectionReceived, null);
            }

            // have the client send a change message to the server
            public void run()
            {
                // This will coordinate communication between the threads of the test cases
                mre1 = new ManualResetEvent(false);
                mre2 = new ManualResetEvent(false);
                messageReceivedCount = 0;

                // client cell changes 
                string cellName = "jackson";
                string cellContent = "jackson";

               

                ssClient.ChangeCell(cellName, cellContent);
                // wait for messages 

                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);
               
                Console.WriteLine(clientMessages);



            }

            private void ReceiveStuff(String words, Exception e, object payload)
            {
                messagesFromClient += " " + words;
                Debug.WriteLine("Received a message: " + words);
                clientMessages.Add(words);
                messageReceivedCount++;
               
                if (messageReceivedCount >= 6)
                    mre1.Set();

               

            }



            private void something(string somethingElse, bool isError)
            {
                Debug.WriteLine("Hey GUI, UPDATE!");
            }

        }// end ChangeCellTest1 class


        public class JoinResponseTest1
        {
            private ClientSocketStuff ssClient;
            private StringSocket serverSocket;
            private string messagesFromClient;
            private TcpListener server;
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;
            private int messageReceivedCount;
            private List<String> clientMessages;
            private static int numberOfMessages = 3;


            // Timeout used in test case
            private static int timeout = 20000;


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public JoinResponseTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = delegate(string dis, bool dat) { };

                server.BeginAcceptSocket(ConnectionReceived, null);
                ssClient = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port);
            }

            // setup connection to client
            private void ConnectionReceived(IAsyncResult ar)
            {
                Socket socket = server.EndAcceptSocket(ar);
                serverSocket = new StringSocket(socket, UTF8Encoding.Default);
                serverSocket.BeginReceive(ReceiveStuff, null);
                server.BeginAcceptSocket(ConnectionReceived, null);
            }

            // have the client send a change message to the server
            public void run()
            {
                // This will coordinate communication between the threads of the test cases
                mre1 = new ManualResetEvent(false);
                mre2 = new ManualResetEvent(false);
                messageReceivedCount = 0;

                string name = "test1";
                string password = "jackson";
                string version = "0";
                string length = "3";
                string xml = "<name>xml</name>";

                ssClient.JoinSpreadsheet(name, password);
                // wait for messages 



                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);


                //Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                //Assert.AreEqual(clientMessages.Contains("JOIN"), true);
                //Assert.AreEqual(clientMessages.Contains("Password:jackson"), true);
                //Assert.AreEqual(clientMessages.Contains("Name:test1"), true);




                // Success Message 
                serverSocket.BeginSend("JOIN SP OK\n" + "Name:" + name + "\n" +
                                        "Verison:" + version + "\n" + "Length:" + length +
                                        "\n" + xml + "\n", ServerSendCallback, null);

                // Thread.Sleep(30000);
                //Assert.AreEqual(true, mre2.WaitOne(timeout), "Timed out waiting 1");

            }

            private void ServerSendCallback(Exception e, object payload)
            {
                Debug.WriteLine("Sent message");
                mre2.Set();
            }

            private void ReceiveStuff(String words, Exception e, object payload)
            {
                messagesFromClient += " " + words;
                Debug.WriteLine("Received a message: " + words);
                clientMessages.Add(words);
                messageReceivedCount++;

                if (messageReceivedCount >= numberOfMessages)
                    mre1.Set();



            }



            private void something(string somethingElse, bool isError)
            {
                Debug.WriteLine("Hey GUI, UPDATE!");
            }

        }// end JoinResponseTest1 class

}

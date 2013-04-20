using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using CustomNetworking;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    
    
    /// <summary>
    ///This is a test class for ClientSocketStuffTest and is intended
    ///to contain all ClientSocketStuffTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClientSocketStuffTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Client Send Tests
        /// <summary>
        ///A test for ChangeCell
        ///</summary>
        [TestMethod()]
        public void ClientChangeCellSendTest()
        {
            string ipAddress = "localhost";
            int port = 2000;

            ChangeCellTest1 cellTest = new ChangeCellTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

        public class ChangeCellTest1
        {
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


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public ChangeCellTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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
                
                

                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");
                
                Assert.AreEqual(clientMessages.Contains("CHANGE"),true);
                Assert.AreEqual(clientMessages.Contains("Cell:jackson"), true);
                Assert.AreEqual(clientMessages.Contains("Name:"), true);
                Assert.AreEqual(clientMessages.Contains("Length:7"), true);
                Assert.AreEqual(clientMessages.Contains("Version:0"), true);
                Assert.AreEqual(clientMessages.Contains("jackson"), true);


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


        /// <summary>
        ///A test for ChangeCell
        ///</summary>
        [TestMethod()]
        public void ChangeCellTester()
        {
            string ipAddress = "localhost";
            int port = 1984;
            Spreadsheet spreadsheet = new Spreadsheet();
            ClientSocketStuff ssClient = new ClientSocketStuff(ipAddress, spreadsheet, null, port);
            //ChangeCellTest1 cellTest = new ChangeCellTest1(ipAddress, port);
            ssClient.ChangeCell("jackson", "jackson");
            while (true)
            {


            }

            //cellTest.run();
            // assertions are made in the run() method

        }

        /// <summary>
        ///A test for Create send
        ///</summary>
        [TestMethod()]
        public void ClientCreateSendTest()
        {
            string ipAddress = "localhost";
            int port = 2001;

            CreateTest1 cellTest = new CreateTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

        public class CreateTest1
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
            private static int timeout = 2000;


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public CreateTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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

               
                ssClient.CreateSpreadsheet("test1", "password");
                // wait for messages 


                
                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);
                
             
                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                Assert.AreEqual(clientMessages.Contains("CREATE"), true);
                Assert.AreEqual(clientMessages.Contains("Password:password"), true);
                Assert.AreEqual(clientMessages.Contains("Name:test1"), true);
              


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



            private void something(string somethingElse, bool isErrorMessage)
            {
                Debug.WriteLine("Hey GUI, UPDATE!");
            }

        }// end ChangeCellTest1 class

        /// <summary>
        ///A test for Join Send
        ///</summary>
        [TestMethod()]
        public void ClientJoinSendTest()
        {
            string ipAddress = "localhost";
            int port = 2007;

            JoinTest1 cellTest = new JoinTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

        public class JoinTest1
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
            private static int timeout = 2000;


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public JoinTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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


                ssClient.JoinSpreadsheet("test1", "jackson");
                // wait for messages 



                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);


                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                Assert.AreEqual(clientMessages.Contains("JOIN"), true);
                Assert.AreEqual(clientMessages.Contains("Password:jackson"), true);
                Assert.AreEqual(clientMessages.Contains("Name:test1"), true);



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

        }// end ChangeCellTest1 class

        /// <summary>
        ///A test for Undo Send
        ///</summary>
        [TestMethod()]
        public void ClientUndoSendTest()
        {
            string ipAddress = "localhost";
            int port = 3000;

            UndoTest1 cellTest = new UndoTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

        public class UndoTest1
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
            private static int timeout = 2000;


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public UndoTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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


                ssClient.Undo();
                // wait for messages 



                serverSocket.BeginReceive(ReceiveStuff, null);
                serverSocket.BeginReceive(ReceiveStuff, null);


                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                Assert.AreEqual(clientMessages.Contains("UNDO"), true);
                Assert.AreEqual(clientMessages.Contains("Version:0"), true);
                Assert.AreEqual(clientMessages.Contains("Name:"), true);



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

        }// end ChangeCellTest1 class

        /// <summary>
        ///A test for Save Send
        ///</summary>
        [TestMethod()]
        public void ClientSaveSendTest()
        {
            string ipAddress = "localhost";
            int port = 2003;

            SaveTest1 cellTest = new SaveTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

        public class SaveTest1
        {
            private ClientSocketStuff ssClient;
            private StringSocket serverSocket;
            private string messagesFromClient;
            private TcpListener server;
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;
            private int messageReceivedCount;
            private List<String> clientMessages;
            private static int numberOfMessages = 2;

            // Timeout used in test case
            private static int timeout = 2000;


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public SaveTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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


                ssClient.Save();
                // wait for messages 



                serverSocket.BeginReceive(ReceiveStuff, null);


                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                Assert.AreEqual(clientMessages.Contains("SAVE"), true);
                Assert.AreEqual(clientMessages.Contains("Name:"), true);



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

        }// end ChangeCellTest1 class

        /// <summary>
        ///A test for Save Send
        ///</summary>
        [TestMethod()]
        public void ClientLeaveSendTest()
        {
            string ipAddress = "localhost";
            int port = 2004;

            LeaveTest1 cellTest = new LeaveTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

        public class LeaveTest1
        {
            private ClientSocketStuff ssClient;
            private StringSocket serverSocket;
            private string messagesFromClient;
            private TcpListener server;
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;
            private int messageReceivedCount;
            private List<String> clientMessages;
            private static int numberOfMessages = 2;

            // Timeout used in test case
            private static int timeout = 2000;


            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public LeaveTest1(string ipAddress, int port)
            {
                // the listener
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null;
                server.Start();
                clientMessages = new List<string>();
                // the is the GUI message sender thing
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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


                ssClient.Leave();
                // wait for messages 



                serverSocket.BeginReceive(ReceiveStuff, null);


                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                Assert.AreEqual(clientMessages.Contains("LEAVE"), true);
                Assert.AreEqual(clientMessages.Contains("Name:"), true);



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

        }// end ChangeCellTest1 class
        #endregion


        #region Server Responses Tests
        /// <summary>
        ///A test for Join Response
        ///</summary>
        [TestMethod()]
        public void ServerJoinResponseTest()
        {
            string ipAddress = "localhost";
            int port = 4000;

            JoinResponseTest1 cellTest = new JoinResponseTest1(ipAddress, port);

            cellTest.run();
            // assertions are made in the run() method

        }

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
                ClientSocketStuff.ClientToGUI_SS receivedMessage = something;

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


                Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");

                Assert.AreEqual(clientMessages.Contains("JOIN"), true);
                Assert.AreEqual(clientMessages.Contains("Password:jackson"), true);
                Assert.AreEqual(clientMessages.Contains("Name:test1"), true);



            
                // Success Message 
                serverSocket.BeginSend("JOIN SP OK\n" + "Name:" + name + "\n" +
                                        "Verison:" + version + "\n" + "Length:" + length +
                                        "\n" + xml + "\n",ServerSendCallback, null);

               // Thread.Sleep(30000);
                Assert.AreEqual(true, mre2.WaitOne(timeout), "Timed out waiting 1");
                   
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

        }// end ChangeCellTest1 class


        #endregion
    } // end clientScoketTests
}// end namespace

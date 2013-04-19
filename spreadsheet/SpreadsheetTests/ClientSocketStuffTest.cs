using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using CustomNetworking;
using System.Threading;
using System.Diagnostics;

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


        /// <summary>
        ///A test for ChangeCell
        ///</summary>
        [TestMethod()]
        public void ChangeCellTest()
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

                // the is the GUI message sender thing
                ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = something;

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
                Assert.AreEqual("CHANGE", messagesFromClient);

            }

            private void ReceiveStuff(String words, Exception e, object payload)
            {
                messagesFromClient += " " + words;
                Debug.WriteLine("Received a message: " + words);
                messageReceivedCount++;
               
                if (messageReceivedCount >= 6)
                    mre1.Set();

               

            }



            private void something(string somethingElse)
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


    } // end clientScoketTests
}// end namespace

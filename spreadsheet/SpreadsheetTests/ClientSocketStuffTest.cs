﻿using SS;
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
            string words = cellTest.run();
            string newWords = "";

            Assert.Inconclusive(words);
        }

        public class ChangeCellTest1
        {
            private ClientSocketStuff target;
            private StringSocket sendSocket;
            private string message;
            private TcpListener server;

            /// <summary>
            /// This will be the default test class constructor for ClientSocketStuffTest.
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <param name="port"></param>
            public ChangeCellTest1(string ipAddress, int port)
            {
                server = new TcpListener(IPAddress.Any, port);
                Spreadsheet spreadsheet = null; 
                server.Start();
                ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = something;
                
                server.BeginAcceptSocket(ConnectionReceived, null);
                target = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port);
            }

            private void ConnectionReceived(IAsyncResult ar)
            {
                Socket socket = server.EndAcceptSocket(ar);
                sendSocket = new StringSocket(socket, UTF8Encoding.Default);
                sendSocket.BeginReceive(ReceiveStuff, null);
                server.BeginAcceptSocket(ConnectionReceived, null);
            }

            public string run()
            { 
                string cellName = "jackson";
                string cellContent = "jackson";

                target.ChangeCell(cellName, cellContent);
                Thread.Sleep(3000);
                return message;
            }

            private void ReceiveStuff(String words, Exception e, object payload)
            {
                message += words.Trim();
                sendSocket.BeginReceive(ReceiveStuff, null);
            }



            private void something(string somethingElse)
            {

            }

        }
        



        /// <summary>
        ///A test for ChangeCellCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void ChangeCellCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.ChangeCellCallback(message, e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            Spreadsheet spreadsheet = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = null; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            ClientSocketStuff target = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CreateSSCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void CreateSSCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.CreateSSCallback(message, e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CreateSpreadsheet
        ///</summary>
        [TestMethod()]
        public void CreateSpreadsheetTest()
        {
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            Spreadsheet spreadsheet = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = null; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            ClientSocketStuff target = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            target.CreateSpreadsheet(name, password);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for JoinSSCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void JoinSSCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.JoinSSCallback(message, e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for JoinSpreadsheet
        ///</summary>
        [TestMethod()]
        public void JoinSpreadsheetTest()
        {
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            Spreadsheet spreadsheet = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = null; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            ClientSocketStuff target = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            target.JoinSpreadsheet(name, password);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            Spreadsheet spreadsheet = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = null; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            ClientSocketStuff target = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port); // TODO: Initialize to an appropriate value
            target.Save();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SaveCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void SaveCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.SaveCallback(message, e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SendCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void SendCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.SendCallback(e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Undo
        ///</summary>
        [TestMethod()]
        public void UndoTest()
        {
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            Spreadsheet spreadsheet = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff.ClientUpdateGUI_SS receivedMessage = null; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            ClientSocketStuff target = new ClientSocketStuff(ipAddress, spreadsheet, receivedMessage, port); // TODO: Initialize to an appropriate value
            target.Undo();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UndoCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void UndoCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.UndoCallback(message, e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UpdateCallback
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpreadsheetGUI.exe")]
        public void UpdateCallbackTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ClientSocketStuff_Accessor target = new ClientSocketStuff_Accessor(param0); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            Exception e = null; // TODO: Initialize to an appropriate value
            object o = null; // TODO: Initialize to an appropriate value
            target.UpdateCallback(message, e, o);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
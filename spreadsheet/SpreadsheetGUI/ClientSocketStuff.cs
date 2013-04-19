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
        public struct Payload
        {
            public Boolean structure1;
            public int number;
        }


        public delegate void ClientUpdateGUI_SS(String message); // The message will be handled by a separate class 
        private string ipAddress;
        private string nameOfSpreadsheet; // name is the name for the new spreadsheet
        private string password; // password is the password to use for the new spreadsheet
        private StringSocket socket;
        private static int SERVERPORT = 1984;
        private ClientUpdateGUI_SS updateGUI_SS;
        private int version;

        /// <summary>
        ///  Creates the communication outlet that the client will use to "talk" to the server.
        /// </summary>
        /// <param name="ipAddress"></param>
        public ClientSocketStuff(string ipAddress, Spreadsheet spreadsheet,  ClientUpdateGUI_SS receivedMessage, int port)
        {

            // set private instance variables 
            this.ipAddress = ipAddress;
            this.updateGUI_SS = receivedMessage;

            TcpClient client = new TcpClient(ipAddress, port);
            Socket sock = client.Client;
            socket = new StringSocket(sock, new UTF8Encoding());

        }

        private void SendCallback(Exception e, object o) { }

        #region Callbacks

        /// <summary>
        /// Master callback deals with the first "line" of every server response
        /// 
        /// Passed to CreateSSCallback
        /// CREATE SP OK LF
        /// CREATE SP FAIL LF
        /// 
        /// Passed to JoinSSCallback
        /// JOIN SP OK LF
        /// JOIN SP FAIL LF
        /// 
        /// Passed to ChangeCellCallback
        /// CHANGE SP OK LF
        /// CHANGE SP FAIL LF
        /// 
        /// Passed to UndoCallback
        /// UNDO SP OK LF
        /// UNDO SP FAIL LF
        /// 
        /// Passed to SaveCallback
        /// SAVE SP OK LF
        /// SAVE SP FAIL LF
        /// 
        /// Passed to UpdateCallback
        /// UPDATE
        /// 
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void MasterCallback(String message, Exception e, object payload)
        {


        }

        /// <summary>
        /// If the server successfully created the new spreadsheet file, it should respond with
        ///
        ///CREATE SP OK LF
        ///Name:name LF
        ///Password:password LF
        ///
        ///Otherwise, it should respond with
        ///
        ///CREATE SP FAIL LF
        ///Name:name LF
        ///message LF
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void CreateSSCallback(String message, Exception e, object o)
        {
            if (message != null)
            {
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string spaceFirstWord = "";
                string colonFirstWord = "";
                string status = "";
                if (o is string)
                    status = (string)o;

                if (spaceSplitup.Length > 0) 
                     spaceFirstWord = spaceSplitup[0].ToUpper().Trim();
                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (spaceFirstWord.Equals("CREATE"))
                {
                    string thirdWord = spaceSplitup[2].ToUpper().Trim();
                    if (thirdWord.Equals("OK"))
                    {
                        //passed
                        status = "PASSED";
                    }
                    else if (thirdWord.Equals("FAIL"))
                    {
                        //failed
                        status = "FAILED";
                    }
                    socket.BeginReceive(CreateSSCallback, status);
                }
                else if (status.Equals("PASSED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                         socket.BeginReceive(CreateSSCallback, status);
                    }
                    else if (colonFirstWord.Equals("PASSWORD"))
                    {
                         // get password
                    }
                }
                else if (status.Equals("FAILED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(CreateSSCallback, status);
                    }
                    else
                    {
                        // must be a message
                    }
                }
                updateGUI_SS(message); // the message from the server will be parsed in a separate class
            }
        }

        /// <summary>
        /// 
        ///If the request succeeded, the server should respond with
        ///
        ///JOIN SP OK LF
        ///Name:name LF; true 1
        ///Version:version LF; true 2
        ///Length:length LF;true 3
        ///xml LF 
        ///
        ///Otherwise, the server should respond with
        ///JOIN SP FAIL LF
        ///Name:name LF; false 1
        ///message LF; false 2
        /// 
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void JoinSSCallback(String message, Exception e, object payload)
        {
            string[] spaceSplit = message.Split(' ');
            string[] colonSplit = message.Split(':');
            string[] payloadSplit = null;
            string spaceFirstWord = "";
            string colonFirstWord = "";
            string status = "";
            string number = "";
            // This if statement parses the string we send in the payload.
            if (payload is string)
            {
                payloadSplit = ((string)payload).Split(' ');
                status = payloadSplit[0];
                number = payloadSplit[1];
            }

            if (spaceSplit.Length > 0)
                spaceFirstWord = spaceSplit[0].ToUpper().Trim();
            if (colonSplit.Length > 0)
                colonFirstWord = colonSplit[0].ToUpper().Trim();


            if (status.Equals("PASSED"))
            {
                if (colonFirstWord.Equals("NAME") && number.Equals("1"))
                {
                    // get name
                    socket.BeginReceive(JoinSSCallback, status);
                }
                else if (colonFirstWord.Equals("VERSION"))
                {
                    // get Version
                    version = Int32.Parse(colonSplit[1].Trim());
                    socket.BeginReceive(JoinSSCallback, status);
                }
                else if (colonFirstWord.Equals("LENGTH"))
                {
                    // get length
                    socket.BeginReceive(JoinSSCallback, status);
                }
                else 
                {
                    // must be the xml
                }
            }
            else if (status.Equals("FAILED"))
            {
                if (colonFirstWord.Equals("NAME"))
                {
                    // get name
                    socket.BeginReceive(JoinSSCallback, status);
                }
                else
                {
                    // must be a message
                }
            }
            updateGUI_SS(message); // the message from the server will be parsed in a separate class
        }

        /// <summary>
        /// If the request succeeded, the server should respond with
        ///
        ///CHANGE SP OK LF
        ///Name:name LF
        ///Version:version LF
        ///
        ///Otherwise, it should respond with
        ///
        ///CHANGE SP FAIL LF
        ///Name:name LF
        ///Version:version LF
        ///message LF
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void ChangeCellCallback(String message, Exception e, object o)
        {
            if (message != null)
            {
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string spaceFirstWord = "";
                string colonFirstWord = "";
                string status = "";
                if (o is string)
                    status = (string)o;

                if (spaceSplitup.Length > 0)
                    spaceFirstWord = spaceSplitup[0].ToUpper().Trim();
                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (spaceFirstWord.Equals("CHANGE"))
                {
                    string thirdWord = spaceSplitup[2].ToUpper().Trim();
                    if (thirdWord.Equals("OK"))
                    {
                        //passed
                        status = "PASSED";
                    }
                    else if (thirdWord.Equals("FAIL"))
                    {
                        //failed
                        status = "FAILED";
                    }
                    socket.BeginReceive(ChangeCellCallback, status);
                }
                else if (spaceFirstWord.Equals("message"))
                {
                    // message
                    //socket.BeginReceive(mainCallback, something);
                }
                else if (colonFirstWord.Equals("Version"))
                {
                    version = Int32.Parse(colonSplitup[1].Trim());
                    if(status.Equals("FAILED"))
                    {
                        socket.BeginReceive(ChangeCellCallback, status);
                    }
                    //version
                }
                else if (colonFirstWord.Equals("Name"))
                {
                    if (status.Equals("PASSED"))
                    {

                    }
                    else
                    {

                    }
                    // get name
                    socket.BeginReceive(ChangeCellCallback, status);
                }
                updateGUI_SS(message); // the message from the server will be parsed in a separate class
            }
        }

        /// <summary>
        /// If the request succeeded, the server should respond with
        ///
        ///UNDO SP OK LF
        ///Name:name LF
        ///Version:version LF
        ///Cell:cell LF
        ///Length:length LF
        ///content LF
        ///
        ///Otherwise, the server should respond with
        ///
        ///UNDO SP FAIL LF
        ///Name:name LF
        ///Version:version LF
        ///message LF
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void UndoCallback(String message, Exception e, object o)
        {
            if (message != null)
            {
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string spaceFirstWord = "";
                string colonFirstWord = "";
                string status = "";
                if (o is string)
                    status = (string)o;

                if (spaceSplitup.Length > 0)
                    spaceFirstWord = spaceSplitup[0].ToUpper().Trim();
                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (spaceFirstWord.Equals("UNDO"))
                {
                    string thirdWord = spaceSplitup[2].ToUpper().Trim();
                    if (thirdWord.Equals("OK"))
                    {
                        //passed
                        status = "PASSED";
                    }
                    else if (thirdWord.Equals("FAIL"))
                    {
                        //failed
                        status = "FAILED";
                    }
                    else if (thirdWord.Equals("WAIT"))
                    {
                        //wait
                        status = "WAIT";
                    }
                    else if (thirdWord.Equals("WAIT"))
                    {
                        //failed
                        status = "END";
                    }
                    socket.BeginReceive(UndoCallback, status);
                }
                else if (status.Equals("PASSED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else if (colonFirstWord.Equals("VERSION"))
                    {
                        // get Version
                        version = Int32.Parse(colonSplitup[1].Trim());
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else if (colonFirstWord.Equals("CELL"))
                    {
                        // get cell
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else if (colonFirstWord.Equals("LENGTH"))
                    {
                        // get length
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else
                    {
                        // must be the content
                    }
                }
                else if (status.Equals("FAILED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else
                    {
                        // must be a message
                    }
                }
                else if (status.Equals("WAIT"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else if (colonFirstWord.Equals("VERSION"))
                    {
                        version = Int32.Parse(colonSplitup[1].Trim());
                        // get Version
                       
                    }
                }
                else if (status.Equals("END"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(UndoCallback, status);
                    }
                    else if (colonFirstWord.Equals("VERSION"))
                    {
                        version = Int32.Parse(colonSplitup[1].Trim());
                        // get Version

                    }
                }
                updateGUI_SS(message); // the message from the server will be parsed in a separate class

            }
        }

        /// <summary>
        /// If the request succeeds, the server should respond with
        ///SAVE SP OK LF
        ///Name:name LF
        ///
        ///If the request fails, the server should respond with
        ///
        ///SAVE SP FAIL LF
        ///Name:name LF
        ///message LF
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void SaveCallback(String message, Exception e, object o)
        {
            if (message != null)
            {
                string[] splitup = message.Split(' ');
                string firstWord = splitup[0].ToUpper().Trim();
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string spaceFirstWord = "";
                string colonFirstWord = "";
                string status = "";
                if (o is string)
                    status = (string)o;

                if (spaceSplitup.Length > 0)
                    spaceFirstWord = spaceSplitup[0].ToUpper().Trim();
                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (spaceFirstWord.Equals("SAVE"))
                {
                    string thirdWord = spaceSplitup[2].ToUpper().Trim();
                    if (thirdWord.Equals("OK"))
                    {
                        //passed
                        status = "PASSED";
                    }
                    else if (thirdWord.Equals("FAIL"))
                    {
                        //failed
                        status = "FAILED";
                    }
                    socket.BeginReceive(SaveCallback, status);
                }
                else if (status.Equals("PASSED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                    }
                }
                else if (status.Equals("FAILED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(SaveCallback, status);
                    }
                    else
                    {
                        // must be a message
                    }
                }
                updateGUI_SS(message); // the message from the server will be parsed in a separate class

            }
        }

        /// <summary>
        /// To communicate a committed change to other clients, the server should send
        ///UPDATE LF
        ///Name:name LF
        ///Version:version LF
        ///Cell:cell LF
        ///Length:length LF
        ///content LF
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void UpdateCallback(String message, Exception e, object o)
        {
            if (message != null)
            {
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string spaceFirstWord = "";
                string colonFirstWord = "";
                string status = "";
                if (o is string)
                    status = (string)o;

                if (spaceSplitup.Length > 0)
                    spaceFirstWord = spaceSplitup[0].ToUpper().Trim();
                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (spaceFirstWord.Equals("UPDATE"))
                {
                    status = "PASSED";
                    socket.BeginReceive(UpdateCallback, status);
                }
                else if (status.Equals("PASSED"))
                {
                    if (colonFirstWord.Equals("NAME"))
                    {
                        // get name
                        socket.BeginReceive(UpdateCallback, status);
                    }
                    else if (colonFirstWord.Equals("VERSION"))
                    {
                        // get Version
                        version = Int32.Parse(colonSplitup[1].Trim());
                        socket.BeginReceive(UpdateCallback, status);
                    }
                    else if (colonFirstWord.Equals("CELL"))
                    {
                        // get cell
                        socket.BeginReceive(UpdateCallback, status);
                    }
                    else if (colonFirstWord.Equals("LENGTH"))
                    {
                        // get length
                        socket.BeginReceive(UpdateCallback, status);
                    }
                    else
                    {
                        // must be the content
                        socket.BeginReceive(UpdateCallback, "");
                    }
                }
              
                updateGUI_SS(message); // the message from the server will be parsed in a separate class

            }
        }

        #endregion

        #region Send Methods
        /// <summary>
        /// To create a new spreadsheet file on the server, the client should send
        ///
        ///CREATE LF
        ///Name:name LF
        ///Password:password LF
        ///
        ///where
        ///- name is the name for the new spreadsheet
        ///- password is the password to use for the new spreadsheet
        ///
        ///If the server successfully created the new spreadsheet file, it should respond with
        ///
        ///CREATE SP OK LF
        ///Name:name LF
        ///Password:password LF
        ///
        ///where
        ///
        ///- name is the name for the new spreadsheet (a confirmation)
        ///
        ///- password is the password to use for the new spreadsheet (a confirmation)
        ///
        ///Otherwise, it should respond with
        ///
        ///CREATE SP FAIL LF
        ///Name:name LF
        ///message LF
        ///
        ///where
        ///- name is the rejected name
        ///- message is an informative notice. It must not contain any linefeeds. The server should 
        ///provide some reason in message why the request failed (e.g., if an existing spreadsheet 
        ///of the same name already exists).
        /// 
        /// </summary>
        /// <param name="name">name is the name for the new spreadsheet</param>
        /// <param name="password">password is the password to use for the new spreadsheet</param>
        public void CreateSpreadsheet(string name, string password)
        {
            socket.BeginSend("CREATE\n" + name + "\n" + password + "\n", SendCallback, socket);
            socket.BeginReceive(CreateSSCallback, "NOTHING");
        }


        /// <summary>
        /// To begin working on an existing spreadsheet on the server, the client should send 
        /// 
        ///JOIN LF
        ///
        ///Name:name LF
        ///Password:password LF
        ///If the request succeeded, the server should respond with
        ///
        ///JOIN SP OK LF
        ///Name:name LF
        ///Version:version LF
        ///Length:length LF
        ///xml LF
        ///
        ///where
        /// name is the name of the spreadsheet version is the version of the spreadsheet sent
        /// length is the length of the xml content, in characters
        /// xml is the spreadsheet xml
        ///
        ///Otherwise, the server should respond with
        ///JOIN SP FAIL LF
        ///Name:name LF
        ///message LF
        ///
        ///where
        /// name is the spreadsheet the client attempted to log in to
        /// message is a notice of why the request failed. It must not contain any linefeeds. The 
        ///server should provide some reason in message why the request failed (e.g., if the 
        ///spreadsheet does not exist, or the password is incorrect).
        /// </summary>
        /// <param name="name">name is the name of the spreadsheet</param>
        /// <param name="password">password is the password to use for the spreadsheet</param>
        public void JoinSpreadsheet(string name, string password)
        {
            socket.BeginSend("JOIN\n" + name + "\n" + password + "\n", SendCallback, socket);
            socket.BeginReceive(JoinSSCallback, "NOTHING");
        }


        /// <summary>
        ///        To change the contents of a cell in a spreadsheet, the client should send 
        ///CHANGE LF
        ///Name:name LF
        ///Version:version LF
        ///Cell:cell LF
        ///Length:length LF
        ///content LF
        ///
        ///where
        ///
        /// name is the name of the spreadsheet
        /// version is the version of the spreadsheet on the client
        /// cell is the name of the cell to change (case-insensitive)
        /// length is the length of content, in characters
        /// content is the new content for the cell
        ///
        ///If the request succeeded, the server should respond with
        ///
        ///CHANGE SP OK LF
        ///Name:name LFVersion:version LF
        ///
        ///where
        /// name is the name of the spreadsheet changed
        /// version is the new version number of the spreadsheet
        ///
        ///Otherwise, it should respond with
        ///CHANGE SP FAIL LF
        ///Name:name LF
        ///Version:version LF
        ///message LF
        ///
        ///where
        /// name is the name of the spreadsheet for which the change was rejected
        /// version is the current version on the server
        /// message is a notice of why the change was rejected. It must not contain any linefeeds. 
        ///The server should provide some reason in message why the request failed (e.g., the 
        ///client has not logged in to work on the spreadsheet, or the client’s version is out of date).
        /// </summary>
        ///<param name="cellName">name of cell</param>
        /// <param name="cellContent">content of cell</param>
        public void ChangeCell(string cellName, string cellContent)
        {
            socket.BeginSend("CHANGE\n" + version.ToString() + "\n" + password + "\n"
                + cellName + "\n" + cellContent.Length.ToString() + "\n" + cellContent + "\n", SendCallback, socket);
            socket.BeginReceive(ChangeCellCallback, "NOTHING");
        }

        /// <summary>
        ///        To undo the last change made to a spreadsheet, the client should send
        ///UNDO LF
        ///Name:name LF
        ///Version:version LF
        ///
        ///If the request succeeded, the server should respond with
        ///UNDO SP OK LF
        ///Name:name LF
        ///Version:version LF
        ///Cell:cell LF
        ///Length:length LF
        ///content LF
        ///
        ///where
        /// name is the name of the spreadsheet
        /// version is the new version number
        /// cell is the cell to revert length is the length of content, in characters
        /// content is the old content of the cell
        ///
        ///If there are no unsaved changes, the server should respond with
        ///UNDO SP END LF
        ///Name:name LF
        ///Version:version LF
        ///
        ///Otherwise, the server should respond with
        ///UNDO SP FAIL LF
        ///Name:name LF
        ///Version:version LF
        ///message LF
        ///
        ///where
        /// name is the name of the spreadsheet for which the undo failed
        /// version is the version of the spreadsheet on the server
        /// message contains information why the undo failed. It must not contain any linefeeds. 
        ///The server should provide some reason in message why the request failed (e.g., the 
        ///client has logged in to work on the spreadsheet, or client’s version is out of date).
        /// </summary>
        ///
        public void Undo()
        {
            socket.BeginSend("UNDO\n" + nameOfSpreadsheet + "\n" + version.ToString()+ "\n", SendCallback, socket);
            socket.BeginReceive(UndoCallback, "NOTHING");
        }
        /// <summary>
        ///        To save the current state of the spreadsheet and merge all outstanding changes to the existing 
        ///file, the client should send to the server
        ///SAVE LF
        ///Name:name LF
        ///
        ///If the request succeeds, the server should respond with 
        ///SAVE SP OK LF
        ///Name:name LF
        ///
        ///If the request fails, the server should respond with
        ///SAVE SP FAIL LF
        ///Name:name LF
        ///message LF
        ///
        ///where
        /// name is the name of the spreadsheet for which saving failed
        /// message contains information why the save failed. It must not contain any linefeeds. 
        ///The server should provide some reason in message why the request failed (e.g., the 
        ///client has not logged in to work on the spreadsheet).
        /// </summary>
        public void Save()
        {
            socket.BeginSend("SAVE\n" + nameOfSpreadsheet + "\n", SendCallback, socket);
            socket.BeginReceive(UndoCallback, "NOTHING");
        }
   

        /// <summary>
        /// Call this method to close the connection with the current server.
        /// </summary>
        public void Close()
        {
         //   socket.CloseAndShutdown();

        }

        #endregion

    }
}

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
        public ClientSocketStuff(string ipAddress, Spreadsheet spreadsheet,  ClientUpdateGUI_SS receivedMessage)
        {

            // set private instance variables 
            this.ipAddress = ipAddress;
            this.updateGUI_SS = receivedMessage;

            TcpClient client = new TcpClient(ipAddress, SERVERPORT);
            Socket sock = client.Client;
            socket = new StringSocket(sock, new UTF8Encoding());

        }

        private void SendCallback(Exception e, object o) { }

        #region Callbacks

      

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
                    }
                    else if (thirdWord.Equals("FAIL"))
                    {
                        //failed
                    }
                }
                else if (spaceFirstWord.Equals("message"))
                {
                    // message
                }
                else if (colonFirstWord.Equals("Name"))
                {
                    // get name
                }
                else if (colonFirstWord.Equals("Password"))
                {
                    // get password
                }
               
              
                updateGUI_SS(message); // the message from the server will be parsed in a separate class
            }
        }

        /// <summary>
        /// 
        ///If the request succeeded, the server should respond with
        ///
        ///JOIN SP OK LF
        ///Name:name LF
        ///Version:version LF
        ///Length:length LF
        ///xml LF
        ///
        ///Otherwise, the server should respond with
        ///JOIN SP FAIL LF
        ///Name:name LF
        ///message LF
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="o"></param>
        private void JoinSSCallback(String message, Exception e, object o)
        {
            if (message != null)
            {
                string[] splitup = message.Split(' ');
                string firstWord = splitup[0].ToUpper().Trim();

                
            }
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

                if (spaceSplitup.Length > 0)
                    spaceFirstWord = spaceSplitup[0].ToUpper().Trim();
                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (spaceFirstWord.Equals("CHANGE"))
                {
                    string thirdWord = spaceSplitup[2].ToUpper().Trim();
                    string okOrNot = "";
                    if (thirdWord.Equals("OK"))
                    {
                        //passed
                        okOrNot = "PASSED";
                    }
                    else if (thirdWord.Equals("FAIL"))
                    {
                        //failed
                        okOrNot = "FAILED";
                    }
                    socket.BeginReceive(ChangeCellCallback, okOrNot);
                }
                else if (spaceFirstWord.Equals("message"))
                {
                    // message
                }
                else if (spaceFirstWord.Equals("Version"))
                {
                    //version
                }
                else if (colonFirstWord.Equals("Name"))
                {
                    // get name
                }
                else if (colonFirstWord.Equals("Password"))
                {
                    // get password
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
                string[] splitup = message.Split(' ');
                string firstWord = splitup[0].ToUpper().Trim();


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
            socket.BeginReceive(CreateSSCallback, socket);
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
            socket.BeginReceive(JoinSSCallback, socket);
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
            socket.BeginReceive(ChangeCellCallback, socket);
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
            socket.BeginReceive(UndoCallback, socket);
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
            socket.BeginReceive(UndoCallback, socket);
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

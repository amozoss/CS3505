using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomNetworking;
using System.Net.Sockets;
using System.Diagnostics;

namespace SS
{
   

    class ClientSocketStuff
    {
        public struct Payload
        {
            public Payload(int num, bool passed)
            {
                number = num;
                valid = passed;
            }

            public Boolean valid;
            public int number;
        }

        enum UndoSpecialStatus{WAIT=100, END=200}

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
            socket.BeginReceive(MasterCallback, null);


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
        /// UNDO SP WAIT LF
        /// UNDO SP END LF
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
        private void MasterCallback(String message, Exception e, object o)
        {
            if (message != null)
            {

                string[] spaceSplit = message.Split(' ');
                string firstWord = "";
                Payload payload = new Payload(0, false);
                if (spaceSplit.Length > 0)
                    firstWord = spaceSplit[0].ToUpper().Trim();

                // Check the status
                string thirdWord = "";
                if (spaceSplit.Length > 2)
                    thirdWord = spaceSplit[2].ToUpper().Trim();

                if (thirdWord.Equals("OK") && !firstWord.Equals("UPDATE"))
                {
                    //passed
                    payload = new Payload(1, true);
                }
                else if (thirdWord.Equals("FAIL") && !firstWord.Equals("UPDATE"))
                {
                    //failed
                    payload = new Payload(1, false);
                }
                else if (!firstWord.Equals("UPDATE"))
                {
                    // there was an error
                    socket.BeginReceive(MasterCallback, payload);
                }

                switch (firstWord)
                {
                    case "CREATE": socket.BeginReceive(CreateSSCallback, payload);
                        break;
                    case "JOIN": socket.BeginReceive(JoinSSCallback, payload);
                        Debug.WriteLine("Join Response Recognized");
                        break;
                    case "CHANGE": socket.BeginReceive(ChangeCellCallback, payload);
                        break;
                    case "UNDO": socket.BeginReceive(UndoCallback, payload);
                        break;
                    case "SAVE": socket.BeginReceive(SaveCallback, payload);
                        break;
                    case "UPDATE": socket.BeginReceive(UpdateCallback, payload);
                        break;
                    default: socket.BeginReceive(MasterCallback, payload);
                        break;
                }
            }
        }



        /// <summary>
        /// If the server successfully created the new spreadsheet file, it should respond with
        ///
        ///CREATE SP OK LF
        ///Name:name LF; true 1
        ///Password:password LF; true 2
        ///
        ///Otherwise, it should respond with
        ///
        ///CREATE SP FAIL LF
        ///Name:name LF; false 1
        ///message LF; false 2
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void CreateSSCallback(String message, Exception e, object payload)
        {
            if (message != null)
            {
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string colonFirstWord = "";
                Payload load = new Payload(0, false);
                if (payload is Payload)
                    load = (Payload)payload;

                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                         socket.BeginReceive(CreateSSCallback, new Payload(1, true));
                    }
                    else if (colonFirstWord.Equals("PASSWORD") && load.number == 2)
                    {
                         // get password
                        socket.BeginReceive(MasterCallback, null);
                    }
                }
                else if (!load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        socket.BeginReceive(CreateSSCallback, new Payload(2, false));
                    }
                    else if (load.number == 2)
                    {
                        // must be a message
                        socket.BeginReceive(MasterCallback, null);
                    }
                }
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
        ///xml LF; true 4
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
            Debug.WriteLine("Join Callback entered");
            string[] colonSplit = message.Split(':');
            string colonFirstWord = "";


            Payload load = new Payload(0, false);
            if (payload is Payload)
            {
                load = (Payload)payload;
            }

            if (colonSplit.Length > 0)
                colonFirstWord = colonSplit[0].ToUpper().Trim();

            if (load.valid) // status == true
            {
                if (colonFirstWord.Equals("NAME") && load.number == 1)
                {
                    // get name
                    socket.BeginReceive(JoinSSCallback, new Payload(2, true));
                    Debug.WriteLine("Join Name Response Recognized");

                }
                else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                {
                    // get Version
                    // @todo check array length before access or make some sort of helper method
                    

                    version = Int32.Parse(colonSplit[1].Trim());
                    socket.BeginReceive(JoinSSCallback, new Payload(3, true));
                    Debug.WriteLine("Join Version Response Recognized");

                }
                else if (colonFirstWord.Equals("LENGTH") && load.number == 3)
                {
                    // get length
                    socket.BeginReceive(JoinSSCallback, new Payload(4, true));
                    Debug.WriteLine("Join Length Response Recognized");

                }
                else if(load.number == 4)
                {
                    // must be the xml
                    socket.BeginReceive(MasterCallback, null);
                    Debug.WriteLine("Join xml Response Recognized");

                }
            }
            else if (!load.valid) // status == false
            {
                if (colonFirstWord.Equals("NAME") && load.number == 1)
                {
                    // get name
                    socket.BeginReceive(JoinSSCallback, new Payload(2, false));
                }
                else if(load.number == 2)
                {
                    // must be a message
                    socket.BeginReceive(MasterCallback, null);
                }
            }
        }

        /// <summary>
        /// If the request succeeded, the server should respond with
        ///
        ///CHANGE SP OK LF
        ///Name:name LF; true 1
        ///Version:version LF true 2
        ///
        ///Otherwise, it should respond with
        ///
        ///CHANGE SP FAIL LF
        ///Name:name LF; true 1
        ///Version:version LF; true 2
        ///message LF; true 3
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="Payload"></param>
        private void ChangeCellCallback(String message, Exception e, object payload)
        {
            if (message != null)
            {
                string[] colonSplitup = message.Split(':');
                string colonFirstWord = "";
                Payload load = new Payload();
                if (payload is Payload)
                {
                    load = (Payload)payload;
                }

                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (load.valid) // load.valid == true
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        socket.BeginReceive(ChangeCellCallback, new Payload(2, true));
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        socket.BeginReceive(MasterCallback, null);
                    }
                }



                else if (!load.valid)
                {
                    if(colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        socket.BeginReceive(ChangeCellCallback, new Payload(2, false));
                    }
                    else if(colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        socket.BeginReceive(ChangeCellCallback, new Payload(3, false));
                    }
                    else if (load.number == 3)
                    {
                        socket.BeginReceive(MasterCallback, null);
                    }
                }
            }
        }

        /// <summary>
        /// If the request succeeded, the server should respond with
        ///
        ///UNDO SP OK LF
        ///Name:name LF; true 1;
        ///Version:version LF; true 2
        ///Cell:cell LF; true 3
        ///Length:length LF; true 4
        ///content LF; true 5
        ///
        ///Otherwise, the server should respond with
        ///
        ///UNDO SP FAIL LF
        ///Name:name LF; false 1
        ///message LF; false 2
        /// 
        /// 
        /// If there are no unsaved changes, the server should respond with
        /// 
        ///UNDO SP END LF
        ///Name:name LF
        ///Version:version LF
        ///
        ///If the client’s version is out of date, the server should respond with 
        ///
        ///UNDO SP WAIT LF
        ///Name:name LF
        ///Version:version LF
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void UndoCallback(String message, Exception e, object payload)
        {
            if (message != null)
            {
                string[] spaceSplitup = message.Split(' ');
                string[] colonSplitup = message.Split(':');
                string colonFirstWord = "";
                string status = "";
                Payload load = new Payload(0, false);
                if (payload is Payload)
                {
                    load = (Payload)payload;
                }


                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                ///Name:name LF; true 1;
                ///Version:version LF; true 2
                ///Cell:cell LF; true 3
                ///Length:length LF; true 4
                ///content LF; true 5
                if (load.valid) 
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        socket.BeginReceive(UndoCallback, new Payload(2, true));
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        // get Version
                        version = Int32.Parse(colonSplitup[1].Trim());
                        socket.BeginReceive(UndoCallback, new Payload(3, true));
                    }
                    else if (colonFirstWord.Equals("CELL") && load.number == 3)
                    {
                        // get cell
                        socket.BeginReceive(UndoCallback, new Payload(4, true));
                    }
                    else if (colonFirstWord.Equals("LENGTH") && load.number == 4)
                    {
                        // get length
                        socket.BeginReceive(UndoCallback, new Payload(5, true));
                    }
                    else if(load.number == 5)
                    {
                        // must be the content
                        socket.BeginReceive(MasterCallback, null);
                    }
                }

                else if (!load.valid)
                {
                    switch (load.number)
                    {
                        case 1:                                         // It is FAIL's name.
                            socket.BeginReceive(UndoCallback, new Payload(2, false));
                            break;
                        case 200:                                       // It is END's name.
                            socket.BeginReceive(UndoCallback, new Payload((int)UndoSpecialStatus.END + 1, false));
                            break;
                        case 100:                                       // It is WAIT's name.
                            socket.BeginReceive(UndoCallback, new Payload((int)UndoSpecialStatus.WAIT + 1, false));
                            break;
                        case 2:                                         // It is FAIL's message.
                            socket.BeginReceive(MasterCallback, null);
                            break;
                        case 201:                                       // It is WAIT's Version.

                        case 101:                                       // It is END's Version.
                            socket.BeginReceive(MasterCallback, null);
                            break;
                        default:
                            socket.BeginReceive(MasterCallback, null);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// If the request succeeds, the server should respond with
        ///SAVE SP OK LF
        ///Name:name LF; true 1
        ///
        ///If the request fails, the server should respond with
        ///
        ///SAVE SP FAIL LF
        ///Name:name LF; true 1
        ///message LF; true 2
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void SaveCallback(String message, Exception e, object payload)
        {
            if (message != null)
            {
                string[] colonSplitup = message.Split(':');
                string colonFirstWord = "";
                Payload load = new Payload(0, false);
                if (payload is Payload)
                {
                    load = (Payload)payload;
                }

                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();


                if (load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)

                    {
                        // get name
                        socket.BeginReceive(MasterCallback, null);
                    }
                }
                else if (!load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        socket.BeginReceive(SaveCallback, new Payload(2, false));
                    }
                    else if(load.number == 2)
                    {
                        // must be a message
                        socket.BeginReceive(MasterCallback, null);
                    }
                }
            }
        }

        /// <summary>
        /// To communicate a committed change to other clients, the server should send
        ///UPDATE LF
        ///Name:name LF; true 1
        ///Version:version LF; true 2
        ///Cell:cell LF; true 3
        ///Length:length LF; true 4
        ///content LF; true 5
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void UpdateCallback(String message, Exception e, object payload)
        {
            if (message != null)
            {
                string[] colonSplitup = message.Split(':');
                string colonFirstWord = "";
                string status = "";
                Payload load = new Payload();
                if (payload is Payload)
                {
                    load = (Payload)payload;
                }

                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        socket.BeginReceive(UpdateCallback, new Payload(2, true));
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        // get Version
                        version = Int32.Parse(colonSplitup[1].Trim());
                        socket.BeginReceive(UpdateCallback, new Payload(2, true));
                    }
                    else if (colonFirstWord.Equals("CELL") && load.number == 3)
                    {
                        // get cell
                        socket.BeginReceive(UpdateCallback, new Payload(2, true));
                    }
                    else if (colonFirstWord.Equals("LENGTH") && load.number == 4)
                    {
                        // get length
                        socket.BeginReceive(UpdateCallback, new Payload(2, true));
                    }
                    else if(load.number == 5)
                    {
                        // must be the content
                        socket.BeginReceive(MasterCallback, null);
                    }
                }
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
            socket.BeginSend("CREATE\n" + "Name:" + name + "\n" + "Password:" + password + "\n", SendCallback, socket);
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
            socket.BeginSend("JOIN\n" + "Name:" + name + "\n" + "Password:" + password + "\n", SendCallback, socket);
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
            socket.BeginSend("CHANGE\n" +  "Name:" + nameOfSpreadsheet + "\n" + "Version:" + version.ToString() + "\n"
                + "Cell:" + cellName + "\n" + "Length:" + cellContent.Length.ToString() + "\n" + cellContent + "\n", SendCallback, socket);
            //socket.BeginReceive(MasterCallback, "NOTHING");
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
            socket.BeginSend("UNDO\n" + "Name:" + nameOfSpreadsheet + "\n" + "Version:" + version.ToString()+ "\n", SendCallback, socket);
            //socket.BeginReceive(MasterCallback, "NOTHING");
        }
        /// <summary>
        /// To save the current state of the spreadsheet and merge all outstanding changes to the existing 
        /// file, the client should send to the server
        /// SAVE LF
        /// Name:name LF
        ///
        /// If the request succeeds, the server should respond with 
        /// SAVE SP OK LF
        /// Name:name LF
        ///
        /// If the request fails, the server should respond with
        /// SAVE SP FAIL LF
        /// Name:name LF
        /// message LF
        ///
        /// where
        ///  name is the name of the spreadsheet for which saving failed
        ///  message contains information why the save failed. It must not contain any linefeeds. 
        /// The server should provide some reason in message why the request failed (e.g., the 
        /// client has not logged in to work on the spreadsheet).
        /// </summary>
        public void Save()
        {
            socket.BeginSend("SAVE\n" + "Name:" + nameOfSpreadsheet + "\n", SendCallback, socket);
            //socket.BeginReceive(MasterCallback, "NOTHING");
        }

        /// <summary>
        /// To leave a spreadsheet, the client should send
        /// LEAVE LF
        /// Name:name LF
        /// </summary>
        public void Leave()
        {
            socket.BeginSend("LEAVE\n" + "Name:" + nameOfSpreadsheet + "\n", SendCallback, null);
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

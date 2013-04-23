﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomNetworking;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SS;

namespace Client
{


    public class ClientSocket
    {
        /// <summary>
        /// The payload that is used by most callbacks in the client
        /// </summary>
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

        /// <summary>
        /// The payload used by ChangeCellCallback, because the state is more complex when keeping track of wait messages as well.
        /// When a wait message is sent we can just store the cell name and contents in a local callback for later use.
        /// </summary>
        public struct ChangePayload
        {
            public string cell;
            public string contents;
            public int number;
            public Boolean valid;
            public ChangeStatus availability;
        }

        /// <summary>
        /// The payload used by UndoCallback, the state is more complex when keeping track of wait messages.  Similar in function to
        /// the changeCalback.
        /// </summary>
        public struct UndoPayload
        {
            public Boolean valid;
            public int number;
            public string name;
            public int version;
            public string cell;
            public int contentLength;
            public string contents;
        }

        /// <summary>
        /// Used only with UpdateCallback; similar in function to UndoPayload.
        /// </summary>
        public struct UpdatePayload
        {
            public Boolean valid;
            public int number;
            public string name;
            public int version;
            public string cell;
            public int contentLength;
            public string contents;
        }

        enum SpecialStatus { UNDO_WAIT = 100, UNDO_END = 200, CHANGE_WAIT = 300 }   // This enum allows extra state to be tracked in the undo and change payloads.
        public enum ChangeStatus { CANSEND, WAITING_TO_SEND, CANT_SEND }            // This enum allows for 3 separate states to be maintained in the ChangeCallback;
                                                                                    // when a change cannot be made, when a change can be made, and when a change must wait for
                                                                                    // the version of the client to be updated.

        public delegate void EditCell(String name, String contents);        // 
        public delegate void SendXML(String name);                          // 
        public delegate void ClientToGUI_SS(String message, bool isError);  // The message will be handled by a separate class 

        private string ipAddress;                                           // The IP address given by the client.
        private string nameOfSpreadsheet;                                   // name is the name for the new spreadsheet
        private string password;                                            // password is the password to use for the new spreadsheet
        private SSOffical socket;                                           // The wrapper for the socket, handles strings terminated by linefeed, or \n.
        private static int SERVERPORT = 1984;                               // The default port for the server, although we never use this.
        private ClientToGUI_SS clientGUI_SS;                                
        private int version;                                                // The version the client-side is. The spreadsheet sends updates to correct when the version is wrong.
        private ChangePayload changePayload;                                // This struct is used to store data when we call beginreceive, helping keep track of state between callbacks.
        private EditCell e_Cell;                                            // This method is called when edits to the spreadsheet need to be made.
        private SendXML send_XML;                                           // XML from the server is sent here.



        /// <summary>
        ///  Creates the communication outlet that the client will use to "talk" to the server.
        /// </summary>
        /// <param name="ipAddress"></param>
        public ClientSocket(string ipAddress, EditCell spreadsheet, ClientToGUI_SS receivedMessage, int port, SendXML xmlSender)
        {
            changePayload.contents = "";
            try
            {
                // set private instance variables 
                this.ipAddress = ipAddress;
                this.clientGUI_SS = receivedMessage;
                this.e_Cell = spreadsheet;
                this.send_XML = xmlSender;

                TcpClient client = new TcpClient(ipAddress, port);
                Socket sock = client.Client;

                socket = new SSOffical(sock, new UTF8Encoding());
                socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                version = 0;
                password = "";
                nameOfSpreadsheet = "";
            }
            catch (Exception e)
            {
                clientGUI_SS("Server connection aborted, \n" + e.Message, true);
            }
        }

        /// <summary>
        /// This function is used when sending data, we didn't have any use for it.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
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
                UpdatePayload upPay = new UpdatePayload();
                UndoPayload undoPay = new UndoPayload();

                if (spaceSplit.Length > 0)
                    firstWord = spaceSplit[0].ToUpper().Trim();

                // Check the status
                string secondWord = "";
                if (spaceSplit.Length > 1)
                    secondWord = spaceSplit[1].ToUpper().Trim();

                // Deal with Each status
                if (secondWord.Equals("OK"))
                {
                    //passed
                    payload = new Payload(1, true);
                    if (firstWord.Equals("CHANGE"))
                    {
                        changePayload.valid = true;
                        changePayload.number = 1;
                    }
                    else if (firstWord.Equals("UNDO"))
                    {
                        undoPay.valid = true;
                        undoPay.number = 1;
                    }
                }
                else if (secondWord.Equals("FAIL"))
                {
                    //failed
                    payload = new Payload(1, false);
                    if (firstWord.Equals("CHANGE"))
                    {
                        changePayload.valid = false;
                        changePayload.number = 1;
                    }
                    else if (firstWord.Equals("UNDO"))
                    {
                        undoPay.valid = false;
                        undoPay.number = 1;
                    }
                }
                else if (secondWord.Equals("WAIT"))
                {
                    if (firstWord.Equals("UNDO"))
                    {
                        //Undo wait message

                        undoPay.valid = false;
                        undoPay.number = (int)SpecialStatus.UNDO_WAIT;
                    }
                    else if (firstWord.Equals("CHANGE"))
                    {
                        //changes wait message
                        
                        changePayload.valid = false;
                        changePayload.number = (int)SpecialStatus.CHANGE_WAIT;
                        changePayload.availability = ChangeStatus.WAITING_TO_SEND;
                    }
                }
                else if (firstWord.Equals("UPDATE"))
                {
                    upPay.number = 1;
                    upPay.valid = true;
                }
                else if (secondWord.Equals("END"))
                {
                    //Undo end message
                    undoPay.valid = false;
                    undoPay.number = (int)SpecialStatus.UNDO_END;
                }
                else if (!firstWord.Equals("UPDATE"))
                {
                    // there was an error
                    // @todo handle error
                    socket.BeginReceive(MasterCallback, payload, CustomNetworking.callbacks.MASTER);
                }

                switch (firstWord)
                {
                    case "CREATE": socket.BeginReceive(CreateSSCallback, payload, CustomNetworking.callbacks.CREATE);
                        Debug.WriteLine("Create Response Recognized");
                        break;
                    case "JOIN": socket.BeginReceive(JoinSSCallback, payload, CustomNetworking.callbacks.JOIN);
                        Debug.WriteLine("Join Response Recognized");
                        break;
                    case "CHANGE": socket.BeginReceive(ChangeCellCallback, changePayload, CustomNetworking.callbacks.CHANGE);
                        Debug.WriteLine("Change Response Recognized");
                        break;
                    case "UNDO": socket.BeginReceive(UndoCallback, undoPay, CustomNetworking.callbacks.UNDO);
                        Debug.WriteLine("Undo Response Recognized");
                        break;
                    case "SAVE": socket.BeginReceive(SaveCallback, payload, CustomNetworking.callbacks.SAVE);
                        Debug.WriteLine("Save Response Recognized");
                        break;
                    case "UPDATE": socket.BeginReceive(UpdateCallback, upPay, CustomNetworking.callbacks.UPDATE);
                        Debug.WriteLine("Update Response Recognized");
                        break;
                    default: socket.BeginReceive(MasterCallback, payload, CustomNetworking.callbacks.MASTER); // If all else fails just call the master
                        Debug.WriteLine("Mastercallback: unrecognized command {0}", message);
                        resetChangePayload();
                        //clientGUI_SS(message, true);
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
                        socket.BeginReceive(CreateSSCallback, new Payload(2, true), CustomNetworking.callbacks.CREATE);
                        // Store the name of the SS
                        nameOfSpreadsheet = getSecondWord(colonSplitup);
                        Debug.WriteLine("Create Name Response Recognized");
                    }
                    else if (colonFirstWord.Equals("PASSWORD") && load.number == 2)
                    {
                        // get password
                        password = getSecondWord(colonSplitup);
                        this.JoinSpreadsheet(nameOfSpreadsheet, password); // Just for fun, and the protocol. After a spreadsheet has been created, we join it.
                        Debug.WriteLine("Create password Response Recognized");
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                    else
                    {
                        // something went wrong 
                        // @todo handle error
                        Debug.WriteLine("Create valid commands corrupted {0}", message);
                        resetChangePayload();

                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                }
                else if (!load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        Debug.WriteLine("Create fail Name Response Recognized");
                        socket.BeginReceive(CreateSSCallback, new Payload(2, false), CustomNetworking.callbacks.CREATE);
                    }
                    else if (load.number == 2)
                    {
                        // must be a message
                        Debug.WriteLine("Create fail message Response Recognized");
                        clientGUI_SS(message, true);
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                    else
                    {
                        // something went wrong 
                        // @todo handle error
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                        resetChangePayload();

                        Debug.WriteLine("Create fail commands corrupted {0}", message);

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
                    nameOfSpreadsheet = getSecondWord(colonSplit);
                    socket.BeginReceive(JoinSSCallback, new Payload(2, true), CustomNetworking.callbacks.JOIN);
                    Debug.WriteLine("Join Name Response Recognized");

                }
                else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                {
                    // get Version
                    updateVersion(getSecondWord(colonSplit));
                    socket.BeginReceive(JoinSSCallback, new Payload(3, true), CustomNetworking.callbacks.JOIN);
                    Debug.WriteLine("Join Version Response Recognized");

                }
                else if (colonFirstWord.Equals("LENGTH") && load.number == 3)
                {
                    // get length
                    socket.BeginReceive(JoinSSCallback, new Payload(4, true), CustomNetworking.callbacks.JOIN);
                    Debug.WriteLine("Join Length Response Recognized");
                }
                else if (load.number == 4)
                {
                    // must be the xml
                    socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    send_XML(message);
                    clientGUI_SS("Updateness!", false);
                    Debug.WriteLine("Join xml Response Recognized");
                }
                else
                {
                    // something went wrong 
                    // @todo handle error
                    socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    Debug.WriteLine("Join succeed commands corrupted {0}", message);

                }
            }
            else if (!load.valid) // status == false
            {
                if (colonFirstWord.Equals("NAME") && load.number == 1)
                {
                    // get name
                    Debug.WriteLine("Join fail Name Response Recognized");
                    socket.BeginReceive(JoinSSCallback, new Payload(2, false), CustomNetworking.callbacks.JOIN);
                }
                else if (load.number == 2)
                {
                    // must be a message
                    Debug.WriteLine("Join fail message Response Recognized");
                    clientGUI_SS(message, true);
                    socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                }
                else
                {
                    // something went wrong 
                    // @todo handle error
                    socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    Debug.WriteLine("Join fail commands corrupted {0}", message);

                }
            }
        }

        /// <summary>
        /// If the request succeeded, the server should respond with
        ///
        ///CHANGE OK \N
        ///Name:name LF; true 1
        ///Version:version LF true 2
        ///
        /// /CHANGE SP WAIT LF
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
                ChangePayload load = new ChangePayload();
                if (payload is ChangePayload)
                {
                    load = (ChangePayload)payload;
                }

                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (load.availability == ChangeStatus.CANT_SEND && load.number <= (int)SpecialStatus.CHANGE_WAIT)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        load.number++;
                        socket.BeginReceive(ChangeCellCallback, load, CustomNetworking.callbacks.CHANGE);
                        Debug.WriteLine("Change Name Response Recognized");
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        //spreadsheet.SetContentsOfCell
                        updateVersion(getSecondWord(colonSplitup));
                        e_Cell(load.cell, load.contents);
                        clientGUI_SS("YAY", false);
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                        Debug.WriteLine("Change Version Response Recognized");
                        resetChangePayload();
                    }
                    else
                    {
                        // something went wrong 
                        // @todo handle error
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                        resetChangePayload();
                        Debug.WriteLine("Change unrecognized command valid {0}", message);

                    }
                }

                else if (load.availability == ChangeStatus.WAITING_TO_SEND || load.availability == ChangeStatus.CANT_SEND)
                {
                    // wait status
                    if (colonFirstWord.Equals("NAME") && load.number == (int)SpecialStatus.CHANGE_WAIT)
                    {
                        Debug.WriteLine("Change fail wait name Response Recognized");
                        load.number++;
                        socket.BeginReceive(ChangeCellCallback, load, CustomNetworking.callbacks.CHANGE);
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == (int)SpecialStatus.CHANGE_WAIT + 1)
                    {
                        Debug.WriteLine("Change fail wait version Response Recognized");
                        changePayload.availability = ChangeStatus.WAITING_TO_SEND;
                        socket.BeginReceive(MasterCallback, load, CustomNetworking.callbacks.MASTER);
                    }
                    // fail status
                    else if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        Debug.WriteLine("Change fail name Response Recognized");
                        load.number++;
                        socket.BeginReceive(ChangeCellCallback, load, CustomNetworking.callbacks.CHANGE);
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        Debug.WriteLine("Change fail version Response Recognized");
                        load.number++;
                        // @TODO: Maybe do versioning stuff.
                        socket.BeginReceive(ChangeCellCallback, load, CustomNetworking.callbacks.CHANGE);
                    }
                    else if (load.number == 3)
                    {
                        Debug.WriteLine("Change fail message Response Recognized");
                        resetChangePayload();
                        clientGUI_SS(message, true);
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                    else
                    {
                        // something went wrong 
                        // @todo handle error
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                        Debug.WriteLine("Change fail commands corrupted {0}", message);

                        resetChangePayload();


                    }
                }
            }
        }

        /// <summary>
        /// The spreadsheet is eligible to make changes.
        /// </summary>
        private void resetChangePayload()
        {
            changePayload.availability = ChangeStatus.CANSEND;
            changePayload.cell = "";
            changePayload.contents = "";
        }

        /// <summary>
        /// This method is used to update the version of the client-side to the latest version.
        /// </summary>
        /// <param name="newV"></param>
        private void updateVersion(string newV)
        {
            int v;
            Int32.TryParse(newV, out v);
            version = v;
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
                UndoPayload load = new UndoPayload();
                if (payload is UndoPayload)
                {
                    load = (UndoPayload)payload;
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
                        Debug.WriteLine("Undo name Response Recognized");
                        load.number = 2;
                        socket.BeginReceive(UndoCallback, load, CustomNetworking.callbacks.UNDO);
                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        // get Version
                        Debug.WriteLine("Undo version Response Recognized");
                        updateVersion(getSecondWord(colonSplitup));
                        load.number = 3;
                        socket.BeginReceive(UndoCallback, load, CustomNetworking.callbacks.UNDO);
                    }
                    else if (colonFirstWord.Equals("CELL") && load.number == 3)
                    {
                        // get cell
                        Debug.WriteLine("Undo cell Response Recognized");
                        load.cell = getSecondWord(colonSplitup);
                        load.number = 4;
                        socket.BeginReceive(UndoCallback, load, CustomNetworking.callbacks.UNDO);
                    }
                    else if (colonFirstWord.Equals("LENGTH") && load.number == 4)
                    {
                        // get length
                        Debug.WriteLine("Undo length Response Recognized");
                        load.number = 5;
                        int lNum = 0;
                        Int32.TryParse(getSecondWord(colonSplitup), out lNum);
                        load.contentLength = lNum;
                        socket.BeginReceive(UndoCallback, load, CustomNetworking.callbacks.UNDO);
                    }
                    else if (load.number == 5)
                    {
                        // must be the content
                        Debug.WriteLine("Undo content Response Recognized");
                        e_Cell(load.cell, message);
                        clientGUI_SS("random", false);
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                    else
                    {
                        // something went wrong 
                        // @todo handle error
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                        Debug.WriteLine("Undo unrecognized commands valid {0}", message);

                    }
                }

                else if (!load.valid)
                {
                    switch (load.number)
                    {
                        case 1:                                         // It is FAIL's name.
                            socket.BeginReceive(UndoCallback, new Payload(2, false), CustomNetworking.callbacks.UNDO);
                            Debug.WriteLine("Undo fail name Response Recognized");

                            break;
                        case (int)SpecialStatus.UNDO_END:                                       // It is END's name.
                            socket.BeginReceive(UndoCallback, new Payload((int)SpecialStatus.UNDO_END + 1, false), CustomNetworking.callbacks.UNDO);
                            Debug.WriteLine("Undo end name Response Recognized");

                            break;
                        case (int)SpecialStatus.UNDO_WAIT:                                       // It is WAIT's name.
                            socket.BeginReceive(UndoCallback, new Payload((int)SpecialStatus.UNDO_WAIT + 1, false), CustomNetworking.callbacks.UNDO);
                            Debug.WriteLine("Undo wait name Response Recognized");
                            break;
                        case 2:                                         // It is FAIL's message.
                            socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                            Debug.WriteLine("Undo fail message Response Recognized");
                            clientGUI_SS(message, true);
                            break;
                        case 201:                                       // It is WAIT's Version.
                            socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                            Debug.WriteLine("End version Response Recognized");
                            break;
                        case 101:                                       // It is END's Version.
                            socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                            Debug.WriteLine("Wait version Response Recognized");
                            clientGUI_SS("YOu can no longer undo!", true);
                            break;
                        default:
                            // something went wrong 
                            // @todo handle error
                            Debug.WriteLine("Undo fail unrecognized command", message);

                            socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
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
                        Debug.WriteLine("Save name Response Recognized");
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                    else
                    {
                        // something went wrong 
                        Debug.WriteLine("Save unrecognized command {0}", message);

                        // @todo handle error
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                }
                else if (!load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        Debug.WriteLine("Save fail name Response Recognized");
                        socket.BeginReceive(SaveCallback, new Payload(2, false), CustomNetworking.callbacks.SAVE);
                    }
                    else if (load.number == 2)
                    {
                        // must be a error message
                        Debug.WriteLine("Save fail message Response Recognized");
                        clientGUI_SS(message, true);
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                    else
                    {
                        // something went wrong 
                        Debug.WriteLine("Save fail unrecognized {0}", message);

                        // @todo handle error
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                    }
                }
            }
        }

        private string getSecondWord(string[] colonSplit)
        {
            string second = "";
            if (colonSplit.Length > 1)
                second = colonSplit[1].Trim();
            else
            {
                // @todo some error
            }
            return second;
        }

        /// <summary>
        /// This method is sent the updated version.
        /// </summary>
        /// <param name="v"></param>
        private void VersionString(string v)
        {

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
                UpdatePayload load = new UpdatePayload();
                if (payload is UpdatePayload)
                {
                    load = (UpdatePayload)payload;
                }

                if (colonSplitup.Length > 0)
                    colonFirstWord = colonSplitup[0].ToUpper().Trim();

                if (load.valid)
                {
                    if (colonFirstWord.Equals("NAME") && load.number == 1)
                    {
                        // get name
                        load.number++;
                        socket.BeginReceive(UpdateCallback, load, CustomNetworking.callbacks.UPDATE);
                        Debug.WriteLine("Update name Response Recognized");

                    }
                    else if (colonFirstWord.Equals("VERSION") && load.number == 2)
                    {
                        // get Version
                        Debug.WriteLine("Update version Response Recognized", message);

                        load.number++;
                        updateVersion(getSecondWord(colonSplitup));
                        socket.BeginReceive(UpdateCallback, load, CustomNetworking.callbacks.UPDATE);
                    }
                    else if (colonFirstWord.Equals("CELL") && load.number == 3)
                    {
                        // get cell
                        Debug.WriteLine("Update cell Response Recognized");
                        load.number++;
                        load.cell = getSecondWord(colonSplitup);

                        socket.BeginReceive(UpdateCallback, load, CustomNetworking.callbacks.UPDATE);
                    }
                    else if (colonFirstWord.Equals("LENGTH") && load.number == 4)
                    {
                        // get length
                        load.number++;
                        int length;
                        Int32.TryParse(getSecondWord(colonSplitup), out length);
                        load.contentLength = length;
                        Debug.WriteLine("Update length Response Recognized");
                        socket.BeginReceive(UpdateCallback, load, CustomNetworking.callbacks.UPDATE);
                    }
                    else if (load.number == 5)
                    {
                        // must be the content
                        load.contents = message;
                        Debug.WriteLine("Update content Response Recognized");

                        // We need to lock on this, right?
                        e_Cell(load.cell, message.Trim());
                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
                        clientGUI_SS("update!", false);

                        if (changePayload.availability == ChangeStatus.WAITING_TO_SEND)
                        {
                            changePayload.availability = ChangeStatus.CANSEND;
                            Debug.WriteLine("Waiting_to_send message being sent {0}", message);

                            ChangeCell(changePayload.cell, changePayload.contents);

                        }
                    }
                    else
                    {
                        // something went wrong
                        // @todo handle error, send error message to gui about bug report or something
                        Debug.WriteLine("Update fail unrecognized command {0}", message);
                        resetChangePayload();

                        socket.BeginReceive(MasterCallback, null, CustomNetworking.callbacks.MASTER);
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
            if (!ReferenceEquals(socket, null) && socket.isConnected())
                socket.BeginSend("CREATE\n" + "Name:" + name + "\n" + "Password:" + password + "\n", SendCallback, socket);
            else clientGUI_SS("Could not create speadsheet. Connection to server could not be established.", true);
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
            if (!ReferenceEquals(socket, null) && socket.isConnected())
                socket.BeginSend("JOIN\n" + "Name:" + name + "\n" + "Password:" + password + "\n", SendCallback, socket);
            else clientGUI_SS("Could not join spreadsheet. Connection could not be established.", true);
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
            if (!ReferenceEquals(socket, null) && socket.isConnected())
            {

                if (changePayload.availability == ChangeStatus.CANSEND)
                {
                    changePayload.cell = cellName;
                    changePayload.contents = cellContent;
                    string length;
                    if (cellContent.Equals(""))
                        cellContent = " ";

                    changePayload.availability = ChangeStatus.CANT_SEND;
                    
                    string sendString = "CHANGE\n" + "Name:" + nameOfSpreadsheet + "\n" + "Version:" + version.ToString() + "\n"
                        + "Cell:" + cellName + "\n" + "Length:" + cellContent.Length.ToString() + "\n" + cellContent + "\n";
                    Debug.WriteLine(sendString);
                    socket.BeginSend(sendString, SendCallback, socket);
                }

            }
            else
                clientGUI_SS("Connection to server was lost. Cannot make change to spreadsheet.", true);

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
            if (socket.isConnected())
                socket.BeginSend("UNDO\n" + "Name:" + nameOfSpreadsheet + "\n" + "Version:" + version.ToString() + "\n", SendCallback, socket);
            else
                clientGUI_SS("Connection to server was lost. Cannot Undo spreadsheet.", true);

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
            if (socket.isConnected())
                socket.BeginSend("SAVE\n" + "Name:" + nameOfSpreadsheet + "\n", SendCallback, socket);
            else
                clientGUI_SS("Connection to server was lost. Could not save spreadsheet.", true);

        }

        /// <summary>
        /// To leave a spreadsheet, the client should send
        /// LEAVE LF
        /// Name:name LF
        /// </summary>
        public void Leave()
        {
            if (!ReferenceEquals(socket, null) && socket.isConnected())
            {
                socket.BeginSend("LEAVE\n" + "Name:" + nameOfSpreadsheet + "\n", SendCallback, null);
                Thread.Sleep(200);
                socket.CloseAndShutdown();
            }
            //else
                //clientGUI_SS("Connection to server was lost.", true);
        }

        #endregion

    }
}

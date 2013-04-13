using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CustomNetworking
{
    /// <summary> 
    /// A StringSocket is a wrapper around a Socket.  It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchonization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// If we have an open Socket s, we can create a StringSocket by doing
    /// 
    ///    StringSocket ss = new StringSocket(s, new UTF8Encoding());
    /// 
    /// We can write a string to the StringSocket by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameters to the callback are a
    /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
    /// the Exception that caused the send attempt to fail.
    /// 
    /// We can read a string from the StringSocket by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
    /// string or the Exception will be non-null, but nor both.  If the string is non-null, 
    /// it is the requested string (with the newline removed).  If the Exception is non-null, 
    /// it is the Exception that caused the send attempt to fail.
    /// </summary>

    public class StringSocket
    {


        // These delegates describe the callbacks that are used for sending and receiving strings.
        public delegate void SendCallback(Exception e, object payload);
        public delegate void ReceiveCallback(String s, Exception e, object payload);


        // The sending and receiving queue will queue up send and receive requests. Each send request 
        // will be a dictionary in the send queue consisting of the string to be sent, callback method and payload. Each receive request will be 
        // a dictionary in the receive queue consisting of the callback method and payload. 
        // The concurrentQueue is thread safe
        private ConcurrentQueue<Dictionary<String, Object>> sendQueue;
        private ConcurrentQueue<Dictionary<String, Object>> receiveQueue;

        // the send and receive threads have infinate loops that dequeue the sendQueue and receiveQueue one at a time and send/receive strings through the socket
        private Thread sendThread;
        private Thread receiveThread;

        private Socket socket; // keep a reference to the socket for sending and receiving

        String receiveBuffer; // a buffer to keep track of extra data that was received, when there are multiple newline characters in the data that was received

        // Encoding used for incoming/outgoing data
        private static Encoding encoding;

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// LineSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        public StringSocket(Socket s, Encoding e)
        {
            // create the queues
            sendQueue = new ConcurrentQueue<Dictionary<string, object>>();
            receiveQueue = new ConcurrentQueue<Dictionary<string, object>>();

            // create the threads
            sendThread = new Thread(sendingLoop);
            receiveThread = new Thread(receivingLoop);
            sendThread.Start();
            receiveThread.Start();

            // create the receive buffer
            receiveBuffer = "";

            // set the socket and encoding
            socket = s;
            encoding = e;
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket, or failed in the 
        /// attempt, it invokes the callback.  The parameters to the callback are a
        /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
        /// the Exception that caused the send attempt to fail. 
        /// 
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            // create and queue up dictionary with s, callback and payload
            Dictionary<string, object> dict = new Dictionary<string, object>(3);
            dict.Add("message", s);
            dict.Add("callback", callback);
            dict.Add("payload", payload);
            sendQueue.Enqueue(dict);
        }

        private void sendingLoop()
        {
            while (true)
            {
                Dictionary<string, object> dequeuedDict;
                if (sendQueue.TryDequeue(out dequeuedDict))
                {
                    // load stringToSend, callback, and payload
                    object stringToSend, callBack, payload;
                    dequeuedDict.TryGetValue("payload", out payload);
                    dequeuedDict.TryGetValue("callback", out callBack);
                    SendCallback cb = (SendCallback)callBack;

                    if (dequeuedDict.TryGetValue("message", out stringToSend))
                    {
                        // convert to bytes
                        byte[] outgoingBuffer = encoding.GetBytes((string)stringToSend);

                        // use the send method so
                        // only have one Socket.BeginSend or Socket.BeginReceive is active at any time. 
                        socket.Send(outgoingBuffer);

                        // call the callback on a different thread
                        ThreadPool.QueueUserWorkItem(new WaitCallback(state => { cb(null, payload); }));
                    }
                }
                else
                {
                    Thread.Sleep(10); // stop thread from using 100% cpu
                }
            }
        }

        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, or
        /// failed in the attempt, it invokes the callback.  The parameters to the callback are
        /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
        /// string or the Exception will be non-null, but nor both.  If the string is non-null, 
        /// it is the requested string (with the newline removed).  If the Exception is non-null, 
        /// it is the Exception that caused the send attempt to fail.
        /// 
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// 
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload)
        {
            // create new dictionary and store callback and payload in it
            Dictionary<string, object> dict = new Dictionary<string, object>(2);
            dict.Add("callback", callback);
            dict.Add("payload", payload);
            receiveQueue.Enqueue(dict);
        }

        private void receivingLoop()
        {
            while (true)
            {
                Dictionary<string, object> dequeuedDict;
                // Ask the socket to call MessageReceive as soon as up to 1024 bytes arrive.
                byte[] buffer = new byte[1024];
               if (receiveQueue.Count > 0)
                {
                    // get the length of string that was sent and use the receive method so
                    // only have one Socket.BeginSend or Socket.BeginReceive is active at any time. 
                    var length = socket.Receive(buffer);

                    // store in receive buffer
                    receiveBuffer += encoding.GetString(buffer, 0, length);

                    int newlineIndex;

                    // continue while loop as long as a newline character is in the receive buffer. 
                    while ((newlineIndex = receiveBuffer.IndexOf('\n')) != -1)
                    {
                        // get the string up to the first new line
                        string msg = receiveBuffer.Substring(0, newlineIndex);
                        // subtract out the msg message
                        receiveBuffer = receiveBuffer.Substring(newlineIndex + 1);

                        // if no pending callbacks
                        if (!receiveQueue.TryDequeue(out dequeuedDict)) continue;

                        object callBack, payload;
                        // load callback and payload
                        dequeuedDict.TryGetValue("payload", out payload);
                        dequeuedDict.TryGetValue("callback", out callBack);

                        // call callback on a different thread
                        ReceiveCallback cb = (ReceiveCallback)callBack;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(state => { cb(msg, null, payload); }));


                    }
                }
               // Thread.Sleep(10); // stop thread from using 100% cpu
            }
        }
    }
}





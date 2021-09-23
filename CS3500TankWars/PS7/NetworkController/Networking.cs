// Luke Ludlow, Ryan Dalby
// CS 3500
// 2019 Fall 

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security;
using System.Threading;

using static NetworkUtil.NetworkingHelper;

namespace NetworkUtil
{
    /// <summary>
    /// a general purpose asynchronous networking library that supports any text-based communication. 
    /// this library is essentially an easy to use wrapper over the C# networking API.
    /// it will be used to implement the upcoming game client and server (PS8 and PS9).
    /// </summary>
    public static class Networking
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        // Server-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Starts a TcpListener on the specified port and starts an event-loop to accept new clients.
        /// The event-loop is started with BeginAcceptSocket and uses AcceptNewClient as the callback.
        /// AcceptNewClient will continue the event-loop.
        /// </summary>
        /// <param name="toCall">The method to call when a new connection is made</param>
        /// <param name="port">The port to listen on</param>
        public static TcpListener StartServer(Action<SocketState> toCall, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptSocket(AcceptNewClient, (toCall, listener));
            return listener;
        }

        /// <summary>
        /// To be used as the callback for accepting a new client that was initiated by StartServer, and 
        /// continues an event-loop to accept additional clients.
        ///
        /// Uses EndAcceptSocket to finalize the connection and create a new SocketState. The SocketState's
        /// OnNetworkAction should be set to the delegate that was passed to StartServer.
        /// 
        /// Then invokes the OnNetworkAction delegate with the new SocketState so the user can take action. 
        /// 
        /// If anything goes wrong during the connection process (such as the server being stopped externally), 
        /// the OnNetworkAction delegate should be invoked with a new SocketState with its ErrorOccured flag set to true 
        /// and an appropriate message placed in its ErrorMessage field. The event-loop should not continue if
        /// an error occurs.
        ///
        /// If an error does not occur, after invoking OnNetworkAction with the new SocketState, an event-loop to accept 
        /// new clients should be continued by calling BeginAcceptSocket again with this method as the callback.
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginAcceptSocket. It must contain a tuple with 
        /// 1) a delegate so the user can take action (a SocketState Action), and 2) the TcpListener</param>
        private static void AcceptNewClient(IAsyncResult ar)
        {
            ValueTuple<Action<SocketState>, TcpListener> asyncState = (ValueTuple<Action<SocketState>, TcpListener>)ar.AsyncState;
            Action<SocketState> toCall = asyncState.Item1;
            TcpListener listener = asyncState.Item2;

            TryEndAcceptSocket(ar, listener, toCall);
        }

        /// <summary>
        /// Helper method that attempts to create a new SocketState given the results from EndAcceptSocket.
        /// If successful will invoke the OnNetworkAction of the created SocketState and continue AcceptSocket loop.
        /// Will HandleError if anything goes wrong during connection process.
        /// </summary>
        private static void TryEndAcceptSocket(IAsyncResult ar, TcpListener listener, Action<SocketState> toCall)
        {
            try {
                Socket clientSocket = listener.EndAcceptSocket(ar);
                // create a new SocketState using the toCall delegate from StartServer and the new Socket from above
                SocketState socketState = new SocketState(toCall, clientSocket);
                // invoke the OnNetworkAction delegate so the user can take action
                socketState.OnNetworkAction(socketState);
            } catch (Exception e) {
                // if anything goes wrong during the connection process,
                // the OnNetworkAction delegate should be invoked with a error socket state 
                SocketState errorSocketState = CreateNewErrorSocketState(toCall);
                HandleError(errorSocketState, e);
                return;
            }
            // continue the AcceptSocket loop
            listener.BeginAcceptSocket(AcceptNewClient, (toCall, listener));
        }

        /// <summary>
        /// Stops the given TcpListener.
        /// note from the msdn docs: 
        /// The Stop method does not close any accepted connections. You are responsible for closing these separately.
        /// </summary>
        public static void StopServer(TcpListener listener)
        {
            listener?.Stop();
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of connecting to a server via BeginConnect, 
        /// and using ConnectedCallback as the method to finalize the connection once it's made.
        /// 
        /// If anything goes wrong during the connection process, toCall should be invoked 
        /// with a new SocketState with its ErrorOccured flag set to true and an appropriate message 
        /// placed in its ErrorMessage field. Between this method and ConnectedCallback, toCall should 
        /// only be invoked once on error.
        ///
        /// This connection process should timeout and produce an error (as discussed above) 
        /// if a connection can't be established within 3 seconds of starting BeginConnect.
        /// 
        /// </summary>
        /// <param name="toCall">The action to take once the connection is open or an error occurs</param>
        /// <param name="hostName">The server to connect to</param>
        /// <param name="port">The port on which the server is listening</param>
        public static void ConnectToServer(Action<SocketState> toCall, string hostName, int port)
        {

            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo = null;
            IPAddress ipAddress = IPAddress.None;

            if (!TryGetIPAddress(hostName, ref ipHostInfo, ref ipAddress)) {
                SocketState errorSocketState = CreateNewErrorSocketState(toCall);
                HandleError(errorSocketState, new NetworkingException("invalid ip address"));
                return;
            }

            Socket socket = CreateNewSocket(ipAddress);
            SocketState socketState = new SocketState(toCall, socket);
            TryBeginConnect(socket, ipAddress, port, ConnectedCallback, socketState);
        }

        /// <summary>
        /// Helper method to determine and verify the given IPAddress information before we can make a connection.
        /// if any of the steps fail, then we indicate an error to the user in the same way as usual 
        /// and cancel the connection process.
        /// </summary>
        private static bool TryGetIPAddress(string hostName, ref IPHostEntry ipHostInfo, ref IPAddress ipAddress)
        {
            // Determine if the server address is a URL or an IP
            try {
                ipHostInfo = Dns.GetHostEntry(hostName);
                bool foundIPV4 = false;
                foreach (IPAddress addr in ipHostInfo.AddressList) {
                    if (addr.AddressFamily != AddressFamily.InterNetworkV6) {
                        foundIPV4 = true;
                        ipAddress = addr;
                        break;
                    }
                }
                if (foundIPV4) {
                    return true;
                } else {
                    // Didn't find any IPV4 addresses
                    return false;
                }
            } catch (Exception) {
                // see if host name is a valid ipaddress
                try {
                    ipAddress = IPAddress.Parse(hostName);
                    return true;
                } catch (Exception) {
                    return false;
                }
            }
        }

        /// <summary>
        /// Helper method that attempts to BeginConnect and then will wait 3 seconds or timeout.
        /// If there is a timeout then will CloseSocket which triggers the ConnectedCallback once 
        /// which in turn will tigger TryEndConnect and eventually cause HandleError to be called.
        /// If there is a failure in the BeginConnect call will HandleError.
        /// </summary>
        private static void TryBeginConnect(Socket socket, IPAddress ipAddress, int port, AsyncCallback ConnectedCallback, SocketState socketState)
        {
            try {
                IAsyncResult result = socket.BeginConnect(ipAddress, port, ConnectedCallback, socketState);
                // if a connection can't be established within 3 seconds of starting BeginConnect, 
                // the connection process should timeout and produce an error. 
                int timeoutMilliseconds = 3000;  // 3 seconds
                bool success = WaitForResultOrTimeout(result, timeoutMilliseconds);
                if (!success) {
                    CloseSocket(socket);
                }
            } catch (Exception e) {
                HandleError(socketState, e);
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a connection process that was initiated by ConnectToServer.
        ///
        /// Uses EndConnect to finalize the connection.
        /// 
        /// As stated in the ConnectToServer documentation, if an error occurs during the connection process,
        /// either this method or ConnectToServer (not both) should indicate the error appropriately.
        /// 
        /// If a connection is successfully established, invokes the toCall Action that was provided to ConnectToServer (above)
        /// with a new SocketState representing the new connection.
        /// 
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginConnect</param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            SocketState socketState = (SocketState)ar.AsyncState;
            TryEndConnect(ar, socketState);
        }

        /// <summary>
        /// Helper method that will attempt to EndConnect and then invoke OnNetworkAction on the socket.
        /// If there is any failure of EndConnect will HandleError.
        /// </summary>
        private static void TryEndConnect(IAsyncResult ar, SocketState socketState)
        {
            try {
                socketState.TheSocket.EndConnect(ar);
                socketState.OnNetworkAction(socketState);
            } catch (Exception e) {
                HandleError(socketState, e);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Server and Client Common Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of receiving data via BeginReceive, using ReceiveCallback 
        /// as the callback to finalize the receive and store data once it has arrived.
        /// The object passed to ReceiveCallback via the AsyncResult should be the SocketState.
        /// 
        /// If anything goes wrong during the receive process, the SocketState's ErrorOccured flag should 
        /// be set to true, and an appropriate message placed in ErrorMessage, then the SocketState's
        /// OnNetworkAction should be invoked. Between this method and ReceiveCallback, OnNetworkAction should only be 
        /// invoked once on error.
        /// 
        /// </summary>
        /// <param name="socketState">The SocketState to begin receiving</param>
        public static void GetData(SocketState socketState)
        {
            TryGetData(socketState);
        }

        /// <summary>
        /// Helper method that attempts to BeginReceive.
        /// If the socket's BeginReceive operation fails, will HandleError.
        /// </summary>
        private static void TryGetData(SocketState socketState)
        {
            try {
                socketState.TheSocket.BeginReceive(socketState.buffer, 0, SocketState.BufferSize, SocketFlags.None, ReceiveCallback, socketState);
            } catch (Exception e) {
                HandleError(socketState, e);
            }

        }
        /// <summary>
        /// To be used as the callback for finalizing a receive operation that was initiated by GetData.
        /// 
        /// Uses EndReceive to finalize the receive.
        ///
        /// As stated in the GetData documentation, if an error occurs during the receive process,
        /// either this method or GetData (not both) should indicate the error appropriately.
        /// 
        /// If data is successfully received:
        ///  (1) Read the characters as UTF8 and put them in the SocketState's unprocessed data buffer (its string builder).
        ///      This must be done in a thread-safe manner with respect to the SocketState methods that access or modify its 
        ///      string builder.
        ///  (2) Call the saved delegate (OnNetworkAction) allowing the user to deal with this data.
        /// </summary>
        /// <param name="ar"> 
        /// This contains the SocketState that is stored with the callback when the initial BeginReceive is called.
        /// </param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState socketState = (SocketState)ar.AsyncState;
            try {
                int numBytesReceived = socketState.TheSocket.EndReceive(ar);
                if (numBytesReceived > 0) {
                    lock (socketState.data) {
                        socketState.data.Append(Encoding.UTF8.GetString(socketState.buffer, 0, numBytesReceived));
                    }
                    socketState.OnNetworkAction(socketState);
                } else {
                    // if we receive 0 bytes, that's an error. 
                    throw new NetworkingException("received 0 bytes");
                }
            } catch (Exception e) {
                HandleError(socketState, e);
            }
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendCallback to finalize the send process.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool Send(Socket socket, string data)
        {
            if (socket.Connected) {
                return TryBeginSend(socket, data, SendCallback);
            } else {
                CloseSocketIfConnected(socket);
                return false;
            }
        }

        /// <summary>
        /// Helper method that will attempt to BeginSend given data.
        /// If BeginSend call fails will make sure the socket is closed.
        /// </summary>
        /// <returns>If the BeginSend was successful</returns>
        private static bool TryBeginSend(Socket socket, string data, AsyncCallback Callback)
        {
            try {
                byte[] dataBuff = Encoding.ASCII.GetBytes(data);
                socket.BeginSend(dataBuff, 0, dataBuff.Length, 0, Callback, socket);
                return true;
            } catch (Exception e) {
                CloseSocketIfConnected(socket);
                return false;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by Send.
        ///
        /// Uses EndSend to finalize the send.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            TryEndSend(ar, socket);
        }

        /// <summary>
        /// Helper method that attempts to EndSend.
        /// If EndSend fails does not throw and exception.
        /// </summary>
        private static void TryEndSend(IAsyncResult ar, Socket socket)
        {
            try {
                socket.EndSend(ar);
            } catch (Exception e) {
                // Do nothing, do not want to throw as per SendCallback specifications
            }
        }


        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendAndCloseCallback to finalize the send process.
        /// This variant closes the socket in the callback once complete. This is useful for HTTP servers.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool SendAndClose(Socket socket, string data)
        {
            if (socket.Connected) {
                return TryBeginSend(socket, data, SendAndCloseCallback);
            } else {
                CloseSocketIfConnected(socket);
                return false;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by SendAndClose.
        ///
        /// Uses EndSend to finalize the send, then closes the socket.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// 
        /// This method ensures that the socket is closed before returning.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendAndCloseCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            TryEndSend(ar, socket);
            CloseSocketIfConnected(socket);
        }

    }
}

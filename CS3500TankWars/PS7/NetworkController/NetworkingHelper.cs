// Luke Ludlow, Ryan Dalby
// CS 3500
// 2019 Fall

using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkUtil
{
    /// <summary>
    /// helper methods for the Networking class. 
    /// </summary>
    internal static class NetworkingHelper
    {

        /// <summary>
        /// create a new socket state with the toCall delegate and the ErrorOccurred flag to true.
        /// this method is only used when a socket state doesn't exist yet.
        /// </summary>
        public static SocketState CreateNewErrorSocketState(Action<SocketState> toCall)
        {
            SocketState errorSocketState = new SocketState(toCall, null);
            errorSocketState.ErrorOccured = true;
            return errorSocketState;
        }
        
        /// <summary>
        /// set the socket state's ErrorOccurred flag to true and set the ErrorMessage to the given error message.
        /// </summary>
        public static SocketState SetSocketStateError(SocketState socketState, string errorMessage)
        {
            socketState.ErrorOccured = true;
            socketState.ErrorMessage = errorMessage;
            return socketState;
        }

        /// <summary>
        /// waits for the given async operation to complete. if it takes longer than the given timeout value,
        /// it will return false. 
        /// this method is mainly used for socket.BeginSend.
        /// </summary>
        public static bool WaitForResultOrTimeout(IAsyncResult result, int timeoutMilliseconds)
        {
            return result.AsyncWaitHandle.WaitOne(timeoutMilliseconds);
        }

        /// <summary>
        /// create a new TCP/IP socket object using the given IPAddress, 
        /// stream socket type, TCP protocol type, and with no delay.
        /// </summary>
        public static Socket CreateNewSocket(IPAddress ipAddress)
        {
            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive game like ours will be 
            socket.NoDelay = true;
            return socket;
        }

        /// <summary>
        /// indicate an error to the user by invoking the OnNetworkAction delegate with an error socket state.
        /// </summary>
        public static void HandleError(SocketState socketState, Exception e)
        {
            socketState = SetSocketStateError(socketState, e.Message);
            socketState.OnNetworkAction(socketState);
        }


        /// <summary>
        /// shutdown and close the socket.
        /// 
        /// if the socket is not connected, then the socket has already been closed, 
        /// so we shouldn't call close again because it will throw an object disposed exception.
        /// </summary>
        public static void CloseSocketIfConnected(Socket socket)
        {
            if (socket.Connected) {
                CloseSocket(socket);
            }
        }

        /// <summary>
        /// NOTE: this method should only be directly invoked if the initial connection process fails!
        /// 
        /// if the call to BeginSend times out, then we need to close the socket. however, 
        /// the socket's Connected property is still false, so we need to go ahead and call Close anyway,
        /// because we know that the socket has not been disposed yet. in all other cases, the socket 
        /// is probably already disposed if Connected is false, that's why we use CloseSocketIfConnected.
        /// </summary>
        public static void CloseSocket(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }


    }
}
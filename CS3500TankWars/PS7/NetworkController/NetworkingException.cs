// Luke Ludlow, Ryan Dalby
// CS 3500
// 2019 Fall

using System;

namespace NetworkUtil
{
    /// <summary>
    /// custom exception to be caught and thrown within the Networking code.
    /// this exception class is public so that the future game client and server can utilize it if they want to.
    /// </summary>
    public class NetworkingException : Exception
    {
        public NetworkingException(string message)
            : base(message)
        {
        }
    }
}
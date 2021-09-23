// Luke Ludlow, Ryan Dalby
// CS 3500
// 2019 Fall

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace NetworkUtil
{
    /// <summary>
    /// unit tests for the PS7 networking library. 
    /// 
    /// test methods follow this naming structure:
    /// [MethodName_StateUnderTest_ExpectedBehavior]
    /// 
    /// note: the given PS7 tests are put in the ExampleNetworkTests class. 
    /// this NetworkTests class contains our own additional tests.
    /// </summary>
    [TestClass]
    public class NetworkTests
    {

        private TcpListener testListener;
        private SocketState testLocalSocketState;
        private SocketState testRemoteSocketState;

        [TestInitialize]
        public void Init()
        {
            testListener = null;
            testLocalSocketState = null;
            testRemoteSocketState = null;
        }

        [TestCleanup]
        public void Cleanup()
        {
            NetworkTestHelper.StopTestServer(testListener, testLocalSocketState, testRemoteSocketState);
        }


        public void SetupTestConnections(bool clientSide,
          out TcpListener listener, out SocketState local, out SocketState remote)
        {
            if (clientSide) {
                NetworkTestHelper.SetupSingleConnectionTest(
                  out listener,
                  out local,    // local becomes client
                  out remote);  // remote becomes server
            } else {
                NetworkTestHelper.SetupSingleConnectionTest(
                  out listener,
                  out remote,   // remote becomes client
                  out local);   // local becomes server
            }
            Assert.IsNotNull(local);
            Assert.IsNotNull(remote);
        }


        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void SendAndClose_SendTinyMessage_ShouldPutDataInSocketStateAndCloseSocket(bool clientSide)
        {
            // assemble
            SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);
            testLocalSocketState.OnNetworkAction = x => { };
            testRemoteSocketState.OnNetworkAction = x => { };
            bool sendWasSuccessful;

            // act
            sendWasSuccessful = Networking.SendAndClose(testLocalSocketState.TheSocket, "abc");
            Networking.GetData(testRemoteSocketState);
            NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

            // assert
            Assert.IsTrue(sendWasSuccessful);
            Assert.AreEqual("abc", testRemoteSocketState.GetData());
            Assert.IsFalse(testLocalSocketState.TheSocket.Connected);
        }

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void SendAndClose_SocketIsAlreadyClosed_ShouldNotAttemptToSend(bool clientSide)
        {
            SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);
            testLocalSocketState.OnNetworkAction = x => { };
            testRemoteSocketState.OnNetworkAction = x => { };
            bool sendWasSuccessful = true;

            testLocalSocketState.TheSocket.Shutdown(SocketShutdown.Both);
            testLocalSocketState.TheSocket.Close();
            sendWasSuccessful = Networking.SendAndClose(testLocalSocketState.TheSocket, "abc");
            Networking.GetData(testRemoteSocketState);
            NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

            Assert.IsFalse(sendWasSuccessful);
            Assert.AreEqual("", testRemoteSocketState.GetData());
            Assert.IsFalse(testLocalSocketState.TheSocket.Connected);
            // TODO is this check helpful? 
            Assert.ThrowsException<ObjectDisposedException>(() => testLocalSocketState.TheSocket.Available);
        }

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public void SendAndClose_SendFails_ShouldCloseSocket(bool clientSide)
        {
            SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);
            testLocalSocketState.OnNetworkAction = x => { };
            testRemoteSocketState.OnNetworkAction = x => { };

            string nullInjection = null;
            bool sendWasSuccessful = Networking.SendAndClose(testLocalSocketState.TheSocket, nullInjection);
            Networking.GetData(testRemoteSocketState);
            NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

            Assert.IsFalse(sendWasSuccessful);
            Assert.AreEqual("", testRemoteSocketState.GetData());
            Assert.IsFalse(testLocalSocketState.TheSocket.Connected);
        }


        [TestMethod]
        public void ConnectToServer_ErrorOccursDuringSocketBeginConnect_ShouldReturnErrorSocketStateAndInvokeToCallDelegateOnce()
        {
            bool isCalled = false;
            int numTimesCalled = 0;
            void saveClientState(SocketState x)
            {
                isCalled = true;
                numTimesCalled++;
                testLocalSocketState = x;
            }
            Networking.ConnectToServer(saveClientState, "localhost", 99999999);
            NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);
            Assert.IsTrue(isCalled);
            Assert.IsTrue(testLocalSocketState.ErrorOccured);
            Assert.AreEqual(1, numTimesCalled);
        }

        [TestMethod]
        public void ConnectToServer_HostNameIsInvalidIPAddress_ShouldReturnErrorSocketStateAndInvokeToCallDelegateOnce()
        {
            bool isCalled = false;
            int numTimesCalled = 0;
            void saveClientState(SocketState x)
            {
                isCalled = true;
                numTimesCalled++;
                testLocalSocketState = x;
            }
            Networking.ConnectToServer(saveClientState, "ybxiciwjwlpdooyqwwesxnvlezxiqe.com", 2112);
            NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);
            Assert.IsTrue(isCalled);
            Assert.IsTrue(testLocalSocketState.ErrorOccured);
            Assert.AreEqual(1, numTimesCalled);
        }


        [TestMethod]
        public void ConnectToServer_CouldNotFindIPV4Address_ShouldReturnErrorSocketStateAndInvokeToCallDelegateOnce()
        {
            bool isCalled = false;
            int numTimesCalled = 0;
            void saveClientState(SocketState x)
            {
                isCalled = true;
                numTimesCalled++;
                testLocalSocketState = x;
            }
            Networking.ConnectToServer(saveClientState, "ipv6.google.com", 2112);
            NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);
            Assert.IsTrue(isCalled);
            Assert.IsTrue(testLocalSocketState.ErrorOccured);
            Assert.AreEqual(1, numTimesCalled);
        }


        [TestMethod]
        public void ConnectToServer_BeginConnectWillStartButThenTimeout_ShouldInvokeToCallDelegateOnce()
        {
            bool isCalled = false;
            int numTimesCalled = 0;
            void saveClientState(SocketState x)
            {
                isCalled = true;
                numTimesCalled++;
                testLocalSocketState = x;
            }
            Networking.ConnectToServer(saveClientState, "google.com", 2112);
            NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);
            Assert.IsTrue(isCalled);
            Assert.IsTrue(testLocalSocketState.ErrorOccured);
            Assert.AreEqual(1, numTimesCalled);
        }


        [DataRow(true)]
        [DataRow(false)]
        [DataTestMethod]
        public void GetData_SocketIsAlreadyClosedSoBeginReceiveWillFail_ShouldSetErrorSocketStateAndInvokeToCallDelegate(bool clientSide)
        {
            bool isCalled = false;
            int numTimesCalled = 0;
            void saveClientState(SocketState x)
            {
                isCalled = true;
                numTimesCalled++;
                testLocalSocketState = x;
            }
            testLocalSocketState = new SocketState(saveClientState, null);

            Networking.GetData(testLocalSocketState);
            NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);

            Assert.AreEqual(1, numTimesCalled);
            Assert.IsTrue(testLocalSocketState.ErrorOccured);
            Assert.AreEqual("", testLocalSocketState.GetData());
        }


        [TestMethod]
        public void StopServer_NullTcpListener_ShouldDoNothing()
        {
            // this should not throw an exception
            Networking.StopServer(null);
        }




    }
}

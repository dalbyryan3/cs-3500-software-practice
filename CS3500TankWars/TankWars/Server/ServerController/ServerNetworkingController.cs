using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TankWars
{
    public class ServerNetworkingController
    {
        // Keep track of clients
        private Dictionary<long, SocketState> clients;
        private Dictionary<long, int> clientToPlayerMap;

        private GameController gameController;

        public ServerNetworkingController(GameController gameController)
        {
            clients = new Dictionary<long, SocketState>();
            clientToPlayerMap = new Dictionary<long, int>();
            this.gameController = gameController;
        }

        public TcpListener StartGameServer()
        {
            return Networking.StartServer(NewClientConnected, 11000);
        }


        public void SendMessageToEveryClient(string message)
        {
            lock (clients) {
                foreach (SocketState client in clients.Values.ToList()) {
                    Networking.Send(client.TheSocket, message);
                }
            }
        }

        private void NewClientConnected(SocketState state)
        {
            if (state.ErrorOccured) {
                return; // Possibly need to add something
            }
            state.OnNetworkAction = ReceivePlayerName;
            Networking.GetData(state);
        }

        private void ReceivePlayerName(SocketState state)
        {
            if (state.ErrorOccured) {
                return; // Possibly need to add something
            }
            List<string> socketData = ExtractSocketData(state, 1);
            string playerName = socketData[0];
            playerName = playerName.Trim();

            // the order of these next operations is very important!!!
            int playerID = gameController.NewPlayerJoined(playerName);
            state.OnNetworkAction = ReceiveMessage;
            SendStartupInfo(state, playerID, gameController.GameSettings.UniverseSize);
            AddClient(state, playerID);
            Networking.GetData(state);
        }

        private void AddClient(SocketState state, int playerID)
        {
            lock (clients) {
                clients[state.ID] = state;
                clientToPlayerMap[state.ID] = playerID;
            }
        }

        private void SendStartupInfo(SocketState state, int playerID, int worldSize)
        {
            string message = playerID + "\n" + worldSize + "\n";
            Networking.Send(state.TheSocket, message);
        }

        private void RemoveClient(SocketState state)
        {
            lock (gameController) {
                gameController.PlayerDisconnected(clientToPlayerMap[state.ID]);
                clients.Remove(state.ID);
                clientToPlayerMap.Remove(state.ID);
                // send the message again that has the latest game world state with the disconnected and dead
                SendMessageToEveryClient(gameController.SerializeGameWorld());
            }
        }

        private void ReceiveMessage(SocketState state)
        {
            if (state.ErrorOccured) {
                RemoveClient(state);
                return; // Want to handle this situation
            }
            // This is the callback for all recieved movement commands associated with a client 
            // Do stuff with recieved movement commands 
            List<string> extractedData = ExtractSocketData(state);
            foreach (string json in extractedData) {
                if (TryParseJsonAsTankControlCommand(json, out TankControlCommand tankControlCommand)) {
                    gameController.UpdateGameControlState(clientToPlayerMap[state.ID], tankControlCommand);
                }
                // Will do nothing if malformed JSON is recieved 
            }

            Networking.GetData(state);
        }

        private List<string> ExtractSocketData(SocketState socketState, int numLinesToRead = int.MaxValue)
        {
            lock (socketState) {
                List<string> socketData = new List<string>();
                int numLinesRead = 0;
                foreach (string s in GetSocketDataSplitByNewlines(socketState)) {
                    if (string.IsNullOrWhiteSpace(s)) {
                        // sometimes the regex split creates empty strings, ignore them and continue looping
                        continue;
                    }
                    if (!StringEndsWithNewline(s)) {
                        // the rest of the message is incomplete, break the loop to leave the extra data alone 
                        break;
                    }
                    socketData.Add(s);
                    socketState.RemoveData(0, s.Length);
                    numLinesRead++;
                    if (numLinesRead >= numLinesToRead) {
                        break;
                    }
                }
                return socketData;
            }
        }

        private string[] GetSocketDataSplitByNewlines(SocketState socketState)
        {
            string totalData = socketState.GetData();
            string[] splitPartsByNewline = Regex.Split(totalData, @"(?<=[\n])");
            return splitPartsByNewline;
        }

        private bool StringEndsWithNewline(string s)
        {
            return s[s.Length - 1] == '\n';
        }

        private bool TryParseJsonAsTankControlCommand(string json, out TankControlCommand tankControlCommand)
        {
            JObject parsedObject = JObject.Parse(json);
            JToken movingAttribute = parsedObject["moving"];
            if (movingAttribute != null) {
                tankControlCommand = (TankControlCommand)parsedObject.ToObject<TankControlCommand>();
                return true;
            } else {
                tankControlCommand = null;
                return false;
            }
        }

    }
}

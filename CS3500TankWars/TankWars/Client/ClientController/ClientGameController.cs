// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Linq;
using System.Text.RegularExpressions;
using NetworkUtil;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TankWars
{
    /// <summary>
    /// This class represents a GameController for TankWars.
    /// It functions to handle user input, communicate with the server,
    /// update the game world model appropriately, and tell the view to 
    /// draw the next frame.
    /// </summary>
    public class GameController
    {
        private SocketState serverSocketState; // Represents connection with the server 
        private string playerName; // Represents the client's name
        public World theWorld { get; private set; } // Represents the client's world; Will be updated with server updates
        private TankControlCommand tankControlCommand; // Represents the control state of the client's tank

        // Object to hold current keypress direction state (Will exclusively contain up to one instance of each of the following: "up", "left", "down", or "right")
        private List<string> currentPressedKeyDirection = new List<string>();

        // Define events and the delegates that can be registered to them.
        public delegate void ShowErrorMessageHandler(string errorMessage, string title);
        public event ShowErrorMessageHandler ShowErrorMessageEvent;
        public delegate void DisableConnectionMenuHandler();
        public event DisableConnectionMenuHandler DisableConnectionMenuEvent;
        public delegate void RedrawFrameHandler();
        public event RedrawFrameHandler RedrawFrameEvent;


        public GameController()
        {
            theWorld = new World();
            tankControlCommand = new TankControlCommand();
        }

        /// <summary>
        /// Begins the handshake process of connecting to a TankWars server.
        /// if any errors occur during the connection process, the view will display an informative error message
        /// and the user can try again.
        /// </summary>
        public void ConnectToServer(string hostName, string playerName)
        {
            if (PlayerNameIsValid(playerName)) {
                this.playerName = playerName;
                int portNumber = 11000;  // networking protocol requires port number to always be 11000
                Networking.ConnectToServer(OnFirstConnect, hostName, portNumber);
            } else {
                ShowErrorMessageEvent?.Invoke("name must be 16 characters or less. please enter a new name.", "invalid name");
            }
        }

        private bool PlayerNameIsValid(string name)
        {
            return name.Length > 0 && name.Length <= 16;
        }

        private void OnFirstConnect(SocketState socketState)
        {
            serverSocketState = socketState;
            if (socketState.ErrorOccured) {
                ShowErrorMessageEvent?.Invoke(serverSocketState.ErrorMessage + " please restart the game client.", "networking error");
                return;
            } else {
                serverSocketState = socketState;
                serverSocketState.OnNetworkAction = ReceiveStartupMessage;

                SendPlayerName();
                // this line starts the async operation that will eventually call ReceiveStartupMessage
                Networking.GetData(serverSocketState);
            }
        }

        private void ReceiveStartupMessage(SocketState socketState)
        {
            serverSocketState = socketState;

            int numLinesToRead = 2;
            List<string> socketData = ExtractSocketData(serverSocketState, numLinesToRead);
            if (socketData.Count == 2) {
                string playerIDString = socketData[0].Replace("\n", "");
                string worldSizeString = socketData[1].Replace("\n", "");
                theWorld.PlayerID = int.Parse(playerIDString);
                theWorld.Size = int.Parse(worldSizeString);

                // handshake successful
                DisableConnectionMenuEvent?.Invoke();
                // start the normal event loop 
                serverSocketState.OnNetworkAction = ReceiveServerUpdate;
                Networking.GetData(serverSocketState);
            } else {
                // keep on waiting for the full startup message
                return;
            }
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

        private void SendPlayerName()
        {
            string message = playerName + "\n";
            Networking.Send(serverSocketState.TheSocket, message);
        }

        private void ReceiveServerUpdate(SocketState socketState)
        {
            serverSocketState = socketState;
            if (serverSocketState.ErrorOccured) {
                ShowErrorMessageEvent?.Invoke(serverSocketState.ErrorMessage + " please restart the game client.", "networking error");
                return;
            } else {
                Networking.GetData(serverSocketState);
                List<string> socketData = ExtractSocketData(serverSocketState);
                UpdateWorld(theWorld, socketData);
            }

            SendTankControlCommand(tankControlCommand);
            RedrawFrameEvent?.Invoke();
        }

        private void UpdateWorld(World world, List<string> socketData)
        {
            lock (theWorld) {
                ClearBeamsAndProjectiles();
                foreach (string s in socketData) {
                    if (TryParseJsonAsTank(s, out Tank newTank)) {
                        AddTankToWorld(theWorld, newTank);
                    } else if (TryParseJsonAsWall(s, out Wall newWall)) {
                        AddWallToWorld(theWorld, newWall);
                    } else if (TryParseJsonAsProjectile(s, out Projectile newProjectile)) {
                        AddProjectileToWorld(theWorld, newProjectile);
                    } else if (TryParseJsonAsBeam(s, out Beam newBeam)) {
                        AddBeamToWorld(theWorld, newBeam);
                    } else if (TryParseJsonAsPowerup(s, out Powerup newPowerup)) {
                        AddPowerupToWorld(theWorld, newPowerup);
                    }
                }
            }
        }

        private void ClearBeamsAndProjectiles()
        {
            theWorld.Beams.Clear();
            theWorld.Projectiles.Clear();
        }


        private void AddTankToWorld(World world, Tank tank)
        {
            if (tank.Disconnected) {
                world.Tanks.Remove(tank.ID);
            } else {
                world.Tanks[tank.ID] = tank;
            }
        }

        private void AddWallToWorld(World world, Wall wall)
        {
            world.Walls[wall.ID] = wall;
        }

        private void AddProjectileToWorld(World world, Projectile projectile)
        {
            if (projectile.IsDead) {
                world.Projectiles.Remove(projectile.ID);
            } else {
                world.Projectiles[projectile.ID] = projectile;
            }
        }

        private void AddBeamToWorld(World world, Beam beam)
        {
            world.Beams[beam.ID] = beam;
        }

        private void AddPowerupToWorld(World world, Powerup powerup)
        {
            if (powerup.IsDead) {
                world.Powerups.Remove(powerup.ID);
            } else {
                world.Powerups[powerup.ID] = powerup;
            }
        }


        private bool TryParseJsonAsTank(string json, out Tank newTank)
        {
            JObject parsedObject = JObject.Parse(json);
            JToken tankID = parsedObject["tank"];
            if (tankID != null) {
                newTank = (Tank)parsedObject.ToObject<Tank>();
                return true;
            } else {
                newTank = null;
                return false;
            }
        }

        private bool TryParseJsonAsWall(string json, out Wall newWall)
        {
            JObject parsedObject = JObject.Parse(json);
            JToken wallID = parsedObject["wall"];
            if (wallID != null) {
                newWall = (Wall)parsedObject.ToObject<Wall>();
                return true;
            } else {
                newWall = null;
                return false;
            }
        }

        private bool TryParseJsonAsProjectile(string json, out Projectile newProjectile)
        {
            JObject parsedObject = JObject.Parse(json);
            JToken projectileID = parsedObject["proj"];
            if (projectileID != null) {
                newProjectile = (Projectile)parsedObject.ToObject<Projectile>();
                return true;
            } else {
                newProjectile = null;
                return false;
            }
        }

        private bool TryParseJsonAsBeam(string json, out Beam newBeam)
        {
            JObject parsedObject = JObject.Parse(json);
            JToken beamID = parsedObject["beam"];
            if (beamID != null) {
                newBeam = (Beam)parsedObject.ToObject<Beam>();
                return true;
            } else {
                newBeam = null;
                return false;
            }
        }

        private bool TryParseJsonAsPowerup(string json, out Powerup newPowerup)
        {
            JObject parsedObject = JObject.Parse(json);
            JToken powerupID = parsedObject["power"];
            if (powerupID != null) {
                newPowerup = (Powerup)parsedObject.ToObject<Powerup>();
                return true;
            } else {
                newPowerup = null;
                return false;
            }
        }

        /// <summary>
        /// get the location of the main player's tank. 
        /// this is mainly needed for centering the player's view.
        /// </summary>
        public Vector2D GetPlayerLocation()
        {
            Vector2D playerLocation;
            if (theWorld.Tanks.TryGetValue(theWorld.PlayerID, out Tank playerTank)) {
                playerLocation = playerTank.Location;
            } else {
                playerLocation = new Vector2D(0, 0);
            }
            return playerLocation;
        }

        private void SendTankControlCommand(TankControlCommand controlCommand)
        {
            string controlCommandJson = JsonConvert.SerializeObject(controlCommand) + "\n";
            Networking.Send(serverSocketState.TheSocket, controlCommandJson);
        }

        /// <summary>
        /// Processes a KeyDown event by attempting to add the direction corresponding 
        /// to the KeyDown to the List representing the current state of keys that are pressed down.
        /// If the direction is added to the current state of the keys that are being pressed down
        /// the Moving property of the ContorlCommand of theWorld is updated.
        /// </summary>
        public void ProcessKeyDown(string keyStr)
        {

            switch (keyStr) {
                case "W":
                    TryAddCurrentPressedKeyDirection(KeyStrToDirection("W"));
                    break;
                case "A":
                    TryAddCurrentPressedKeyDirection(KeyStrToDirection("A"));
                    break;
                case "S":
                    TryAddCurrentPressedKeyDirection(KeyStrToDirection("S"));
                    break;
                case "D":
                    TryAddCurrentPressedKeyDirection(KeyStrToDirection("D"));
                    break;
            }
        }
        private void TryAddCurrentPressedKeyDirection(string direction)
        {
            if (!currentPressedKeyDirection.Contains(direction)) {
                currentPressedKeyDirection.Add(direction);
                tankControlCommand.Moving = direction;
            }
        }
        private string KeyStrToDirection(string keyStr)
        {

            switch (keyStr) {
                case "W":
                    return "up";
                case "A":
                    return "left";
                case "S":
                    return "down";
                case "D":
                    return "right";
            }
            return "";
        }

        /// <summary>
        /// Processes a KeyUp event by removing the corresponding key
        /// and then checking what keys are still being pressed 
        /// and updating the Moving property of the ControlCommand of theWorld
        /// to be moving in the direction of the key on top of the List (in a Stack sense)
        /// if there are not keys still being pressed will set ControlCommand to "none"
        /// </summary>
        public void ProcessKeyUp(string keyStr)
        {
            currentPressedKeyDirection.Remove(KeyStrToDirection(keyStr));
            if (currentPressedKeyDirection.Count > 0) {
                tankControlCommand.Moving = currentPressedKeyDirection[currentPressedKeyDirection.Count - 1];
            } else {
                tankControlCommand.Moving = "none";
            }
        }

        /// <summary>
        /// Processes a MouseMove event by updating the TurretDirection of the ControlCommand of theWorld
        /// </summary>
        public void ProcessMouseMove(double posX, double posY)
        {
            Vector2D newTurretDirection = new Vector2D(posX, posY);
            newTurretDirection.Normalize();
            lock (theWorld) {
                tankControlCommand.TurretDirection = newTurretDirection;
            }
        }

        /// <summary>
        /// Processes a MouseDown event by updating the Fire property of the ControlCommand of theWorld.
        /// </summary>
        public void ProcessMouseDown(string mouseButton)
        {
            string fireCommand = "none";
            switch (mouseButton) {
                case "Left":
                    fireCommand = "main";
                    break;
                case "Right":
                    fireCommand = "alt";
                    break;
            }
            lock (theWorld) {
                tankControlCommand.Fire = fireCommand;
            }
        }
        /// <summary>
        /// Processes a MouseUp event by updating the Fire property of the ControlCommand of theWorld.
        /// </summary>
        public void ProcessMouseUp(string mouseButton)
        {
            bool isMouseButton = (mouseButton == "Left" || mouseButton == "Right");
            if (isMouseButton) {
                lock (theWorld) {
                    tankControlCommand.Fire = "none";
                }
            }
        }



    }
}
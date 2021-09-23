using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TankWars
{
    public class GameController
    {
        // The settings of the game are part of a gameController
        public GameSettings GameSettings { get; private set; }

        // Will hold the state of the world, modified by gameMechanics
        public World GameWorld { get; private set; }

        // calculates and tracks the amount of seconds that have passed since the game started.
        // this is calculated by incrementing a frame count every time Update is called and
        // multiplying it by the milliseconds per frame in GameSettings.
        // the seconds are calculated as a double and then truncated to a uint, because the database schema
        // requires a uint for the duration.
        public uint GameDurationInSeconds { get; private set; }
        private int gameDurationInFrames;

        // This handles the mechanics behind the game (the model of the game)
        private GameLogic gameLogic;

        // The current control state of the game is part of a gameController
        private GameControlState gameControlState;


        public GameController()
        {
            // Initialize GameSettings
            GameSettings = new GameSettings();
            string settingsFilePath = @"..\..\..\..\Resources\Server\settings.xml";
            GameSettings.ReadSettingsFile(settingsFilePath);

            // Setup a new world
            GameWorld = new World();
            GameWorld.Size = GameSettings.UniverseSize;

            GameDurationInSeconds = 0;
            gameDurationInFrames = 0;

            // Create a GameControlState to hold game control state of the world
            gameControlState = new GameControlState();

            gameLogic = new GameLogic(GameWorld, GameSettings, gameControlState);
        }

        public int NewPlayerJoined(string playerName)
        {
            return gameLogic.NewPlayerJoined(playerName);
        }

        public void PlayerDisconnected(int playerID)
        {
            gameLogic.RemovePlayerFromGame(playerID);
        }


        public void InitializeWorld()
        {
            gameLogic.InitializeWorld();
        }

        private uint CalculateGameDurationInSeconds()
        {
            double totalMillisecondsPassed = gameDurationInFrames * GameSettings.MillisecondsPerFrame;
            double seconds = TimeSpan.FromMilliseconds(totalMillisecondsPassed).TotalSeconds;
            uint secondsAsUInt = Convert.ToUInt32(Math.Truncate(seconds));
            return secondsAsUInt;
        }

        public void UpdateWorld()
        {
            gameLogic.CleanupWorld();
            gameLogic.UpdateMotion();
            gameLogic.UpdateGameLogic();
            gameLogic.UpdateCollisions();
            gameDurationInFrames++;
            GameDurationInSeconds = CalculateGameDurationInSeconds();
        }

        public string SerializeGameWorld()
        {
            StringBuilder message = new StringBuilder();
            foreach (Wall wall in GameWorld.Walls.Values) {
                message.Append(JsonConvert.SerializeObject(wall));
                message.Append("\n");
            }
            foreach (Tank tank in GameWorld.Tanks.Values) {
                message.Append(JsonConvert.SerializeObject(tank));
                message.Append("\n");
            }
            foreach (Projectile projectile in GameWorld.Projectiles.Values) {
                message.Append(JsonConvert.SerializeObject(projectile));
                message.Append("\n");
            }
            foreach (Beam beam in GameWorld.Beams.Values) {
                message.Append(JsonConvert.SerializeObject(beam));
                message.Append("\n");
            }
            foreach (Powerup powerup in GameWorld.Powerups.Values) {
                message.Append(JsonConvert.SerializeObject(powerup));
                message.Append("\n");
            }
            return message.ToString();
        }

        public void UpdateGameControlState(int tankID, TankControlCommand tankControlCommand)
        {
            gameControlState.UpdateTankControlCommand(tankID, tankControlCommand);
        }


    }
}
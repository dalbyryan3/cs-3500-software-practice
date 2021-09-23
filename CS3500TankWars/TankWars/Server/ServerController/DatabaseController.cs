using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    public class DatabaseController
    {
        private readonly string connectString = "server=atr.eng.utah.edu;database=cs3500_u0848407;uid=cs3500_u0848407;password=skiing";

        public DatabaseController()
        {
        }

        public Dictionary<uint, DatabaseGameModel> GetAllGames()
        {
            Dictionary<uint, DatabaseGameModel> gameContainer = new Dictionary<uint, DatabaseGameModel>();
            using (MySqlConnection sqlConnection = new MySqlConnection(connectString)) {
                sqlConnection.Open();
                MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT * FROM Games;";
                using (MySqlDataReader reader = sqlCommand.ExecuteReader()) {
                    while (reader.Read()) {
                        uint gameID = (uint)reader["gID"];
                        uint duration = (uint)reader["Duration"];
                        DatabaseGameModel g = new DatabaseGameModel(gameID, duration);
                        gameContainer.Add(gameID, g);
                    }

                }

                foreach (DatabaseGameModel g in gameContainer.Values) {
                    string selectBygIDCommandText = "SELECT * FROM Players NATURAL JOIN GamesPlayed WHERE gID = " + g.ID + ";";
                    sqlCommand.CommandText = selectBygIDCommandText;
                    using (MySqlDataReader reader = sqlCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            string name = (string)reader["Name"];
                            uint score = (uint)reader["Score"];
                            uint accuracy = (uint)reader["Accuracy"];
                            g.AddPlayer(name, score, accuracy);
                        }
                    }
                }
            }
            return gameContainer;
        }


        public List<DatabaseSessionModel> GetPlayerSessions(string playerName)
        {

            List<DatabaseSessionModel> playerSessionContainer = new List<DatabaseSessionModel>();

            using (MySqlConnection sqlConnection = new MySqlConnection(connectString)) {
                sqlConnection.Open();
                MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT * FROM Players NATURAL JOIN Games NATURAL JOIN GamesPlayed WHERE Name = \"" + playerName + "\";";
                using (MySqlDataReader reader = sqlCommand.ExecuteReader()) {
                    while (reader.Read()) {
                        uint gid = (uint)reader["gID"];
                        uint duration = (uint)reader["Duration"];
                        uint score = (uint)reader["Score"];
                        uint accuracy = (uint)reader["Accuracy"];
                        DatabaseSessionModel m = new DatabaseSessionModel(gid, duration, score, accuracy);
                        playerSessionContainer.Add(m);
                    }

                }

            }
            return playerSessionContainer;
        }


        public void SaveGameToDatabase(World gameWorld, uint gameDuration)
        {
            InsertGameModelIntoDatabase(gameDuration, CreateDatabasePlayerModels(gameWorld));
        }

        private List<DatabasePlayerModel> CreateDatabasePlayerModels(World gameWorld)
        {
            List<DatabasePlayerModel> players = new List<DatabasePlayerModel>();
            foreach (Tank tank in gameWorld.Tanks.Values) {
                DatabasePlayerModel player = new DatabasePlayerModel(tank.PlayerName, Convert.ToUInt32(tank.Score), CalculatePlayerAccuracy(tank));
                players.Add(player);
            }
            return players;
        }

        private uint CalculatePlayerAccuracy(Tank tank)
        {
            if (tank.ShotsFired <= 0) {
                return 0;
            }
            double playerAccuracyAsDouble = (double)tank.ShotsHit / (double)tank.ShotsFired;
            double playerAccuracyPercentage = Math.Truncate(playerAccuracyAsDouble * 100);
            uint playerAccuracy = Convert.ToUInt32(playerAccuracyPercentage);
            return playerAccuracy;
        }

        private void InsertGameModelIntoDatabase(uint gameDuration, List<DatabasePlayerModel> gamePlayers)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(connectString)) {
                sqlConnection.Open();
                MySqlCommand sqlCommand = sqlConnection.CreateCommand();

                StringBuilder sqlCommandText = new StringBuilder();

                sqlCommandText.Append("INSERT INTO Games(Duration) VALUES(");
                sqlCommandText.Append(gameDuration);
                sqlCommandText.Append(");");
                sqlCommand.CommandText = sqlCommandText.ToString();
                sqlCommand.ExecuteNonQuery();
                string gameID = sqlCommand.LastInsertedId.ToString();
                sqlCommandText.Clear();

                foreach (DatabasePlayerModel player in gamePlayers) {
                    sqlCommandText.Append("INSERT INTO Players(Name) VALUES(\"");
                    sqlCommandText.Append(player.Name);
                    sqlCommandText.Append("\");");
                    sqlCommand.CommandText = sqlCommandText.ToString();
                    sqlCommand.ExecuteNonQuery();
                    string playerID = sqlCommand.LastInsertedId.ToString();
                    sqlCommandText.Clear();

                    sqlCommandText.Append("INSERT INTO GamesPlayed(gID, pID, Score, Accuracy) VALUES(");
                    sqlCommandText.Append(gameID);
                    sqlCommandText.Append(",");
                    sqlCommandText.Append(playerID);
                    sqlCommandText.Append(",");
                    sqlCommandText.Append(player.Score.ToString());
                    sqlCommandText.Append(",");
                    sqlCommandText.Append(player.Accuracy.ToString());
                    sqlCommandText.Append(");");
                    sqlCommand.CommandText = sqlCommandText.ToString();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommandText.Clear();
                }
            }
        }


    }
}

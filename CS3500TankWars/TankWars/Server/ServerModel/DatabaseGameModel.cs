using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// A simple container class representing one game and its players
    /// </summary>
    public class DatabaseGameModel
    {
        public readonly uint ID;
        public readonly uint Duration;
        private List<DatabasePlayerModel> players;

        public DatabaseGameModel(uint id, uint duration)
        {
            this.ID = id;
            this.Duration = duration;
            this.players = new List<DatabasePlayerModel>();
        }

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        /// <param name="name">The player's name</param>
        /// <param name="score">The player's score</param>
        /// <param name="accuracy">The player's accuracy</param>
        public void AddPlayer(string name, uint score, uint accuracy)
        {
            players.Add(new DatabasePlayerModel(name, score, accuracy));
        }

        /// <summary>
        /// Returns the players in this game
        /// </summary>
        /// <returns></returns>
        public List<DatabasePlayerModel> GetPlayers()
        {
            return players;
        }
    }
}

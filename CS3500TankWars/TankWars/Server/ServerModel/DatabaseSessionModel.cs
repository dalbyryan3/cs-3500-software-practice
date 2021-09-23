using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// A simple container class representing the information about one player's session in one game
    /// </summary>
    public class DatabaseSessionModel
    {
        public readonly uint GameID;
        public readonly uint Duration;
        public readonly uint Score;
        public readonly uint Accuracy;

        public DatabaseSessionModel(uint gid, uint duration, uint score, uint accuracy)
        {
            this.GameID = gid;
            this.Duration = duration;
            this.Score = score;
            this.Accuracy = accuracy;
        }
    }
}

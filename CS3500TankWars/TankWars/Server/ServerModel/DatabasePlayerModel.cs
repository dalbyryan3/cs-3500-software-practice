using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{

    /// <summary>
    /// A simple container class representing one player in one game
    /// </summary>
    public class DatabasePlayerModel
    {
        public readonly string Name;
        public readonly uint Score;
        public readonly uint Accuracy;
        public DatabasePlayerModel(string name, uint score, uint accuracy)
        {
            this.Name = name;
            this.Score = score;
            this.Accuracy = accuracy;
        }
    }

}

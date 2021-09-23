// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// This class contains a representation of a TankWars world
    /// </summary>
    public class World
    {
        public Dictionary<int, Tank> Tanks { get; set; }
        public Dictionary<int, Projectile> Projectiles { get; set; }
        public Dictionary<int, Wall> Walls { get; set; }
        public Dictionary<int, Beam> Beams { get; set; }
        public Dictionary<int, Powerup> Powerups { get; set; }

        public int Size { get; set; }

        public int PlayerID { get; set; }


        public World()
        {
            Tanks = new Dictionary<int, Tank>();
            Projectiles = new Dictionary<int, Projectile>();
            Walls = new Dictionary<int, Wall>();
            Beams = new Dictionary<int, Beam>();
            Powerups = new Dictionary<int, Powerup>();
        }

    }
}

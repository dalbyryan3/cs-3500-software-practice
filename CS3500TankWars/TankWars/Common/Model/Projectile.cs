// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// This class contains a representation of a TankWars Projectile.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        /// <summary>
        /// an int representing the projectile's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "proj")]
        public int ID { get; set; }

        /// <summary>
        /// a Vector2D representing the projectile's location.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }
        
        /// <summary>
        /// a Vector2D representing the projectile's orientation.
        /// </summary>
        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction { get; set; }

        /// <summary>
        /// a bool representing if the projectile died on this frame (hit a wall or left the bounds of the world).
        /// The server will send the dead projectiles only once
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        public bool IsDead { get; set; }

        /// <summary>
        /// an int representing the ID of the tank that created the projectile. 
        /// You can use this to draw the projectiles with a different color or image for each player.
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; set; }

        public Projectile(int ID, Vector2D location, Vector2D direction, bool died, int owner)
        {
            this.ID = ID;
            this.Location = location;
            this.Direction = direction;
            this.IsDead = died;
            this.Owner = owner;
        }

        public Projectile(int ID, double xLocation, double yLocation, double xDirection, double yDirection, bool died, int owner)
        {
            this.ID = ID;
            this.Location = new Vector2D(xLocation, yLocation);
            this.Direction = new Vector2D(xDirection, yDirection);
            this.IsDead = died;
            this.Owner = owner;
        }

        public Projectile()
        {
        }

    }
}

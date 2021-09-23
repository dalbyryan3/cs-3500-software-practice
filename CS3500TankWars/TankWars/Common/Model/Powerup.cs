// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// This class contains a representation of a TankWars Powerup.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        /// <summary>
        /// an int representing the powerup's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "power")]
        public int ID { get; set; }

        /// <summary>
        /// a Vector2D representing the location of the powerup.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }
        
        /// <summary>
        /// a bool indicating if the powerup "died" (was collected by a player) on this frame. 
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        public bool IsDead { get; set; }

        public Powerup(int ID, Vector2D location, bool isDead)
        {
            this.ID = ID;
            this.Location = location;
            this.IsDead = isDead;
        }

        public Powerup()
        {

        }
    }
}

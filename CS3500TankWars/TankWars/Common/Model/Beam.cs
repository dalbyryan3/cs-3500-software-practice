// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// This class contains a representation of a TankWars Beam.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {

        /// <summary>
        /// "beam" - an int representing the beam's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "beam")]
        public int ID { get; set; }

        /// <summary>
        /// "org" - a Vector2D representing the origin of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "org")]
        public Vector2D Origin { get; set; }

        /// <summary>
        /// "dir" - a Vector2D representing the direction of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction { get; set; }

        /// <summary>
        /// "owner" - an int representing the ID of the tank that fired the beam. 
        /// You can use this to draw the beams with a different color or image for each player.
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; set; }


        public Beam(int ID, Vector2D origin, Vector2D direction, int owner)
        {
            this.ID = ID;
            this.Origin = origin;
            this.Direction = direction;
            this.Owner = owner;
        }

        /// <summary>
        /// model classes need a default constructor in order for the JSON libraries to work.
        /// </summary>
        public Beam()
        {
        }


    }
}

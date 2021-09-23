// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Control commands are how the client will tell the server that it wants to do (moving, firing, etc). 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TankControlCommand
    {

        /// <summary>
        /// "moving" - a string representing whether the player wants to move or not, and the desired direction. 
        /// Possible values are: "none", "up", "left", "down", "right".
        /// </summary>
        [JsonProperty(PropertyName = "moving")]
        public string Moving { get; set; }

        /// <summary>
        /// "fire" - a string representing whether the player wants to fire or not, and the desired type. 
        /// Possible values are: "none", "main", (for a normal projectile) and "alt" (for a beam attack).
        /// </summary>
        [JsonProperty(PropertyName = "fire")]
        public string Fire { get; set; }

        /// <summary>
        /// "tdir" - a Vector2D representing where the player wants to aim their turret. 
        /// Important: This vector must be normalized.
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D TurretDirection { get; set; }

        /// <summary>
        /// model classes need a default constructor in order for the JSON libraries to work.
        /// </summary>
        public TankControlCommand()
        {
            this.Moving = "none";
            this.Fire = "none";
            this.TurretDirection = new Vector2D(1, 0);
            this.TurretDirection.Normalize();
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != typeof(TankControlCommand)) {
                return false;
            }
            TankControlCommand other = obj as TankControlCommand;
            bool isEqual = (this.Moving == other.Moving)
                && (this.Fire == other.Fire) 
                && (this.TurretDirection == other.TurretDirection);
            return isEqual;
        }

        // we won't ever use the hashcode, just overriding this just because of the compiler warning
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }
}

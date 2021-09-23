// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// This class contains a representation of a TankWars Wall.
    /// 
    /// You can assume the following about all walls:
    /// 
    /// - They will always be axis-aligned (purely horizontal or purely vertical, never diagonal). 
    /// 
    /// /// - This means p1 and p2 will have either the same x value or the same y value.
    /// 
    /// - The length between p1 and p2 will always be a multiple of the wall width (50 units).
    /// 
    /// - The endpoints of the wall can be anywhere (not just multiples of 50), as long as the distance 
    /// between them is a multiple of 50.
    /// 
    /// - The order of p1 and p2 is irrelevant (they can be top to bottom, bottom to top, left to right, or right to left).
    /// Walls can overlap and intersect eachother.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {

        /// <summary>
        /// Represents a globally unique identifier (GUID).
        /// https://docs.microsoft.com/en-us/dotnet/api/system.guid?redirectedfrom=MSDN&view=netframework-4.8
        /// 
        /// this.ID = Guid.NewGuid();
        /// or
        /// this.ID = Guid.NewGuid().ToString();
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "wall")]
        public int ID { get; set; }

        /// <summary>
        /// "p1" - a Vector2D representing one endpoint of the wall.
        /// </summary>
        [JsonProperty(PropertyName = "p1")]
        public Vector2D EndPoint1 { get; set; }

        /// <summary>
        /// "p2" - a Vector2D representing the other endpoint of the wall.
        /// </summary>
        [JsonProperty(PropertyName = "p2")]
        public Vector2D EndPoint2 { get; set; }


        public Wall(int ID, double xPos1, double yPos1, double xPos2, double yPos2)
        {
            this.ID = ID;
            this.EndPoint1 = new Vector2D(xPos1, yPos1);
            this.EndPoint2 = new Vector2D(xPos2, yPos2);
        }

        /// <summary>
        /// model classes need a default constructor in order for the JSON libraries to work.
        /// </summary>
        public Wall()
        {
        }

    }
}

// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// This class contains a representation of a TankWars Tank.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {

        /// <summary>
        /// "tank" - an int representing the tank's unique ID.  
        /// 
        /// Represents a globally unique identifier (GUID).
        /// https://docs.microsoft.com/en-us/dotnet/api/system.guid?redirectedfrom=MSDN&view=netframework-4.8
        /// 
        /// this.ID = Guid.NewGuid();
        /// </summary>
        [JsonProperty(PropertyName = "tank")]
        public int ID { get; set; }

        /// <summary>
        /// "name" - a string representing the player's name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string PlayerName { get; set; }

        /// <summary>
        /// "loc" - a Vector2D representing the tank's location. (See below for description of Vector2D).
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        /// <summary>
        /// "bdir" - a Vector2D representing the tank's orientation. 
        /// This will always be an axis-aligned vector (purely horizontal or vertical).
        /// </summary>
        [JsonProperty(PropertyName = "bdir")]
        public Vector2D BodyDirection { get; set; }

        /// <summary>
        /// "tdir" - a Vector2D representing the direction of the tank's turret (where it's aiming). 
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D TurretDirection { get; set; }
        // TODO do we need to set it in the constructor like this?
        //private Vector2D aiming = new Vector2D(0, -1);

        /// <summary>
        /// "score" - an int representing the player's score.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }
        // TODO do we need to set it in the constructor like this?
        //private int score = 0;

        /// <summary>
        /// "hp" - and int representing the hit points of the tank. 
        /// This value ranges from 0 - 3. If it is 0, then this tank is temporarily destroyed, and waiting to respawn.
        /// </summary>
        [JsonProperty(PropertyName = "hp")]
        public int HealthPoints { get; set; }
        // TODO do we need to set it in the constructor like this?
        //private int healthPoints = Constants.MaxHP;

        /// <summary>
        /// "died" - a bool indicating if the tank died on that frame. 
        /// This will only be true on the exact frame in which the tank died. 
        /// You can use this to determine when to start drawing an explosion. 
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        public bool Died { get; set; }
        // TODO do we need to set it in the constructor like this?
        //private bool died = false;

        /// <summary>
        /// "dc" - a bool indicating if the player controlling that tank disconnected on that frame. 
        /// The server will send the tank with this flag set to true only once, 
        /// then it will discontinue sending that tank for the rest of the game. 
        /// You can use this to remove disconnected players from your model.
        /// </summary>
        [JsonProperty(PropertyName = "dc")]
        public bool Disconnected { get; set; }
        // TODO do we need to set it in the constructor like this?
        // private bool disconnected = false;

        /// <summary>
        /// "join" - a bool indicating if the player joined on this frame. 
        /// This will only be true for one frame. 
        /// This field may not be needed, but may be useful for certain additional View related features.
        /// </summary>
        [JsonProperty(PropertyName = "join")]
        public bool Joined { get; set; }

        /// <summary>
        /// how many shots this tank has fired. this is updated by the game logic, and is also used to calculate the
        /// player's accuracy.
        /// </summary>
        [JsonIgnore]
        public int ShotsFired { get; set; }

        /// <summary>
        /// how many shots this tank has successfully hit. this is updated by the game logic, and is also used 
        /// to calculate the player's accuracy.
        /// </summary>
        [JsonIgnore]
        public int ShotsHit { get; set; }


        public Tank(int ID, string playerName,
            double xLocation, double yLocation,
            double xBodyDirection, double yBodyDirection,
            double xTurretDirection, double yTurretDirection,
            int score, int hp, bool died, bool disconnected, bool joined)
        {
            this.ID = ID;
            this.PlayerName = playerName;
            this.Location = new Vector2D(xLocation, yLocation);
            this.BodyDirection = new Vector2D(xBodyDirection, yBodyDirection);
            this.TurretDirection = new Vector2D(xTurretDirection, yTurretDirection);
            this.Score = score;
            this.HealthPoints = hp;
            this.Died = died;
            this.Disconnected = disconnected;
            this.Joined = joined;
            this.ShotsFired = 0;
            this.ShotsHit = 0;
        }

        /// <summary>
        /// model classes need a default constructor in order for the JSON libraries to work.
        /// </summary>
        public Tank()
        {
            // initialize default values for everything (just in case? or is this necessary?)
            ID = Guid.NewGuid().GetHashCode();
            PlayerName = "no name";
            Location = new Vector2D(0, 0);
            BodyDirection = new Vector2D(0, 0);
            TurretDirection = new Vector2D(0, 0);
            Score = 0;
            HealthPoints = 100;  // TODO create a Constants.MaxHP
            Died = false;
            Disconnected = false;
            Joined = false;
            ShotsFired = 0;
            ShotsHit = 0;
        }

        public bool IsHighHealth()
        {
            return HealthPoints == Constants.MaxHP;
        }

        public bool IsMediumHealth()
        {
            return HealthPoints == 2;
        }

        public bool IsLowHealth()
        {
            return HealthPoints == 1;
        }

        public bool IsZeroHealth()
        {
            return HealthPoints == 0;
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. mainly used for testing purposes.
        /// NOTE: Private members are not cloned using this method.
        /// </summary>
        public Tank Clone()
        {
            JsonSerializerSettings deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<Tank>(JsonConvert.SerializeObject(this), deserializeSettings);
        }

    }
}

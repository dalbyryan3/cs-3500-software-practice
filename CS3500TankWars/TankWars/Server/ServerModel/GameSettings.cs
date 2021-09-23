using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace TankWars
{
    public class GameSettings
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // The below items are modifiable settings in the sample server xml file, and must be modifiable in your
        // solution. Use the same names as the sample xml file does.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Universe size - The number of units on each side of the square universe.
        /// </summary>
        public int UniverseSize { get; set; }

        /// <summary>
        /// Time per frame - How often does the server attempt to update the world?
        /// </summary>
        public int MillisecondsPerFrame { get; set; }

        /// <summary>
        /// Projectile firing delay - How many frames must a tank wait between firing projectiles? 
        /// This is not in units of time. It is in units of frames.
        /// </summary>
        public int ProjectileFiringDelay { get; set; }

        /// <summary>
        /// Respawn delay - How many frames must a tank wait before respawning? 
        /// This is not in units of time. It is in units of frames.
        /// </summary>
        public int RespawnDelay { get; set; }

        /// <summary>
        /// Walls - where are the walls (if any)?
        /// </summary>
        public List<Wall> Walls { get; set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // The below items are all hard-coded by the sample server, but a better solution would make them settings in an
        // xml file. This would be fairly trivial to implement.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Starting hit points - How many "hit points" do tanks start with? The default is 3.
        /// </summary>
        public int StartingHealthPoints { get; set; }

        /// <summary>
        /// Projectile speed - How fast do projectiles travel? The default is 25 units per frame.
        /// </summary>
        public int ProjectileSpeed { get; set; }

        /// <summary>
        /// Engine strength - How fast do tanks travel? The default is 2.9 units per frame.
        /// </summary>
        public double TankSpeed { get; set; }

        /// <summary>
        /// Tank size - How much area does a tank occupy? The default is a 60x60 square. 
        /// This is used for the purposes of detecting collisions with walls and projectiles. 
        /// </summary>
        public int TankSize { get; set; }

        /// <summary>
        /// Wall size - How much area does a single segment of a wall occupy? The default is a 50x50 square. 
        /// This is used for detecting collisions with other objects. 
        /// </summary>
        public int WallSize { get; set; }

        /// <summary>
        /// Max powerups - how many powerups can be in the world at any time? The default is 2.
        /// </summary>
        public int MaxPowerups { get; set; }

        /// <summary>
        /// Max powerup delay - what is the maximum delay between spawning new powerups? 
        /// The default is 1650 frames. After spawning a powerup, the server should pick a random number 
        /// of frames less than this number before trying to spawn another.
        /// </summary>
        public int MaxPowerupDelay { get; set; }

        // used for assigning unique IDs to new objects when needed
        private Random random;

        public GameSettings()
        {
            // the default settings are those given in the professor's example server xml file.
            // technically, we should never use default values for the essential settings like universe size. we're
            // required to read those from the xml file. but we'll hardcode default values for these settings
            // anyways just to be safe. 
            UniverseSize = 1200;
            MillisecondsPerFrame = 17;
            ProjectileFiringDelay = 80;
            RespawnDelay = 300;
            Walls = new List<Wall>();
            // these fields all have proper default values
            StartingHealthPoints = 3;
            ProjectileSpeed = 25;
            TankSpeed = 2.9;
            TankSize = 60;
            WallSize = 50;
            MaxPowerups = 2;
            MaxPowerupDelay = 1650;
            random = new Random();
        }

        // returns true if successfully read settings file
        public bool ReadSettingsFile(string filename)
        {
            try {
                using (XmlReader reader = CreateXmlReader(filename)) {
                    while (reader.Read()) {
                        if (reader.IsStartElement()) {
                            ProcessStartElement(reader);
                        } else {
                            ProcessEndElement(reader);
                        }
                    }
                }
                return true;
            } catch (Exception e) {
                // do something?
                throw e;
                return false;
            }
        }

        private void ProcessStartElement(XmlReader reader)
        {
            switch (reader.Name) {
                case "GameSettings":
                    // cool, this is the start tag we need
                    break;
                case "UniverseSize":
                    UniverseSize = int.Parse(reader.ReadString());
                    break;
                case "MSPerFrame":
                    MillisecondsPerFrame = int.Parse(reader.ReadString());
                    break;
                case "FramesPerShot":
                    ProjectileFiringDelay = int.Parse(reader.ReadString());
                    break;
                case "RespawnRate":
                    RespawnDelay = int.Parse(reader.ReadString());
                    break;
                case "Wall":
                    ReadWall(reader);
                    break;
                // TODO the rest of the non-essential options
                case "StartingHealthPoints":
                    StartingHealthPoints = int.Parse(reader.ReadString());
                    break;
                case "ProjectileSpeed":
                    ProjectileSpeed = int.Parse(reader.ReadString());
                    break;
                case "TankSpeed":
                    TankSpeed = double.Parse(reader.ReadString());
                    break;
                case "TankSize":
                    TankSize = int.Parse(reader.ReadString());
                    break;
                case "WallSize":
                    WallSize = int.Parse(reader.ReadString());
                    break;
                case "MaxPowerups":
                    MaxPowerups = int.Parse(reader.ReadString());
                    break;
                case "MaxPowerupDelay":
                    MaxPowerupDelay = int.Parse(reader.ReadString());
                    break;
                default:
                    // unexpected xml. just ignore it i guess. or throw an error.
                    break;
            }
        }

        private void ReadWall(XmlReader reader)
        {
            Wall wall = new Wall();
            wall.ID = random.Next();
            ReadWallEndpoint1(reader, out Vector2D endPoint1);
            ReadWallEndpoint2(reader, out Vector2D endPoint2);
            wall.EndPoint1 = endPoint1;
            wall.EndPoint2 = endPoint2;
            Walls.Add(wall);
        }

        private void ReadWallEndpoint1(XmlReader reader, out Vector2D endPoint)
        {
            endPoint = new Vector2D(0, 0);
            reader.Read();
            switch (reader.Name) {
                case "p1":
                    double xPos1 = ReadWallX(reader);
                    double yPos1 = ReadWallY(reader);
                    endPoint = new Vector2D(xPos1, yPos1);
                    break;
            }
            reader.Read();
        }

        private void ReadWallEndpoint2(XmlReader reader, out Vector2D endPoint)
        {
            endPoint = new Vector2D(0, 0);
            reader.Read();
            switch (reader.Name) {
                case "p2":
                    double xPos2 = ReadWallX(reader);
                    double yPos2 = ReadWallY(reader);
                    endPoint = new Vector2D(xPos2, yPos2);
                    break;
            }
            reader.Read();
        }

        private double ReadWallX(XmlReader reader)
        {
            double x = 0.0;
            reader.Read();
            switch (reader.Name) {
                case "x":
                    string xString = reader.ReadString();
                    x = double.Parse(xString);
                    break;
            }
            return x;
        }

        private double ReadWallY(XmlReader reader)
        {
            double y = 0.0;
            reader.Read();
            switch (reader.Name) {
                case "y":
                    string yString = reader.ReadString();
                    y = double.Parse(yString);
                    break;
            }
            return y;
        }

        private void ProcessEndElement(XmlReader reader)
        {
            // TODO
        }

        private XmlReader CreateXmlReader(string filename)
        {
            // get absolute path of the file (the parameter given is the relative path that looks like "..\..\whatever.xml")
            string fullPath = Path.GetFullPath(filename);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            return XmlReader.Create(fullPath, settings);
        }

    }
}

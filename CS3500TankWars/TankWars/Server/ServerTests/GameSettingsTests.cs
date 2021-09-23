using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TankWars
{
    [TestClass]
    public class GameSettingsTests
    {

        [TestMethod]
        public void Constructor_NoArgs_ShouldUseDefaultsForEverything()
        {
            GameSettings gameSettings = new GameSettings();
            Assert.AreEqual(1200, gameSettings.UniverseSize);
            Assert.AreEqual(17, gameSettings.MillisecondsPerFrame);
            Assert.AreEqual(80, gameSettings.ProjectileFiringDelay);
            Assert.AreEqual(300, gameSettings.RespawnDelay);
            Assert.AreEqual(0, gameSettings.Walls.Count);

            Assert.AreEqual(3, gameSettings.StartingHealthPoints);
            Assert.AreEqual(25, gameSettings.ProjectileSpeed);
            Assert.AreEqual(2.9, gameSettings.TankSpeed);
            Assert.AreEqual(60, gameSettings.TankSize);
            Assert.AreEqual(50, gameSettings.WallSize);
            Assert.AreEqual(2, gameSettings.MaxPowerups);
            Assert.AreEqual(1650, gameSettings.MaxPowerupDelay);
        }

        [TestMethod]
        public void ReadSettingsFile_SampleXmlFile_ShouldReadMainSettingsAndSetRestToDefault()
        {
            string settingsXmlText =
            @"<GameSettings>
                <UniverseSize>1200</UniverseSize>
                <MSPerFrame>17</MSPerFrame>
                <FramesPerShot>80</FramesPerShot>
                <RespawnRate>300</RespawnRate>

                <!-- top border -->
                <Wall>
                    <p1><x>-575</x><y>-575</y></p1>
                    <p2><x>575</x><y>-575</y></p2>
                </Wall>
                <!-- left border -->
                <Wall>
                    <p1><x>-575</x><y>-575</y></p1>
                    <p2><x>-575</x><y>575</y></p2>
                </Wall>
                <!-- right border -->
                <Wall>
                    <p1><x>575</x><y>575</y></p1>
                    <p2><x>575</x><y>-575</y></p2>
                </Wall>
                <!-- bottom border -->
                <Wall>
                    <p1><x>575</x><y>575</y></p1>
                    <p2><x>-575</x><y>575</y></p2>
                </Wall>
                <!-- upper left 'L' piece -->
                <Wall>
                    <p1><x>-325</x><y>-325</y></p1>
                    <p2><x>-175</x><y>-325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>-325</x><y>-325</y></p1>
                    <p2><x>-325</x><y>-175</y></p2>
                </Wall>
                <!-- upper right 'L' piece -->
                <Wall>
                    <p1><x>325</x><y>-325</y></p1>
                    <p2><x>175</x><y>-325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>325</x><y>-325</y></p1>
                    <p2><x>325</x><y>-175</y></p2>
                </Wall>
                <!-- lower right 'L' piece -->
                <Wall>
                    <p1><x>325</x><y>325</y></p1>
                    <p2><x>175</x><y>325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>325</x><y>325</y></p1>
                    <p2><x>325</x><y>175</y></p2>
                </Wall>
                <!-- lower left 'L' piece -->
                <Wall>
                    <p1><x>-325</x><y>325</y></p1>
                    <p2><x>-175</x><y>325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>-325</x><y>325</y></p1>
                    <p2><x>-325</x><y>175</y></p2>
                </Wall>
                <!-- bottom innermost wall -->
                <Wall>
                    <p1><x>-50</x><y>-175</y></p1>
                    <p2><x>50</x><y>-175</y></p2>
                </Wall> 
                <!-- top innermost wall -->
                <Wall>
                    <p1><x>-50</x><y>175</y></p1>
                    <p2><x>50</x><y>175</y></p2>
                </Wall> 
                <!-- left innermost wall -->
                <Wall>
                    <p1><x>-175</x><y>-50</y></p1>
                    <p2><x>-175</x><y>50</y></p2>
                </Wall>   
                <!-- right innermost wall -->
                <Wall>
                    <p1><x>175</x><y>-50</y></p1>
                    <p2><x>175</x><y>50</y></p2>
                </Wall>  

            </GameSettings>
            ";
            System.IO.File.WriteAllText("settings.xml", settingsXmlText);

            GameSettings gameSettings = new GameSettings();
            gameSettings.ReadSettingsFile("settings.xml");
            File.Delete("settings.xml");

            Assert.AreEqual(1200, gameSettings.UniverseSize);
            Assert.AreEqual(17, gameSettings.MillisecondsPerFrame);
            Assert.AreEqual(80, gameSettings.ProjectileFiringDelay);
            Assert.AreEqual(300, gameSettings.RespawnDelay);
            Assert.AreEqual(16, gameSettings.Walls.Count);
        }

        [TestMethod]
        public void ReadSettingsFile_SampleXmlFile_WallsShouldAllBeInTheRightPlace()
        {
            string settingsXmlText =
            @"<GameSettings>
                <UniverseSize>1200</UniverseSize>
                <MSPerFrame>17</MSPerFrame>
                <FramesPerShot>80</FramesPerShot>
                <RespawnRate>300</RespawnRate>

                <!-- top border -->
                <Wall>
                    <p1><x>-575</x><y>-575</y></p1>
                    <p2><x>575</x><y>-575</y></p2>
                </Wall>
                <!-- left border -->
                <Wall>
                    <p1><x>-575</x><y>-575</y></p1>
                    <p2><x>-575</x><y>575</y></p2>
                </Wall>
                <!-- right border -->
                <Wall>
                    <p1><x>575</x><y>575</y></p1>
                    <p2><x>575</x><y>-575</y></p2>
                </Wall>
                <!-- bottom border -->
                <Wall>
                    <p1><x>575</x><y>575</y></p1>
                    <p2><x>-575</x><y>575</y></p2>
                </Wall>
                <!-- upper left 'L' piece -->
                <Wall>
                    <p1><x>-325</x><y>-325</y></p1>
                    <p2><x>-175</x><y>-325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>-325</x><y>-325</y></p1>
                    <p2><x>-325</x><y>-175</y></p2>
                </Wall>
                <!-- upper right 'L' piece -->
                <Wall>
                    <p1><x>325</x><y>-325</y></p1>
                    <p2><x>175</x><y>-325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>325</x><y>-325</y></p1>
                    <p2><x>325</x><y>-175</y></p2>
                </Wall>
                <!-- lower right 'L' piece -->
                <Wall>
                    <p1><x>325</x><y>325</y></p1>
                    <p2><x>175</x><y>325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>325</x><y>325</y></p1>
                    <p2><x>325</x><y>175</y></p2>
                </Wall>
                <!-- lower left 'L' piece -->
                <Wall>
                    <p1><x>-325</x><y>325</y></p1>
                    <p2><x>-175</x><y>325</y></p2>
                </Wall>  
                <Wall>
                    <p1><x>-325</x><y>325</y></p1>
                    <p2><x>-325</x><y>175</y></p2>
                </Wall>
                <!-- bottom innermost wall -->
                <Wall>
                    <p1><x>-50</x><y>-175</y></p1>
                    <p2><x>50</x><y>-175</y></p2>
                </Wall> 
                <!-- top innermost wall -->
                <Wall>
                    <p1><x>-50</x><y>175</y></p1>
                    <p2><x>50</x><y>175</y></p2>
                </Wall> 
                <!-- left innermost wall -->
                <Wall>
                    <p1><x>-175</x><y>-50</y></p1>
                    <p2><x>-175</x><y>50</y></p2>
                </Wall>   
                <!-- right innermost wall -->
                <Wall>
                    <p1><x>175</x><y>-50</y></p1>
                    <p2><x>175</x><y>50</y></p2>
                </Wall>  

            </GameSettings>
            ";
            System.IO.File.WriteAllText("settings.xml", settingsXmlText);
            GameSettings gameSettings = new GameSettings();
            gameSettings.ReadSettingsFile("settings.xml");
            File.Delete("settings.xml");

            Assert.AreEqual(16, gameSettings.Walls.Count);
            // top border
            Wall topBorder = gameSettings.Walls[0];
            Assert.AreEqual(-575.0, topBorder.EndPoint1.GetX());
            Assert.AreEqual(-575.0, topBorder.EndPoint1.GetY());
            Assert.AreEqual(575.0, topBorder.EndPoint2.GetX());
            Assert.AreEqual(-575.0, topBorder.EndPoint2.GetY());
            // left border
            Wall leftBorder = gameSettings.Walls[1];
            Assert.AreEqual(-575.0, leftBorder.EndPoint1.GetX());
            Assert.AreEqual(-575.0, leftBorder.EndPoint1.GetY());
            Assert.AreEqual(-575.0, leftBorder.EndPoint2.GetX());
            Assert.AreEqual(575.0, leftBorder.EndPoint2.GetY());
            // right border
            Wall rightBorder = gameSettings.Walls[2];
            Assert.AreEqual(575.0, rightBorder.EndPoint1.GetX());
            Assert.AreEqual(575.0, rightBorder.EndPoint1.GetY());
            Assert.AreEqual(575.0, rightBorder.EndPoint2.GetX());
            Assert.AreEqual(-575.0, rightBorder.EndPoint2.GetY());
            // ... and so on ...
            Wall rightInnermostWall = gameSettings.Walls[15];
            Assert.AreEqual(175, rightInnermostWall.EndPoint1.GetX());
            Assert.AreEqual(-50, rightInnermostWall.EndPoint1.GetY());
            Assert.AreEqual(175, rightInnermostWall.EndPoint2.GetX());
            Assert.AreEqual(50, rightInnermostWall.EndPoint2.GetY());
        }

        [TestMethod]
        public void ReadSettingsFile_CustomXmlFile_ShouldReadMainSettingsAndSetRestToDefault()
        {
            string settingsXmlText =
            @"<GameSettings>
                <UniverseSize>420</UniverseSize>
                <MSPerFrame>69</MSPerFrame>
                <FramesPerShot>420</FramesPerShot>
                <RespawnRate>69</RespawnRate>

                <Wall>
                    <p1><x>0</x><y>0</y></p1>
                    <p2><x>500</x><y>500</y></p2>
                </Wall>
                <Wall>
                    <p1><x>-100</x><y>-0</y></p1>
                    <p2><x>100</x><y>0</y></p2>
                </Wall>

            </GameSettings>
            ";
            System.IO.File.WriteAllText("settings.xml", settingsXmlText);

            GameSettings gameSettings = new GameSettings();
            gameSettings.ReadSettingsFile("settings.xml");
            File.Delete("settings.xml");

            Assert.AreEqual(420, gameSettings.UniverseSize);
            Assert.AreEqual(69, gameSettings.MillisecondsPerFrame);
            Assert.AreEqual(420, gameSettings.ProjectileFiringDelay);
            Assert.AreEqual(69, gameSettings.RespawnDelay);
            Assert.AreEqual(2, gameSettings.Walls.Count);
        }

        [TestMethod]
        public void ReadSettingsFile_CustomXmlFileWithFullSettings_ShouldReadAllSettings()
        {
            string settingsXmlText =
            @"<GameSettings>
                <UniverseSize>420</UniverseSize>
                <MSPerFrame>69</MSPerFrame>
                <FramesPerShot>420</FramesPerShot>
                <RespawnRate>69</RespawnRate>

                <Wall>
                    <p1><x>0</x><y>0</y></p1>
                    <p2><x>500</x><y>500</y></p2>
                </Wall>
                <Wall>
                    <p1><x>-100</x><y>-0</y></p1>
                    <p2><x>100</x><y>0</y></p2>
                </Wall>

                <StartingHealthPoints>10</StartingHealthPoints>
                <ProjectileSpeed>1</ProjectileSpeed>
                <TankSpeed>69.69</TankSpeed>
                <TankSize>69</TankSize>
                <WallSize>59</WallSize>
                <MaxPowerups>999</MaxPowerups>
                <MaxPowerupDelay>420</MaxPowerupDelay>

            </GameSettings>
            ";
            System.IO.File.WriteAllText("settings.xml", settingsXmlText);

            GameSettings gameSettings = new GameSettings();
            gameSettings.ReadSettingsFile("settings.xml");
            File.Delete("settings.xml");

            Assert.AreEqual(420, gameSettings.UniverseSize);
            Assert.AreEqual(69, gameSettings.MillisecondsPerFrame);
            Assert.AreEqual(420, gameSettings.ProjectileFiringDelay);
            Assert.AreEqual(69, gameSettings.RespawnDelay);
            Assert.AreEqual(2, gameSettings.Walls.Count);

            Assert.AreEqual(10, gameSettings.StartingHealthPoints);
            Assert.AreEqual(1, gameSettings.ProjectileSpeed);
            Assert.AreEqual(69.69, gameSettings.TankSpeed);
            Assert.AreEqual(69, gameSettings.TankSize);
            Assert.AreEqual(59, gameSettings.WallSize);
            Assert.AreEqual(999, gameSettings.MaxPowerups);
            Assert.AreEqual(420, gameSettings.MaxPowerupDelay);
        }

    }
}

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TankWars
{
    [TestClass]
    public class ServerGameControllerTests
    {

        // [TestMethod]
        // public void CreateNewTankForPlayer_Normal_ShouldReturnNewTankWithInitializedValues()
        // {
        //     World gameWorld = new World();
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     GameControlState gameControlState = new GameControlState();
        //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);
        //     Tank tank = gameController.CreateTankForPlayer("player");
        //     Assert.IsTrue(tank.ID != 0);
        //     Assert.AreEqual("player", tank.PlayerName);
        //     Assert.IsTrue(tank.Location.GetX() >= -(gameSettings.UniverseSize / 2) && tank.Location.GetX() <= (gameSettings.UniverseSize / 2));
        //     Assert.IsTrue(tank.Location.GetY() >= -(gameSettings.UniverseSize / 2) && tank.Location.GetY() <= (gameSettings.UniverseSize / 2));
        //     Assert.AreEqual(1, tank.BodyDirection.GetX());
        //     Assert.AreEqual(0, tank.BodyDirection.GetY());
        //     Assert.AreEqual(1, tank.TurretDirection.GetX());
        //     Assert.AreEqual(0, tank.TurretDirection.GetY());
        //     Assert.AreEqual(0, tank.Score);
        //     Assert.AreEqual(gameSettings.StartingHealthPoints, tank.HealthPoints);
        //     Assert.IsFalse(tank.Died);
        //     Assert.IsFalse(tank.Disconnected);
        //     Assert.IsTrue(tank.Joined);
        // }

        // [TestMethod]
        // public void AddTankToWorld_NewTankForPlayer_ShouldAddTankToWorld()
        // {
        //     World gameWorld = new World();
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     GameControlState gameControlState = new GameControlState();
        //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);
        //     Assert.AreEqual(0, gameWorld.Tanks.Count);
        //     Tank tank = gameController.CreateTankForPlayer("player");
        //     gameController.AddTankToWorld(tank);
        //     Assert.AreEqual(1, gameWorld.Tanks.Values.Count);
        // }

        // // [TestMethod]
        // // public void AddDummyObjectsToWorld_Random_ShouldAddRandomObjectsToWorld()
        // // {
        // //     World gameWorld = new World();
        // //     GameSettings gameSettings = new GameSettings();
        // //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        // //     GameControlState gameControlState = new GameControlState();
        // //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);
        // //     gameController.AddDummyWallsToWorld();
        // //     gameController.AddDummyTanksToWorld();
        // //     gameController.AddDummyProjectilesToWorld();
        // //     Assert.AreEqual(5, gameWorld.Walls.Count);
        // //     Assert.AreEqual(5, gameWorld.Tanks.Count);
        // //     Assert.AreEqual(5, gameWorld.Projectiles.Count);
        // // }

        // [TestMethod]
        // public void SerializeGameWorld_EmptyWorld_ShouldReturnEmptyString()
        // {
        //     GameController gameController = new GameController();
        //     string actual = gameController.SerializeGameWorld();
        //     Assert.AreEqual("", actual);
        // }

        // [TestMethod]
        // public void SerializeGameWorld_AFewObjects_ShouldReturnStringWithSerializedObjectsAndNewlines()
        // {
        //     GameController gameController = new GameController();

        //     Wall wall = new Wall(1, -575.0, -575.0, -575.0, 575.0);
        //     gameController.AddWallToWorld(wall);
        //     Tank tank = new Tank(0, "Danny", 220.995264, -253.63331367, 1.0, 0.0, -0.795849908004867, 0.60549395036502607, 3, 0, false, false, false);
        //     gameController.AddTankToWorld(tank);
        //     Projectile projectile = new Projectile(5, -279.01316584757348, -226.5850218601696, -0.99983624342305211, 0.018096583591367461, false, 0);
        //     gameController.AddProjectileToWorld(projectile);

        //     string wallJson = JsonConvert.SerializeObject(wall) + "\n";
        //     string tankJson = JsonConvert.SerializeObject(tank) + "\n";
        //     string projectileJson = JsonConvert.SerializeObject(projectile) + "\n";
        //     string expectedMessage = wallJson + tankJson + projectileJson;

        //     string actualMessage = gameController.SerializeGameWorld();
        //     Assert.AreEqual(expectedMessage.Length, actualMessage.Length);
        //     Assert.AreEqual(expectedMessage, actualMessage);
        // }

        // // [TestMethod]
        // // public void SerializeGameWorld_WithDummyObjects_ShouldReturnStringWithSerializedObjectsAndNewlines()
        // // {
        // //     GameController gameController = new GameController();
        // //     gameController.AddDummyWallsToWorld();
        // //     gameController.AddDummyTanksToWorld();
        // //     gameController.AddDummyProjectilesToWorld();
        // //     // we could check for each string individually but that's a waste of effort.
        // //     // just make sure that the right number of messages were serialized and we should be fine.
        // //     int expectedNumMessages = 15;  // 5 walls, 5 tanks, 5 projectiles
        // //     string actualMessage = gameController.SerializeGameWorld();
        // //     int actualNumMessages = actualMessage.Split('\n').Length - 1;  // -1 because the last message also has a newline
        // //     Assert.AreEqual(expectedNumMessages, actualNumMessages);
        // // }

        // [TestMethod]
        // public void UpdateWorld_TankIsDrivingIntoAWall_ShouldNotChangeTankPosition()
        // {
        //     // --- assemble ---
        //     World gameWorld = new World();
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     GameControlState gameControlState = new GameControlState();
        //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);
        //     gameSettings.TankSize = 60;
        //     gameSettings.WallSize = 50;
        //     Tank tank = new Tank();
        //     Wall wall = new Wall();
        //     // the tank's left side will be at (-30, 0)
        //     tank.Location = new Vector2D(0, 0);
        //     // the wall's right side will be at X = -30
        //     wall.EndPoint1 = new Vector2D(-55, -100);
        //     wall.EndPoint2 = new Vector2D(-55, 100);
        //     gameController.AddTankToWorld(tank);
        //     gameController.AddWallToWorld(wall);
        //     // the player is trying to move to the left (which would cause a collision into the wall)
        //     TankControlCommand controlCommand = new TankControlCommand();
        //     controlCommand.Moving = "left";
        //     gameControlState.AddNewPlayer(0, 0);
        //     gameControlState.TryUpdateTankControlCommandByClientID(0, controlCommand);

        //     // --- act ---
        //     Vector2D originalLocation = tank.Location.Clone(); 
        //     gameController.UpdateWorld();
        //     Vector2D updatedLocation = gameWorld.Tanks[tank.ID].Location;

        //     // --- assert ---
        //     Assert.AreEqual(originalLocation, updatedLocation);
        // }

        // [TestMethod]
        // public void UpdateWorld_TankIsDrivingNextToWallButNotIntoIt_ShouldChangeTankPosition()
        // {
        //     // --- assemble ---
        //     World gameWorld = new World();
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     GameControlState gameControlState = new GameControlState();
        //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);
        //     gameSettings.TankSize = 60;
        //     gameSettings.WallSize = 50;
        //     Tank tank = new Tank();
        //     Wall wall = new Wall();
        //     // the tank's left side will be at (-30, 0)
        //     tank.Location = new Vector2D(0, 0);
        //     // the wall's right side will be at X = -30
        //     wall.EndPoint1 = new Vector2D(-55, -100);
        //     wall.EndPoint2 = new Vector2D(-55, 100);
        //     gameController.AddTankToWorld(tank);
        //     gameController.AddWallToWorld(wall);
        //     // the player is trying to move to the left (which would cause a collision into the wall)
        //     TankControlCommand controlCommand = new TankControlCommand();
        //     controlCommand.Moving = "up";
        //     gameControlState.AddNewPlayer(0, 0);
        //     gameControlState.TryUpdateTankControlCommandByClientID(0, controlCommand);

        //     // --- act ---
        //     Vector2D originalLocation = tank.Location.Clone();
        //     gamePhysics.UpdateTankBody(tank, controlCommand);

        //     gameController.UpdateWorld();
        //     Vector2D updatedLocation = gameWorld.Tanks[tank.ID].Location;

        //     // --- assert ---
        //     Assert.AreNotEqual(originalLocation, updatedLocation);
        // }

        // [TestMethod]
        // public void UpdateWorld_PlayerIsNotMoving_ShouldNotMoveTank()
        // {
        //     // --- assemble ---
        //     World gameWorld = new World();
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     GameControlState gameControlState = new GameControlState();
        //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);
        //     Tank tank = new Tank();
        //     gameController.AddTankToWorld(tank);
        //     TankControlCommand controlCommand = new TankControlCommand();
        //     controlCommand.Moving = "none";
        //     gameControlState.AddNewPlayer(0, 0);
        //     gameControlState.TryUpdateTankControlCommandByClientID(0, controlCommand);

        //     // --- act ---
        //     Vector2D originalLocation = tank.Location.Clone();
        //     gameController.UpdateWorld();
        //     Vector2D updatedLocation = gameWorld.Tanks[tank.ID].Location;

        //     // --- assert ---
        //     Assert.AreEqual(originalLocation, updatedLocation);
        // }

        // [TestMethod]
        // public void UpdateWorld_ProjectileHitsTank_ShouldDecreaseTankHealth()
        // {
        //     // --- assemble ---
        //     World gameWorld = new World();
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     GameControlState gameControlState = new GameControlState();
        //     GameController gameController = new GameController(gameWorld, gamePhysics, gameSettings, gameControlState);

        //     Tank tank = new Tank();
        //     // the tank's right side is at (30, 0)
        //     tank.Location = new Vector2D(0, 0);
        //     Projectile projectile = new Projectile();
        //     // this projectile has hit the tank's right side
        //     projectile.Location = new Vector2D(29, 0);
        //     projectile.Direction =  new Vector2D(-1, 0);

        //     gameController.AddTankToWorld(tank);
        //     gameController.AddProjectileToWorld(projectile);

        //     int originalHealth = tank.HealthPoints;
        //     gameController.UpdateWorld();
        //     int updatedHealth = tank.HealthPoints;
        //     Assert.IsTrue(updatedHealth == (originalHealth - 1));
        // }



    }
}
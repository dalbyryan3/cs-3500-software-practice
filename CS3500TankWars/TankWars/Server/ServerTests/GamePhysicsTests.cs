using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TankWars
{
    [TestClass]
    public class GamePhysicsTests
    {

        // [TestMethod]
        // public void UpdateTankBody_MovementIsNone_TankLocationShouldNotChange()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     TankControlCommand controlCommand = new TankControlCommand();
        //     controlCommand.Moving = "none";
        //     Tank tank = new Tank();
        //     Vector2D originalLocation = tank.Location;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     Vector2D updatedLocation = tank.Location;
        //     Assert.AreEqual(originalLocation, updatedLocation);
        // }

        // [TestMethod]
        // public void UpdateTankBody_MovementIsUpDownLeftRight_TankLocationShouldChangeAccordingly()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     TankControlCommand controlCommand = new TankControlCommand();
        //     controlCommand.Moving = "up";
        //     Tank tank = new Tank();
        //     Vector2D originalLocation = tank.Location;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     Vector2D updatedLocation = tank.Location;
        //     Assert.IsTrue(updatedLocation.GetY() < originalLocation.GetY());

        //     controlCommand.Moving = "down";
        //     originalLocation = updatedLocation;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     updatedLocation = tank.Location;
        //     Assert.IsTrue(updatedLocation.GetY() > originalLocation.GetY());

        //     controlCommand.Moving = "left";
        //     originalLocation = updatedLocation;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     updatedLocation = tank.Location;
        //     Assert.IsTrue(updatedLocation.GetX() < originalLocation.GetX());

        //     controlCommand.Moving = "right";
        //     originalLocation = updatedLocation;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     updatedLocation = tank.Location;
        //     Assert.IsTrue(updatedLocation.GetX() > originalLocation.GetX());
        // }

        // [TestMethod]
        // public void UpdateTankBody_AlteredGameSettingsTankSpeed_TankLocationShouldChangeBasedUponNewTankSpeed()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     TankControlCommand controlCommand = new TankControlCommand();
        //     controlCommand.Moving = "right";
        //     Tank tank = new Tank();
        //     Vector2D originalLocation = tank.Location;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     Vector2D updatedLocation = tank.Location;
        //     double distanceTravelled = updatedLocation.GetX() - originalLocation.GetX();
        //     Assert.AreEqual(gameSettings.TankSpeed, distanceTravelled);
        //     // now we'll change the tank speed and check again
        //     gameSettings.TankSpeed = 100;
        //     originalLocation = updatedLocation;
        //     gamePhysics.UpdateTankBody(tank, controlCommand);
        //     updatedLocation = tank.Location;
        //     distanceTravelled = updatedLocation.GetX() - originalLocation.GetX();
        //     Assert.AreEqual(gameSettings.TankSpeed, distanceTravelled);
        // }

        // [TestMethod]
        // public void IsTankWallCollision_TankIsInMiddleOfWall_ShouldReturnTrue()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     Wall wall = new Wall();
        //     tank.Location = new Vector2D(-575, 0);
        //     wall.EndPoint1 = new Vector2D(-575, -575);
        //     wall.EndPoint2 = new Vector2D(-575, 575);
        //     bool isCollision = gamePhysics.IsTankWallCollision(tank, wall);
        //     Assert.IsTrue(isCollision);
        // }

        // [TestMethod]
        // public void IsTankWallCollision_NoCollision_ShouldReturnFalse()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     Wall wall = new Wall();
        //     tank.Location = new Vector2D(0, 0);
        //     wall.EndPoint1 = new Vector2D(-575, -575);
        //     wall.EndPoint2 = new Vector2D(-575, 575);
        //     bool isCollision = gamePhysics.IsTankWallCollision(tank, wall);
        //     Assert.IsFalse(isCollision);
        // }

        // [TestMethod]
        // public void IsTankWallCollision_BoundsAreSame_ShouldReturnFalse()
        // {      
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     Wall wall = new Wall();
        //     gameSettings.TankSize = 60;
        //     gameSettings.WallSize = 50;
        //     // the tank's left side will be at (-30, 0)
        //     tank.Location = new Vector2D(0, 0);
        //     // the wall's right side will be at X = -30
        //     wall.EndPoint1 = new Vector2D(-55, -100);
        //     wall.EndPoint2 = new Vector2D(-55, 100);
        //     bool isCollision = gamePhysics.IsTankWallCollision(tank, wall);
        //     Assert.IsFalse(isCollision);
        // }

        // [TestMethod]
        // public void IsTankWallCollision_BoundsAreOneUnitPast_ShouldReturnTrue()
        // {      
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     Wall wall = new Wall();
        //     gameSettings.TankSize = 60;
        //     gameSettings.WallSize = 50;
        //     // the tank's left side will be at (-31, 0)
        //     tank.Location = new Vector2D(-1, 0);
        //     // the wall's right side will be at X = -30
        //     wall.EndPoint1 = new Vector2D(-55, -100);
        //     wall.EndPoint2 = new Vector2D(-55, 100);
        //     bool isCollision = gamePhysics.IsTankWallCollision(tank, wall);
        //     Assert.IsTrue(isCollision);
        // }


        // [TestMethod]
        // public void IsTankWallCollision_AllFourBoundsAreColliding_ShouldReturnTrue()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     gameSettings.TankSize = 60;
        //     gameSettings.WallSize = 50;

        //     tank.Location = new Vector2D(-1, 0);
        //     Wall leftWall = new Wall();
        //     leftWall.EndPoint1 = new Vector2D(-55, -100);
        //     leftWall.EndPoint2 = new Vector2D(-55, 100);
        //     bool isCollision = gamePhysics.IsTankWallCollision(tank, leftWall);
        //     Assert.IsTrue(isCollision);

        //     tank.Location = new Vector2D(1, 0);
        //     Wall rightWall = new Wall();
        //     rightWall.EndPoint1 = new Vector2D(55, -100);
        //     rightWall.EndPoint2 = new Vector2D(55, 100);
        //     gamePhysics.IsTankWallCollision(tank, leftWall);
        //     Assert.IsTrue(isCollision);

        //     tank.Location = new Vector2D(0, -1);
        //     Wall topWall = new Wall();
        //     topWall.EndPoint1 = new Vector2D(-100, -55);
        //     topWall.EndPoint2 = new Vector2D(100, -55);
        //     gamePhysics.IsTankWallCollision(tank, leftWall);
        //     Assert.IsTrue(isCollision);

        //     tank.Location = new Vector2D(0, 1);
        //     Wall bottomWall = new Wall();
        //     bottomWall.EndPoint1 = new Vector2D(-100, 55);
        //     bottomWall.EndPoint2 = new Vector2D(100, 55);
        //     gamePhysics.IsTankWallCollision(tank, leftWall);
        //     Assert.IsTrue(isCollision);
        // }

        // [TestMethod]
        // public void IsProjectileTankCollision_ProjectileIsFarAway_ShouldReturnFalse()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     tank.Location = new Vector2D(0, 0);
        //     Projectile projectile = new Projectile();
        //     projectile.Location = new Vector2D(500, 500);
        //     bool isCollision = gamePhysics.IsProjectileTankCollision(projectile, tank);
        //     Assert.IsFalse(isCollision);
        // }

        // [TestMethod]
        // public void IsProjectileTankCollision_SameBounds_ShouldReturnFalseBecauseTheyreNotActuallyCollidingYet()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     // the tank's right side is at (30, 0)
        //     tank.Location = new Vector2D(0, 0);
        //     Projectile projectile = new Projectile();
        //     // a projectile has a size of 1, so its left side will be at 30 (30.5 - (size/2))
        //     projectile.Location = new Vector2D(30.5, 0);
        //     bool isCollision = gamePhysics.IsProjectileTankCollision(projectile, tank);
        //     Assert.IsFalse(isCollision);
        // }

        // [TestMethod]
        // public void IsProjectileTankCollision_BoundsAreOneUnitPast_ShouldReturnTrue()
        // {
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     // the tank's right side is at (30, 0)
        //     tank.Location = new Vector2D(0, 0);
        //     Projectile projectile = new Projectile();
        //     // this projectile has hit the tank's right side
        //     projectile.Location = new Vector2D(29, 0);
        //     bool isCollision = gamePhysics.IsProjectileTankCollision(projectile, tank);
        //     Assert.IsTrue(isCollision);
        // }

        // [TestMethod]
        // public void IsProjectileTankCollision_ProjectileIsInsideTank_ShouldReturnTrue()
        // {
        //     // this is a weird situation that shouldn't happen but we'll detect it anyways just in case
        //     GameSettings gameSettings = new GameSettings();
        //     GamePhysics gamePhysics = new GamePhysics(gameSettings);
        //     Tank tank = new Tank();
        //     tank.Location = new Vector2D(0, 0);
        //     Projectile projectile = new Projectile();
        //     projectile.Location = new Vector2D(0, 0);
        //     bool isCollision = gamePhysics.IsProjectileTankCollision(projectile, tank);
        //     Assert.IsTrue(isCollision);
        // }








    }
}

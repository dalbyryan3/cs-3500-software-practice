using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TankWars
{
    [TestClass]
    public class GameControlStateTests
    {

        // [TestMethod]
        // public void TryUpdateTankControlCommandByClientID_PlayerExists_ShouldUpdateTankControlCommand()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     gameControlState.AddNewPlayer(420, 0);
        //     TankControlCommand tankControlCommand;
        //     gameControlState.TryGetTankControlCommandByTankID(0, out tankControlCommand);
        //     Assert.AreEqual("none", tankControlCommand.Moving);
        //     TankControlCommand updatedTankControlCommand = new TankControlCommand();
        //     updatedTankControlCommand.Moving = "up";
        //     gameControlState.TryUpdateTankControlCommandByClientID(420, updatedTankControlCommand);
        //     gameControlState.TryGetTankControlCommandByTankID(0, out tankControlCommand);
        //     Assert.AreEqual("up", tankControlCommand.Moving);
        // }

        // [TestMethod]
        // public void TryUpdateTankControlCommandByClientID_ControlCommandObjectIsNull_ShouldReturnFalseAndShouldNotThrowException()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     bool success = gameControlState.TryUpdateTankControlCommandByClientID(420, null);
        //     Assert.IsFalse(success);
        // }


        // [TestMethod]
        // public void TryGetTankControlCommandByTankID_PlayerHasntJoinedYet_ShouldReturnFalseAndNull()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     bool success = gameControlState.TryGetTankControlCommandByTankID(0, out TankControlCommand tankControlCommand);
        //     Assert.IsFalse(success);
        //     Assert.IsNull(tankControlCommand);
        // }

        // [TestMethod]
        // public void TryGetTankControlCommandByTankID_NewClientJustConnected_ControlStatesShouldAllBeDefault()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     gameControlState.AddNewPlayer(420, 0);
        //     bool success = gameControlState.TryGetTankControlCommandByTankID(0, out TankControlCommand tankControlCommand);
        //     Assert.IsTrue(success);
        //     Assert.IsNotNull(tankControlCommand);
        // }

        // [TestMethod]
        // public void TryGetTankControlCommandByTankID_UpdatedMultipleTimes_ShouldReturnLatestControlCommand()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     gameControlState.AddNewPlayer(420, 0);
        //     TankControlCommand command1 = new TankControlCommand();
        //     gameControlState.TryUpdateTankControlCommandByClientID(420, command1);
        //     TankControlCommand command2 = new TankControlCommand();
        //     command2.Moving = "down";
        //     command2.Fire = "alt";
        //     gameControlState.TryUpdateTankControlCommandByClientID(420, command2);
        //     TankControlCommand command3 = new TankControlCommand();
        //     command3.Moving = "up";
        //     command3.Fire = "main";
        //     command3.TurretDirection = new Vector2D(0.5, -0.5);
        //     gameControlState.TryUpdateTankControlCommandByClientID(420, command3);
        //     bool success = gameControlState.TryGetTankControlCommandByTankID(0, out TankControlCommand actualCommand);
        //     Assert.IsTrue(success);
        //     Assert.AreEqual(command3, actualCommand);
        // }

        // [TestMethod]
        // public void TryGetTankControlCommandByTankID_PlayerDisconnected_ShouldReturnFalseAndNull()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     gameControlState.AddNewPlayer(420, 0);
        //     bool success;
        //     TankControlCommand expectedCommand = new TankControlCommand();
        //     TankControlCommand actualCommand;
        //     success = gameControlState.TryGetTankControlCommandByTankID(0, out actualCommand);
        //     Assert.IsTrue(success);
        //     Assert.AreEqual(expectedCommand, actualCommand);
        //     gameControlState.RemoveDisconnectedPlayer(420);
        //     success = gameControlState.TryGetTankControlCommandByTankID(0, out actualCommand);
        //     Assert.IsFalse(success);
        //     Assert.IsNull(actualCommand);
        // }

        // [TestMethod]
        // public void RemoveDisconnectedPlayer_NoPlayersAddedYet_ShouldDoNothing()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     gameControlState.RemoveDisconnectedPlayer(420);
        //     bool success = gameControlState.TryGetTankControlCommandByTankID(0, out TankControlCommand tankControlCommand);
        //     Assert.IsFalse(success);
        //     Assert.IsNull(tankControlCommand);
        // }

        // [TestMethod]
        // public void RemoveDisconnectedPlayer_MultiplePlayersExist_ShouldRemoveThatOnePlayerOnly()
        // {
        //     GameControlState gameControlState = new GameControlState();
        //     gameControlState.AddNewPlayer(0, 0);
        //     gameControlState.AddNewPlayer(1, 1);
        //     gameControlState.AddNewPlayer(2, 2);
        //     bool success;
        //     TankControlCommand tankControlCommand;  // can ignore this
        //     success = gameControlState.TryGetTankControlCommandByTankID(0, out tankControlCommand);
        //     Assert.IsTrue(success);
        //     success = gameControlState.TryGetTankControlCommandByTankID(1, out tankControlCommand);
        //     Assert.IsTrue(success);
        //     success = gameControlState.TryGetTankControlCommandByTankID(2, out tankControlCommand);
        //     Assert.IsTrue(success);
        //     gameControlState.RemoveDisconnectedPlayer(1);
        //     success = gameControlState.TryGetTankControlCommandByTankID(0, out tankControlCommand);
        //     Assert.IsTrue(success);
        //     success = gameControlState.TryGetTankControlCommandByTankID(1, out tankControlCommand);
        //     Assert.IsFalse(success);  // this is the one we deleted
        //     success = gameControlState.TryGetTankControlCommandByTankID(2, out tankControlCommand);
        //     Assert.IsTrue(success);
        // }





    }

}

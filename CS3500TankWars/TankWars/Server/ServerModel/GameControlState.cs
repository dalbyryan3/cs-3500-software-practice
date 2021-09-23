using System;
using System.Collections.Generic;

namespace TankWars
{
    public class GameControlState
    {

        private Dictionary<int, TankControlCommand> controlCommands;

        public GameControlState()
        {
            controlCommands = new Dictionary<int, TankControlCommand>();
        }

        public void AddNewPlayer(int tankID)
        {
            controlCommands[tankID] = new TankControlCommand();
        }

        public void RemoveDisconnectedPlayer(int tankID)
        {
            controlCommands.Remove(tankID);
        }
        public void UpdateTankControlCommand(int tankID, TankControlCommand tankControlCommand)
        {
            controlCommands[tankID] = tankControlCommand;
        }

        public bool TryGetTankControlCommandByTankID(int tankID, out TankControlCommand tankControlCommand)
        {
            return controlCommands.TryGetValue(tankID, out tankControlCommand);
        }


    }
}
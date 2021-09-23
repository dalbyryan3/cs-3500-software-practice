// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankWars
{
    /// <summary>
    /// this class assigns unique colors for each player that joins the game. 
    /// the first 8 players all get a unique color. after 8 players, the colors will be reused.
    /// colors are chosen randomly instead of iteratively, because that way you can
    /// get a new tank color every time you start up the game, which is fun.
    /// 
    /// note that this is part of the tank wars client, not the server,
    /// meaning that every client will see tanks as different colors because they're assigned randomly.
    /// </summary>
    public class PlayerColorManager
    {

        private Dictionary<int, PlayerColor> playerColorAssignments;

        public PlayerColorManager()
        {
            playerColorAssignments = new Dictionary<int, PlayerColor>();
        }

        public PlayerColor GetPlayerColorByID(int playerID)
        {
            if (!PlayerHasAssignedColor(playerID)) {
                AssignColorToPlayer(playerID);
            }
            return playerColorAssignments[playerID];
        }

        private bool PlayerHasAssignedColor(int playerID)
        {
            return playerColorAssignments.ContainsKey(playerID);
        }

        private void AssignColorToPlayer(int playerID)
        {
            PlayerColor newPlayersColor = ChooseRandomColor();
            if (ColorIsTaken(newPlayersColor)) {
                if (AllColorsAreTaken()) {
                    // all colors are taken, so it's okay just use this color
                } else {
                    newPlayersColor = GetNextAvailableColor();
                } 
            }
            playerColorAssignments[playerID] = newPlayersColor;
        }

        private PlayerColor GetNextAvailableColor()
        {
            foreach (PlayerColor color in (PlayerColor[])Enum.GetValues(typeof(PlayerColor))) {
                if (!ColorIsTaken(color)) {
                    return color;
                }
            }
            // this should never happen because you need to check if all colors are taken before calling this method
            return ChooseRandomColor();
        }

        private PlayerColor ChooseRandomColor()
        {
            Array colorValues = Enum.GetValues(typeof(PlayerColor));
            Random random = new Random();
            int randomIndex = random.Next(colorValues.Length);
            PlayerColor randomColor = (PlayerColor)colorValues.GetValue(randomIndex);
            return randomColor;
        }

        private bool ColorIsTaken(PlayerColor color)
        {
            return playerColorAssignments.Values.Contains(color);
        }

        private bool AllColorsAreTaken()
        {
            bool allColorsAreTaken = true;
            foreach (PlayerColor color in (PlayerColor[])Enum.GetValues(typeof(PlayerColor))) {
                if (!ColorIsTaken(color)) {
                    allColorsAreTaken = false;
                    break;
                }
            }
            return allColorsAreTaken;
        }

    }
}

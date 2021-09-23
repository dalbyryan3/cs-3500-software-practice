// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TankWars
{
    /// <summary>
    /// This class looks up the images for the turrets. 
    /// </summary>
    public static class TurretImageLookup
    {

        private static readonly Dictionary<PlayerColor, Image> turretImagesColorMap = LoadTurretImagesColorMap();

        public static Image GetTurretImageByColor(PlayerColor color)
        {
            return turretImagesColorMap[color];
        }

        private static Dictionary<PlayerColor, Image> LoadTurretImagesColorMap()
        {
            Dictionary<PlayerColor, Image> dic = new Dictionary<PlayerColor, Image>();
            dic[PlayerColor.Blue] = DrawingImages.BlueTurret;
            dic[PlayerColor.DarkBlue] = DrawingImages.DarkBlueTurret;
            dic[PlayerColor.Green] = DrawingImages.GreenTurret;
            dic[PlayerColor.LightGreen] = DrawingImages.LightGreenTurret;
            dic[PlayerColor.Orange] = DrawingImages.OrangeTurret;
            dic[PlayerColor.Purple] = DrawingImages.PurpleTurret;
            dic[PlayerColor.Red] = DrawingImages.RedTurret;
            dic[PlayerColor.Yellow] = DrawingImages.YellowTurret;
            return dic;
        }

    }
}


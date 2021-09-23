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
    /// This class looks up the images for the tank bodies.
    /// </summary>
    public static class TankImageLookup
    {

        private static readonly Dictionary<PlayerColor, Image> tankImagesColorMap = LoadTankImagesColorMap();

        public static Image GetTankImageByColor(PlayerColor color)
        {
            return tankImagesColorMap[color];
        }

        private static Dictionary<PlayerColor, Image> LoadTankImagesColorMap()
        {
            Dictionary<PlayerColor, Image> dic = new Dictionary<PlayerColor, Image>();
            dic[PlayerColor.Blue] = DrawingImages.BlueTank;
            dic[PlayerColor.DarkBlue] = DrawingImages.DarkBlueTank;
            dic[PlayerColor.Green] = DrawingImages.GreenTank;
            dic[PlayerColor.LightGreen] = DrawingImages.LightGreenTank;
            dic[PlayerColor.Orange] = DrawingImages.OrangeTank;
            dic[PlayerColor.Purple] = DrawingImages.PurpleTank;
            dic[PlayerColor.Red] = DrawingImages.RedTank;
            dic[PlayerColor.Yellow] = DrawingImages.YellowTank;
            return dic;
        }

    }
}

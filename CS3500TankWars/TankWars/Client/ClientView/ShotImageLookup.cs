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
    /// This class looks up the shot images for projectiles.
    /// </summary>
    public static class ShotImageLookup
    {

        private static readonly Dictionary<PlayerColor, Image> shotImagesColorMap = LoadShotImagesColorMap();

        public static Image GetShotImageByColor(PlayerColor color)
        {
            return shotImagesColorMap[color];
        }

        private static Dictionary<PlayerColor, Image> LoadShotImagesColorMap()
        {
            Dictionary<PlayerColor, Image> dic = new Dictionary<PlayerColor, Image>();
            dic[PlayerColor.Blue] = DrawingImages.BlueShot;
            dic[PlayerColor.DarkBlue] = DrawingImages.DarkBlueShot;
            dic[PlayerColor.Green] = DrawingImages.GreenShot;
            dic[PlayerColor.LightGreen] = DrawingImages.LightGreenShot;
            dic[PlayerColor.Orange] = DrawingImages.OrangeShot;
            dic[PlayerColor.Purple] = DrawingImages.PurpleShot;
            dic[PlayerColor.Red] = DrawingImages.RedShot;
            dic[PlayerColor.Yellow] = DrawingImages.YellowShot;
            return dic;
        }

    }
}


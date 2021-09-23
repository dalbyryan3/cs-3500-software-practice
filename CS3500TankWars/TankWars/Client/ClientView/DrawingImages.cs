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
    /// This class contains all the images used by the view. 
    /// each image is only loaded into memory once, which is why they're all assigned 
    /// to static readonly fields.
    /// 
    /// loading images only once is a very important optimization that boosts performance
    /// and solves the screen flickering problem.
    /// 
    /// when the gif images need to be used, each individual animator object (e.g. ExplosionDrawer)
    /// actually makes its own clone of the Bitmap object. this is necessary because otherwise they would
    /// all literally be animating the same image.
    /// </summary>
    public static class DrawingImages
    {

        public static readonly Image BlueTank = LoadBlueTank();
        public static readonly Image BlueTurret = LoadBlueTurret();
        public static readonly Image BlueShot = LoadBlueShot();

        public static readonly Image DarkBlueTank = LoadDarkBlueTank();
        public static readonly Image DarkBlueTurret = LoadDarkBlueTurret();
        public static readonly Image DarkBlueShot = LoadDarkBlueShot();

        public static readonly Image GreenTank = LoadGreenTank();
        public static readonly Image GreenTurret = LoadGreenTurret();
        public static readonly Image GreenShot = LoadGreenShot();

        public static readonly Image LightGreenTank = LoadLightGreenTank();
        public static readonly Image LightGreenTurret = LoadLightGreenTurret();
        public static readonly Image LightGreenShot = LoadLightGreenShot();

        public static readonly Image OrangeTank = LoadOrangeTank();
        public static readonly Image OrangeTurret = LoadOrangeTurret();
        public static readonly Image OrangeShot = LoadOrangeShot();

        public static readonly Image PurpleTank = LoadPurpleTank();
        public static readonly Image PurpleTurret = LoadPurpleTurret();
        public static readonly Image PurpleShot = LoadPurpleShot();

        public static readonly Image RedTank = LoadRedTank();
        public static readonly Image RedTurret = LoadRedTurret();
        public static readonly Image RedShot = LoadRedShot();

        public static readonly Image YellowTank = LoadYellowTank();
        public static readonly Image YellowTurret = LoadYellowTurret();
        public static readonly Image YellowShot = LoadYellowShot();


        public static readonly Image Background = LoadBackground();
        public static readonly Image Wall = LoadWall();

        public static readonly Bitmap ExplosionGif = LoadExplosionGif();
        public static readonly Bitmap PowerupGif = LoadPowerupGif();
        public static readonly Bitmap LaserBeamGif = LoadLaserBeamGif();


        private const string imagesDirectoryPath = @"..\..\..\..\Resources\Images\";
        private static Image LoadBlueTank()
        {
            string imageName = "BlueTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadBlueTurret()
        {
            string imageName = "BlueTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadBlueShot()
        {
            string imageName = "shot_blue.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadDarkBlueTank()
        {
            string imageName = "DarkTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadDarkBlueTurret()
        {
            string imageName = "DarkTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadDarkBlueShot()
        {
            string imageName = "shot_grey.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadGreenTank()
        {
            string imageName = "GreenTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadGreenTurret()
        {
            string imageName = "GreenTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadGreenShot()
        {
            string imageName = "shot-green.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadLightGreenTank()
        {
            string imageName = "LightGreenTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadLightGreenTurret()
        {
            string imageName = "LightGreenTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadLightGreenShot()
        {
            string imageName = "shot-white.png";  // use white shot because there is no "light green" shot image
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadOrangeTank()
        {
            string imageName = "OrangeTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadOrangeTurret()
        {
            string imageName = "OrangeTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadOrangeShot()
        {
            string imageName = "shot-brown.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadPurpleTank()
        {
            string imageName = "PurpleTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadPurpleTurret()
        {
            string imageName = "PurpleTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadPurpleShot()
        {
            string imageName = "shot_violet.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadRedTank()
        {
            string imageName = "RedTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadRedTurret()
        {
            string imageName = "RedTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadRedShot()
        {
            string imageName = "shot_red_new.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadYellowTank()
        {
            string imageName = "YellowTank.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadYellowTurret()
        {
            string imageName = "YellowTurret.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }
        private static Image LoadYellowShot()
        {
            string imageName = "shot-yellow.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadBackground()
        {
            string imageName = "Background.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Image LoadWall()
        {
            string imageName = "WallSprite.png";
            return Image.FromFile(imagesDirectoryPath + imageName);
        }

        private static Bitmap LoadExplosionGif()
        {
            string imageName = "Explosion.gif";
            return new Bitmap(imagesDirectoryPath + imageName);
        }

        private static Bitmap LoadPowerupGif()
        {
            string imageName = "mario-star-30px.gif";
            return new Bitmap(imagesDirectoryPath + imageName);
        }

        private static Bitmap LoadLaserBeamGif()
        {
            string imageName = "laser-beam.gif";
            return new Bitmap(imagesDirectoryPath + imageName);
        }




    }
}

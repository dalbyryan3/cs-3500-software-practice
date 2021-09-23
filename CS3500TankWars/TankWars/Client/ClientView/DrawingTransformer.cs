// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    /// <summary>
    /// This class handles all the transformations needed to center the player's view.
    /// Centering the view is not difficult if you simply draw things as though the image size matches the world size, 
    /// but with one transformation (a translation) before drawing everything.
    /// First, we calculate the overall transform to center the main players view.
    /// Then all Drawers utilize DrawObjectWithTransform, which
    /// performs the necessary transformation, given information that you supply, 
    /// and then invokes your ObjectDrawer delegate.
    /// </summary>
    internal class DrawingTransformer
    {

        /// <summary>
        /// A delegate for DrawObjectWithTransform.
        /// Methods matching this delegate can draw whatever they want using PaintEventArgs e  
        /// </summary>
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        public static void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();
            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);
            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }


        public static void TranslateTransformToCenterPlayersView(int viewSize, int worldSize, Vector2D playerLocation, PaintEventArgs e)
        {
            double playerX = playerLocation.GetX();
            double playerY = playerLocation.GetY();
            double ratio = (double)viewSize / (double)worldSize;
            int halfSizeScaled = (int)(worldSize / 2.0 * ratio);
            double inverseTranslateX = -WorldSpaceToImageSpace(worldSize, playerX) + halfSizeScaled;
            double inverseTranslateY = -WorldSpaceToImageSpace(worldSize, playerY) + halfSizeScaled;
            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);
        }


        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns>Image space coordinate corresponding to the given world space coordinate</returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

    }
}

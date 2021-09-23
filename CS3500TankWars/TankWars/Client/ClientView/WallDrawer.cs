// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    /// <summary>
    /// This class draws a wall for the TankWars game.
    /// </summary>
    internal class WallDrawer
    {

        private const int wallSize = 50;  

        public void DrawWall(Wall wall, PaintEventArgs e, int worldSize)
        {
            if (IsVerticalWall(wall)) {
                DrawVerticalWall(wall, e, worldSize);
            } else if (IsHorizontalWall(wall)) {
                DrawHorizontalWall(wall, e, worldSize);
            }
        }

        private void DrawVerticalWall(Wall wall, PaintEventArgs e, int worldSize)
        {
            double posX = wall.EndPoint1.GetX();  // X position will always be the same
            double lowestPosY = Math.Min(wall.EndPoint1.GetY(), wall.EndPoint2.GetY());
            double highestPosY = Math.Max(wall.EndPoint1.GetY(), wall.EndPoint2.GetY());
            // The length between p1 and p2 will always be a multiple of the wall width (50 units).
            int numWallBlocksToDraw = Convert.ToInt32((highestPosY - lowestPosY) / wallSize);
            for (int i = 0; i <= numWallBlocksToDraw; i++) {
                double offset = wallSize * i;
                double posY = lowestPosY + offset;
                // TODO how should we get the gameWorld size?
                DrawingTransformer.DrawObjectWithTransform(e, wall, worldSize, posX, posY, 0, DrawWallBlock);
            }
        }


        private void DrawHorizontalWall(Wall wall, PaintEventArgs e, int worldSize)
        {
            double posY = wall.EndPoint1.GetY();  // Y position will always be the same
            double leftmostPosX = Math.Min(wall.EndPoint1.GetX(), wall.EndPoint2.GetX());
            double rightmostPosX = Math.Max(wall.EndPoint1.GetX(), wall.EndPoint2.GetX());
            // The length between p1 and p2 will always be a multiple of the wall width (50 units).
            int numWallBlocksToDraw = Convert.ToInt32((rightmostPosX - leftmostPosX) / wallSize);
            for (int i = 0; i <= numWallBlocksToDraw; i++) {
                double offset = wallSize * i;
                double posX = leftmostPosX + offset;
                DrawingTransformer.DrawObjectWithTransform(e, wall, worldSize, posX, posY, 0, DrawWallBlock);
            }
        }

        private void DrawWallBlock(object o, PaintEventArgs e)
        {
            int wallSize = 50;
            Rectangle wallBounds = new Rectangle(-(wallSize / 2), -(wallSize / 2), wallSize, wallSize);
            e.Graphics.DrawImage(DrawingImages.Wall, wallBounds);
        }

        // walls will always be axis-aligned (purely horizontal or purely vertical, never diagonal). 
        // This means p1 and p2 will have either the same x value or the same y value.
        private bool IsVerticalWall(Wall wall)
        {
            return ((wall.EndPoint1.GetX() - wall.EndPoint2.GetX()) == 0);
        }
        private bool IsHorizontalWall(Wall wall)
        {
            return ((wall.EndPoint1.GetY() - wall.EndPoint2.GetY()) == 0);
        }

    }
}

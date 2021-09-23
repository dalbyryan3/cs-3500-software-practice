using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    public class BoundingBox
    {

        public double X { get; private set;}
        public double Y { get; private set;}
        public int Width { get; set;}
        public int Height { get; set;}

        public BoundingBox(Tank tank, int tankSize)
        {
            this.Width = tankSize;
            this.Height = tankSize;
            this.X = tank.Location.GetX() - (this.Width / 2.0);
            this.Y = tank.Location.GetY() - (this.Height / 2.0);
        }

        public BoundingBox(Wall wall, int wallSize)
        {
            if (wall.EndPoint1.GetX() == wall.EndPoint2.GetX()) {
                this.X = wall.EndPoint1.GetX() - (wallSize / 2.0);
                // "lowest" Y is actually the top of the screen, the most negative number in world space coordinates
                double topY = Math.Min(wall.EndPoint1.GetY(), wall.EndPoint2.GetY());
                // "highest" Y is actually the bottom of the screen, the most positive number in world space coordinates
                double bottomY = Math.Max(wall.EndPoint1.GetY(), wall.EndPoint2.GetY());
                this.Y = topY - (wallSize / 2.0);
                this.Width = wallSize;
                this.Height = (int)Math.Round((bottomY - topY) + wallSize);
            } else {
                this.Y = wall.EndPoint1.GetY() - (wallSize / 2.0);
                double leftX = Math.Min(wall.EndPoint1.GetX(), wall.EndPoint2.GetX());
                double rightX = Math.Max(wall.EndPoint1.GetX(), wall.EndPoint2.GetX());
                this.X = leftX - (wallSize / 2.0);
                this.Height = wallSize;
                this.Width = (int)Math.Round((rightX - leftX) + wallSize);
            }
        }

        public BoundingBox(Projectile projectile, int projectileSize)
        {
            this.Width = projectileSize;
            this.Height = projectileSize;
            this.X = projectile.Location.GetX() - (this.Width / 2.0);
            this.Y = projectile.Location.GetY() - (this.Height / 2.0);
        }

        public BoundingBox(Powerup powerup, int powerupSize) 
        {
            this.Width = powerupSize;
            this.Height = powerupSize;
            this.X = powerup.Location.GetX() - (this.Width / 2.0);
            this.Y = powerup.Location.GetY() - (this.Height / 2.0);
        }

        public BoundingBox(double x, double y, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.X = x - (this.Width / 2.0);
            this.Y = y - (this.Height / 2.0);
        }


        public bool CollidesWith(BoundingBox box2)
        {
            // Implements a standard axis aligned bounding box collision check
            return this.X < box2.X + box2.Width
                && this.X + this.Width > box2.X
                && this.Y < box2.Y + box2.Height
                && this.Y + this.Height > box2.Y;
        }

    }
}

// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TankWars
{
    /// <summary>
    /// This class draws the projectiles for the TankWars game.
    /// </summary>
    internal class ProjectileDrawer
    {

        private const int projectileSize = 30;
        private PlayerColorManager playerColorManager;

        public ProjectileDrawer(PlayerColorManager playerColorManager) 
        {
            this.playerColorManager = playerColorManager;
        }

        public void DrawProjectile(Projectile projectile, PaintEventArgs e, int worldSize)
        {
            double posX = projectile.Location.GetX();
            double posY = projectile.Location.GetY();
            projectile.Direction.Normalize();
            double angle = projectile.Direction.ToAngle();
            if (Double.IsNaN(posX) || Double.IsNaN(posY) || Double.IsNaN(angle)) {
                return;
            }
            DrawingTransformer.DrawObjectWithTransform(e, projectile, worldSize, posX, posY, angle, DrawProjectileSprite);
        }

        private void DrawProjectileSprite(object o, PaintEventArgs e)
        {
            Projectile projectile = o as Projectile;
            Rectangle projectileBounds = new Rectangle(-(projectileSize / 2), -(projectileSize / 2), projectileSize, projectileSize);
            PlayerColor projectileColor = playerColorManager.GetPlayerColorByID(projectile.Owner);
            Image projectileImage = ShotImageLookup.GetShotImageByColor(projectileColor);
            e.Graphics.DrawImage(projectileImage, projectileBounds);
        }

    }
}

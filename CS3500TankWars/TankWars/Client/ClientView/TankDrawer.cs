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
    /// This class draws the tank body, turret, health bar, and name that comprise a Tank in TankWars.
    /// </summary>
    internal class TankDrawer
    {

        private const int tankSize = 60;
        private const int turretSize = 50;
        private const int displayNameWidth = 200;
        private const int displayNameHeight = 30;
        private PlayerColorManager playerColorManager;

        public TankDrawer(PlayerColorManager playerColorManager)
        {
            this.playerColorManager = playerColorManager;
        }

        public void DrawTank(Tank tank, PaintEventArgs e, int worldSize)
        {
            DrawingTransformer.DrawObjectWithTransform(e, tank, worldSize, tank.Location.GetX(), tank.Location.GetY(), tank.BodyDirection.ToAngle(), DrawTankBody);
            tank.TurretDirection.Normalize();
            double angle = tank.TurretDirection.ToAngle();
            if (Double.IsNaN(angle)) {
                // this prevents the game from crashing when you put your mouse in the middle of the tank 
                angle = 0;
            }
            DrawingTransformer.DrawObjectWithTransform(e, tank, worldSize, tank.Location.GetX(), tank.Location.GetY(), angle, DrawTankTurret);
            DrawingTransformer.DrawObjectWithTransform(e, tank, worldSize, tank.Location.GetX(), tank.Location.GetY(), 0, DrawTankName);
            DrawingTransformer.DrawObjectWithTransform(e, tank, worldSize, tank.Location.GetX(), tank.Location.GetY(), 0, DrawTankHealth);
        }

        public void DrawTankBody(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;
            Rectangle tankBounds = new Rectangle(-(tankSize / 2), -(tankSize / 2), tankSize, tankSize);
            PlayerColor tankColor = playerColorManager.GetPlayerColorByID(tank.ID);
            Image tankImage = TankImageLookup.GetTankImageByColor(tankColor);
            e.Graphics.DrawImage(tankImage, tankBounds);
        }

        public void DrawTankTurret(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;
            Rectangle turretBounds = new Rectangle(-(turretSize / 2), -(turretSize / 2), turretSize, turretSize);
            PlayerColor turretColor = playerColorManager.GetPlayerColorByID(tank.ID);
            Image turretImage = TurretImageLookup.GetTurretImageByColor(turretColor);
            e.Graphics.DrawImage(turretImage, turretBounds);
        }

        private void DrawTankName(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;
            string tankDisplayName = tank.PlayerName + ": " + tank.Score;
            Font drawFont = new Font("Arial", 16);
            using (SolidBrush drawBrush = new SolidBrush(Color.White)) {
                Rectangle tankNameBounds = new Rectangle(-displayNameHeight, displayNameHeight, displayNameWidth, displayNameHeight);
                e.Graphics.DrawString(tankDisplayName, drawFont, drawBrush, tankNameBounds);
            }
        }


        private void DrawTankHealth(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;
            using (SolidBrush greenBrush = new SolidBrush(Color.SpringGreen)) 
            using (SolidBrush yellowBrush = new SolidBrush(Color.Yellow)) 
            using (SolidBrush redBrush = new SolidBrush(Color.Red)) {
                SolidBrush drawBrush = null;
                Rectangle healthBounds = new Rectangle(0, 0, 0, 0);
                if (tank.IsHighHealth()) {
                    drawBrush = greenBrush;
                    healthBounds = new Rectangle(-25, 60, 60, 10);
                } else if (tank.IsMediumHealth()) {
                    drawBrush = yellowBrush;
                    healthBounds = new Rectangle(-25, 60, 40, 10);
                } else if (tank.IsLowHealth()) {
                    drawBrush = redBrush;
                    healthBounds = new Rectangle(-25, 60, 20, 10);
                }
                if (drawBrush != null) {
                    e.Graphics.FillRectangle(drawBrush, healthBounds);
                }
            }
        }

    }
}

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
    /// This class draws a powerup for the TankWars game.
    /// powerups are animated super mario stars. the animations will continue indefinitely until a player picks them up,
    /// so the game drawing panel is responsible for telling the powerup drawer to stop. 
    /// </summary>
    internal class PowerupDrawer
    {

        // powerups don't have a required size, but the gif we use is 30x30 pixels
        private const int powerupSize = 30;

        private DrawingPanel drawingPanel;
        private Powerup powerup;
        private Bitmap marioStarGif;
        private bool currentlyAnimating;


        public PowerupDrawer(DrawingPanel drawingPanel, Powerup powerup)
        {
            this.drawingPanel = drawingPanel;
            this.powerup = powerup;
            this.currentlyAnimating = false;
            this.marioStarGif = DrawingImages.PowerupGif.Clone() as Bitmap;
        }

        public void ContinueDrawingPowerup(PaintEventArgs e, int worldSize)
        {
            if (!powerup.IsDead) {
                DrawPowerup(powerup, e, worldSize);
            }
        }

        public void DrawPowerup(Powerup powerup, PaintEventArgs e, int worldSize)
        {
            double posX = powerup.Location.GetX();
            double posY = powerup.Location.GetY();
            DrawingTransformer.DrawObjectWithTransform(e, powerup, worldSize, posX, posY, 0, DrawPowerupSprite);
        }

        private void DrawPowerupSprite(object o, PaintEventArgs e)
        {
            AnimatePowerup();
            ImageAnimator.UpdateFrames();
            Rectangle powerupBounds = new Rectangle(-(powerupSize / 2), -(powerupSize / 2), powerupSize, powerupSize);
            e.Graphics.DrawImage(marioStarGif, powerupBounds);
        }

        private void AnimatePowerup()
        {
            if (!currentlyAnimating) {
                ImageAnimator.Animate(marioStarGif, new EventHandler(this.OnFrameChanged));
                currentlyAnimating = true;
            }
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            drawingPanel.Invalidate();
        }

    }
}

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
    /// draw and animate explosions when tanks die. 
    /// this class will draw a certain number of frames for the explosion gif and then stop.
    /// </summary>
    public class ExplosionDrawer
    {

        private bool currentlyAnimating;
        private DrawingPanel drawingPanel;
        private Tank tank;  
        private Bitmap explosionGif;

        private int numFramesPassed;

        public ExplosionDrawer(DrawingPanel drawingPanel, Tank tank)
        {
            currentlyAnimating = false;
            this.drawingPanel = drawingPanel;
            this.tank = tank;
            this.explosionGif = DrawingImages.ExplosionGif.Clone() as Bitmap;

            numFramesPassed = 0;
        }


        public void ContinueDrawingExplosion(PaintEventArgs e, int worldSize)
        {
            if (numFramesPassed < 50) {
                DrawExplosion(tank, e, worldSize);
                numFramesPassed++;
            }
        }


        public void DrawExplosion(Tank tank, PaintEventArgs e, int worldSize)
        {
           DrawingTransformer.DrawObjectWithTransform(e, tank, worldSize, tank.Location.GetX(), tank.Location.GetY(), 0, DrawExplosionSprite);
        }

        private void DrawExplosionSprite(object o, PaintEventArgs e)
        {
           AnimateExplosion();
           ImageAnimator.UpdateFrames();
           Rectangle explosionBounds = new Rectangle(-150, -150, 300, 300);
           e.Graphics.DrawImage(explosionGif, explosionBounds);
        }

        private void AnimateExplosion()
        {
           if (!currentlyAnimating) {
               ImageAnimator.Animate(explosionGif, new EventHandler(this.OnFrameChanged));
               currentlyAnimating = true;
           }
        }

        public void OnFrameChanged(object o, EventArgs e)
        {
            drawingPanel.Invalidate();
        }

    }
}

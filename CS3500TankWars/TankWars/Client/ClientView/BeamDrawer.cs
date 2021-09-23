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
    /// This class draws and animates beams attacks that are fired by tanks. 
    /// </summary>
    public class BeamDrawer
    {

        private const int beamWidth = 50;
        private const int beamLength = 2000;

        private DrawingPanel drawingPanel;
        private Beam beam;
        private Bitmap beamGif;

        private bool currentlyAnimating;
        private int numFramesAnimatedSoFar;
        private int numFramesToAnimate;

        public BeamDrawer(DrawingPanel drawingPanel, Beam beam)
        {
            this.drawingPanel = drawingPanel;
            this.beam = beam;
            this.beamGif = DrawingImages.LaserBeamGif.Clone() as Bitmap;
            this.currentlyAnimating = false;
            this.numFramesAnimatedSoFar = 0;
            this.numFramesToAnimate = 40;
        }

        public void ContinueDrawingBeam(PaintEventArgs e, int worldSize)
        {
            if (numFramesAnimatedSoFar < numFramesToAnimate) {
                DrawBeam(beam, e, worldSize);
                numFramesAnimatedSoFar++;
            }
        }

        public void DrawBeam(Beam beam, PaintEventArgs e, int worldSize)
        {
            double posX = beam.Origin.GetX();
            double posY = beam.Origin.GetY();
            beam.Direction.Normalize();
            double angle = beam.Direction.ToAngle();
            angle += 180;  // rotate 180 degrees
            if (Double.IsNaN(posX) || Double.IsNaN(posY) || Double.IsNaN(angle)) {
                return;
            }
            DrawingTransformer.DrawObjectWithTransform(e, beam, worldSize, posX, posY, angle, DrawBeamSprite);
        }

        public void DrawBeamSprite(object o, PaintEventArgs e)
        {
            AnimateBeam();
            ImageAnimator.UpdateFrames();
            Rectangle beamBounds = new Rectangle(-(beamWidth / 2), (beamWidth / 4), beamWidth, beamLength);
            e.Graphics.DrawImage(beamGif, beamBounds);
        }

        private void AnimateBeam()
        {
            if (!currentlyAnimating) {
                ImageAnimator.Animate(beamGif, new EventHandler(this.OnFrameChanged));
                currentlyAnimating = false;
            }
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            drawingPanel.Invalidate();
        }

    }
}

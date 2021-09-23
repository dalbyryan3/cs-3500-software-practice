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
    /// This class draws the background for the TankWars game.
    /// </summary>
    public class BackgroundDrawer
    {
        public void DrawBackground(PaintEventArgs e, int worldSize)
        {
            int posX = 0;
            int posY = 0;
            e.Graphics.DrawImage(DrawingImages.Background, posX, posY, worldSize, worldSize);
        }
    }
}

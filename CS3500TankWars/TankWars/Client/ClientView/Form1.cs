// Luke Ludlow, Ryan Dalby, CS 3500 Fall 2019
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    /// <summary>
    /// User written portion of the main Form class for TankWars.
    /// </summary>
    public partial class Form1 : Form
    {

        private DrawingPanel drawingPanel;
        private GameController gameController;
        private World theWorld;

        public Form1()
        {
            InitializeComponent();

            // Initialize a GameController for the client and register handlers for its events
            gameController = new GameController();
            theWorld = gameController.theWorld;
            gameController.RedrawFrameEvent += RedrawFrame;
            gameController.ShowErrorMessageEvent += ShowErrorMessage;
            gameController.DisableConnectionMenuEvent += DisableConnectionMenu;

            // Initialize a DrawingPanel for the client and register handler for its events
            drawingPanel = new DrawingPanel(gameController);
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.Size = new Size(800, 800); // A drawingPanel is always 800x800 pixels on the form
            drawingPanel.MouseMove += GamePanel_MouseMove;
            drawingPanel.MouseDown += GamePanel_MouseDown;
            drawingPanel.MouseUp += GamePanel_MouseUp;
            gamePanel.Controls.Add(drawingPanel); // Add our drawingPanel to the gamePnel on the form

        }

        /// <summary>
        /// Disables all contorls in the connectionMenuPanel except the helpButton
        /// </summary>
        private void DisableConnectionMenu()
        {
            foreach (Control control in connectionMenuPanel.Controls) {
                Console.WriteLine(control.Name);
                if (control == helpButton) {
                    continue;
                } 
                MethodInvoker invoker = new MethodInvoker(() => control.Enabled = false);
                this.Invoke(invoker);
            }
        }

        /// <summary>
        /// Popup a MessageBox for an error
        /// </summary>
        private void ShowErrorMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Redraw the frame.
        /// </summary>
        private void RedrawFrame()
        {
            // Don't try to redraw if the window doesn't exist yet.
            if (!IsHandleCreated) {
                return;
            }
            try {
                MethodInvoker invoker = new MethodInvoker(() => this.Invalidate(true));
                this.Invoke(invoker);
            } catch (Exception) {
                // just ignore the ObjectDisposedException inside the catch block. 
                // It happens when the form is closed right before an OnPaint event.
            }
        }

        /// <summary>
        /// Triggers handshake for connecting to server.
        /// </summary>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            gameController.ConnectToServer(serverTextBox.Text, nameTextBox.Text);
        }

        /// <summary>
        /// Triggers processing of a key being pressed down.
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            gameController.ProcessKeyDown(e.KeyCode.ToString());
        }

        /// <summary>
        /// Triggers processing of a key being released.
        /// </summary>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            gameController.ProcessKeyUp(e.KeyCode.ToString());
        }
 
        /// <summary>
        /// Triggers processing of a mouse movement.
        /// </summary>
        private void GamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            // should be the cursor position (normalized) relative to the center of the image, since your tank is always in the center of the image. 
            gameController.ProcessMouseMove(e.X - 400, e.Y - 400); // This needs to be changed, should make sure tank really is the center of the image
        }

        /// <summary>
        /// Triggers processing of mouse being pressed down.
        /// </summary>
        private void GamePanel_MouseDown(object sender, MouseEventArgs e)
        {
            gameController.ProcessMouseDown(e.Button.ToString());
        }

        /// <summary>
        /// Triggers processing of mouse being released.
        /// </summary>
        private void GamePanel_MouseUp(object sender, MouseEventArgs e)
        {
            gameController.ProcessMouseUp(e.Button.ToString());
        }

        /// <summary>
        /// Popup the help menu.
        /// </summary>
        private void HelpButton_Click(object sender, EventArgs e)
        {
            StringBuilder helpInformation = new StringBuilder();
            helpInformation.Append("TankWars\n\n");
            helpInformation.Append("About:\n");
            helpInformation.Append("Game design and implementation by Ryan Dalby and Luke Ludlow\n");
            helpInformation.Append("Artwork by Jolie Uk, Alex Smith, and Luke Ludlow\n");
            helpInformation.Append("University of Utah CS 3500 Fall 2019\n");
            helpInformation.Append("\n");
            helpInformation.Append("Controls:\n");
            helpInformation.Append("w:\t\tmove up\n");
            helpInformation.Append("a:\t\tmove left\n");
            helpInformation.Append("s:\t\tmove down\n");
            helpInformation.Append("d:\t\tmove right\n");
            helpInformation.Append("mouse:\t\taim\n");
            helpInformation.Append("left click:\t\tfire main attack\n");
            helpInformation.Append("right click:\tfire special attack\n");
            MessageBox.Show(helpInformation.ToString());
        }
    }
}

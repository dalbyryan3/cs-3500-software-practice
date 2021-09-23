namespace TankWars
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectionMenuPanel = new System.Windows.Forms.Panel();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.serverLabel = new System.Windows.Forms.Label();
            this.helpButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.connectionMenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectionMenuPanel
            // 
            this.connectionMenuPanel.Controls.Add(this.nameTextBox);
            this.connectionMenuPanel.Controls.Add(this.serverTextBox);
            this.connectionMenuPanel.Controls.Add(this.nameLabel);
            this.connectionMenuPanel.Controls.Add(this.serverLabel);
            this.connectionMenuPanel.Controls.Add(this.helpButton);
            this.connectionMenuPanel.Controls.Add(this.connectButton);
            this.connectionMenuPanel.Location = new System.Drawing.Point(11, 9);
            this.connectionMenuPanel.Margin = new System.Windows.Forms.Padding(2);
            this.connectionMenuPanel.Name = "connectionMenuPanel";
            this.connectionMenuPanel.Size = new System.Drawing.Size(800, 27);
            this.connectionMenuPanel.TabIndex = 0;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(174, 3);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(76, 20);
            this.nameTextBox.TabIndex = 5;
            this.nameTextBox.TabStop = false;
            this.nameTextBox.Text = "player";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(46, 3);
            this.serverTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(76, 20);
            this.serverTextBox.TabIndex = 4;
            this.serverTextBox.TabStop = false;
            this.serverTextBox.Text = "localhost";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(134, 6);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(36, 13);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "name:";
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(2, 7);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(39, 13);
            this.serverLabel.TabIndex = 2;
            this.serverLabel.Text = "server:";
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(742, 4);
            this.helpButton.Margin = new System.Windows.Forms.Padding(2);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(56, 19);
            this.helpButton.TabIndex = 1;
            this.helpButton.Text = "help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(265, 3);
            this.connectButton.Margin = new System.Windows.Forms.Padding(2);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(56, 19);
            this.connectButton.TabIndex = 0;
            this.connectButton.TabStop = false;
            this.connectButton.Text = "connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // gamePanel
            // 
            this.gamePanel.Location = new System.Drawing.Point(11, 40);
            this.gamePanel.Margin = new System.Windows.Forms.Padding(2);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(800, 800);
            this.gamePanel.TabIndex = 1;
            this.gamePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GamePanel_MouseDown);
            this.gamePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GamePanel_MouseMove);
            this.gamePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GamePanel_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 849);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.connectionMenuPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "TankWars";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.connectionMenuPanel.ResumeLayout(false);
            this.connectionMenuPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel connectionMenuPanel;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Panel gamePanel;
    }
}


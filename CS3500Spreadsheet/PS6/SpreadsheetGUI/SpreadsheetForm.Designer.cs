namespace SS
{
    partial class SpreadsheetForm
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
            if (disposing && (components != null))
            {
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
            this.FormSpreadsheetPanel = new SS.SpreadsheetPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CellValueUneditableTextBox = new System.Windows.Forms.TextBox();
            this.CellContentTextBox = new System.Windows.Forms.TextBox();
            this.CellNameTextLabel = new System.Windows.Forms.Label();
            this.CellValueTextLabel = new System.Windows.Forms.Label();
            this.CellContentTextLabel = new System.Windows.Forms.Label();
            this.EnterContentButton = new System.Windows.Forms.Button();
            this.CellNameUneditableTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormSpreadsheetPanel
            // 
            this.FormSpreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FormSpreadsheetPanel.Location = new System.Drawing.Point(12, 64);
            this.FormSpreadsheetPanel.Name = "FormSpreadsheetPanel";
            this.FormSpreadsheetPanel.Size = new System.Drawing.Size(776, 375);
            this.FormSpreadsheetPanel.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_click);
            // 
            // CellValueUneditableTextBox
            // 
            this.CellValueUneditableTextBox.Location = new System.Drawing.Point(178, 37);
            this.CellValueUneditableTextBox.Name = "CellValueUneditableTextBox";
            this.CellValueUneditableTextBox.ReadOnly = true;
            this.CellValueUneditableTextBox.Size = new System.Drawing.Size(147, 20);
            this.CellValueUneditableTextBox.TabIndex = 3;
            // 
            // CellContentTextBox
            // 
            this.CellContentTextBox.Location = new System.Drawing.Point(404, 37);
            this.CellContentTextBox.Name = "CellContentTextBox";
            this.CellContentTextBox.Size = new System.Drawing.Size(147, 20);
            this.CellContentTextBox.TabIndex = 4;
            // 
            // CellNameTextLabel
            // 
            this.CellNameTextLabel.AutoSize = true;
            this.CellNameTextLabel.Location = new System.Drawing.Point(26, 40);
            this.CellNameTextLabel.Name = "CellNameTextLabel";
            this.CellNameTextLabel.Size = new System.Drawing.Size(27, 13);
            this.CellNameTextLabel.TabIndex = 5;
            this.CellNameTextLabel.Text = "Cell:";
            // 
            // CellValueTextLabel
            // 
            this.CellValueTextLabel.AutoSize = true;
            this.CellValueTextLabel.Location = new System.Drawing.Point(115, 40);
            this.CellValueTextLabel.Name = "CellValueTextLabel";
            this.CellValueTextLabel.Size = new System.Drawing.Size(57, 13);
            this.CellValueTextLabel.TabIndex = 7;
            this.CellValueTextLabel.Text = "Cell Value:";
            // 
            // CellContentTextLabel
            // 
            this.CellContentTextLabel.AutoSize = true;
            this.CellContentTextLabel.Location = new System.Drawing.Point(331, 40);
            this.CellContentTextLabel.Name = "CellContentTextLabel";
            this.CellContentTextLabel.Size = new System.Drawing.Size(67, 13);
            this.CellContentTextLabel.TabIndex = 8;
            this.CellContentTextLabel.Text = "Cell Content:";
            // 
            // EnterContentButton
            // 
            this.EnterContentButton.Location = new System.Drawing.Point(557, 35);
            this.EnterContentButton.Name = "EnterContentButton";
            this.EnterContentButton.Size = new System.Drawing.Size(85, 23);
            this.EnterContentButton.TabIndex = 9;
            this.EnterContentButton.Text = "Enter Content";
            this.EnterContentButton.UseVisualStyleBackColor = true;
            this.EnterContentButton.Click += new System.EventHandler(this.EnterContentButton_click);
            // 
            // CellNameUneditableTextBox
            // 
            this.CellNameUneditableTextBox.Location = new System.Drawing.Point(59, 37);
            this.CellNameUneditableTextBox.Name = "CellNameUneditableTextBox";
            this.CellNameUneditableTextBox.ReadOnly = true;
            this.CellNameUneditableTextBox.Size = new System.Drawing.Size(50, 20);
            this.CellNameUneditableTextBox.TabIndex = 10;
            this.CellNameUneditableTextBox.Text = "A1";
            this.CellNameUneditableTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "sprd";
            this.openFileDialog.Filter = "Spreadsheet files|*.sprd|All files|*.*";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog_FileOk);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "sprd";
            this.saveFileDialog.Filter = "Spreadsheet files|*.sprd|All files|*.*";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileDialog_FileOk);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CellNameUneditableTextBox);
            this.Controls.Add(this.EnterContentButton);
            this.Controls.Add(this.CellContentTextLabel);
            this.Controls.Add(this.CellValueTextLabel);
            this.Controls.Add(this.CellNameTextLabel);
            this.Controls.Add(this.CellContentTextBox);
            this.Controls.Add(this.CellValueUneditableTextBox);
            this.Controls.Add(this.FormSpreadsheetPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpreadsheetForm";
            this.Text = "Spreadsheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetForm_FormClosing);
            this.Shown += new System.EventHandler(this.SpreadsheetForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SpreadsheetPanel FormSpreadsheetPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TextBox CellValueUneditableTextBox;
        private System.Windows.Forms.TextBox CellContentTextBox;
        private System.Windows.Forms.Label CellNameTextLabel;
        private System.Windows.Forms.Label CellValueTextLabel;
        private System.Windows.Forms.Label CellContentTextLabel;
        private System.Windows.Forms.Button EnterContentButton;
        private System.Windows.Forms.TextBox CellNameUneditableTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}


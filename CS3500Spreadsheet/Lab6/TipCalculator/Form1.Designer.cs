namespace TipCalculator
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
            this.EnterTotalBillTextBox = new System.Windows.Forms.TextBox();
            this.EnterTotalBillLabel = new System.Windows.Forms.Label();
            this.ComputeTipButton = new System.Windows.Forms.Button();
            this.ComputeTipTextBox = new System.Windows.Forms.TextBox();
            this.EnterTipPercentageLabel = new System.Windows.Forms.Label();
            this.EnterTipPercentageTextBox = new System.Windows.Forms.TextBox();
            this.TotalAmountToPayLabel = new System.Windows.Forms.Label();
            this.TotalAmountToPayTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // EnterTotalBillTextBox
            // 
            this.EnterTotalBillTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.EnterTotalBillTextBox.Location = new System.Drawing.Point(246, 101);
            this.EnterTotalBillTextBox.Name = "EnterTotalBillTextBox";
            this.EnterTotalBillTextBox.Size = new System.Drawing.Size(100, 22);
            this.EnterTotalBillTextBox.TabIndex = 0;
            this.EnterTotalBillTextBox.TextChanged += new System.EventHandler(this.EnterTotalBillTextBox_TextChanged);
            // 
            // EnterTotalBillLabel
            // 
            this.EnterTotalBillLabel.AutoSize = true;
            this.EnterTotalBillLabel.Location = new System.Drawing.Point(95, 101);
            this.EnterTotalBillLabel.Name = "EnterTotalBillLabel";
            this.EnterTotalBillLabel.Size = new System.Drawing.Size(100, 17);
            this.EnterTotalBillLabel.TabIndex = 1;
            this.EnterTotalBillLabel.Text = "Enter Total Bill";
            // 
            // ComputeTipButton
            // 
            this.ComputeTipButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ComputeTipButton.Location = new System.Drawing.Point(74, 220);
            this.ComputeTipButton.Name = "ComputeTipButton";
            this.ComputeTipButton.Size = new System.Drawing.Size(130, 34);
            this.ComputeTipButton.TabIndex = 2;
            this.ComputeTipButton.Text = "Compute Tip";
            this.ComputeTipButton.UseVisualStyleBackColor = false;
            this.ComputeTipButton.Click += new System.EventHandler(this.ComputeTipButton_Click);
            // 
            // ComputeTipTextBox
            // 
            this.ComputeTipTextBox.Location = new System.Drawing.Point(246, 232);
            this.ComputeTipTextBox.Name = "ComputeTipTextBox";
            this.ComputeTipTextBox.Size = new System.Drawing.Size(100, 22);
            this.ComputeTipTextBox.TabIndex = 3;
            // 
            // EnterTipPercentageLabel
            // 
            this.EnterTipPercentageLabel.AutoSize = true;
            this.EnterTipPercentageLabel.Location = new System.Drawing.Point(71, 159);
            this.EnterTipPercentageLabel.Name = "EnterTipPercentageLabel";
            this.EnterTipPercentageLabel.Size = new System.Drawing.Size(143, 17);
            this.EnterTipPercentageLabel.TabIndex = 4;
            this.EnterTipPercentageLabel.Text = "Enter Tip Percentage";
            // 
            // EnterTipPercentageTextBox
            // 
            this.EnterTipPercentageTextBox.Location = new System.Drawing.Point(246, 159);
            this.EnterTipPercentageTextBox.Name = "EnterTipPercentageTextBox";
            this.EnterTipPercentageTextBox.Size = new System.Drawing.Size(100, 22);
            this.EnterTipPercentageTextBox.TabIndex = 5;
            this.EnterTipPercentageTextBox.TextChanged += new System.EventHandler(this.EnterTipPercentageTextBox_TextChanged);
            // 
            // TotalAmountToPayLabel
            // 
            this.TotalAmountToPayLabel.AutoSize = true;
            this.TotalAmountToPayLabel.Location = new System.Drawing.Point(74, 303);
            this.TotalAmountToPayLabel.Name = "TotalAmountToPayLabel";
            this.TotalAmountToPayLabel.Size = new System.Drawing.Size(141, 17);
            this.TotalAmountToPayLabel.TabIndex = 6;
            this.TotalAmountToPayLabel.Text = "Total Amount To Pay";
            // 
            // TotalAmountToPayTextBox
            // 
            this.TotalAmountToPayTextBox.Location = new System.Drawing.Point(246, 303);
            this.TotalAmountToPayTextBox.Name = "TotalAmountToPayTextBox";
            this.TotalAmountToPayTextBox.Size = new System.Drawing.Size(100, 22);
            this.TotalAmountToPayTextBox.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TotalAmountToPayTextBox);
            this.Controls.Add(this.TotalAmountToPayLabel);
            this.Controls.Add(this.EnterTipPercentageTextBox);
            this.Controls.Add(this.EnterTipPercentageLabel);
            this.Controls.Add(this.ComputeTipTextBox);
            this.Controls.Add(this.ComputeTipButton);
            this.Controls.Add(this.EnterTotalBillLabel);
            this.Controls.Add(this.EnterTotalBillTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox EnterTotalBillTextBox;
        private System.Windows.Forms.Label EnterTotalBillLabel;
        private System.Windows.Forms.Button ComputeTipButton;
        private System.Windows.Forms.TextBox ComputeTipTextBox;
        private System.Windows.Forms.Label EnterTipPercentageLabel;
        private System.Windows.Forms.TextBox EnterTipPercentageTextBox;
        private System.Windows.Forms.Label TotalAmountToPayLabel;
        private System.Windows.Forms.TextBox TotalAmountToPayTextBox;
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ComputeTipButton_Click(object sender, EventArgs e)
        {
            double total = 0.0;
            double percentage = 0.0;
            Double.TryParse(EnterTotalBillTextBox.Text, out total);
            Double.TryParse(EnterTipPercentageTextBox.Text, out percentage);
            double amount =  total * (percentage / 100.0);
            ComputeTipTextBox.Text = (amount + "");
            TotalAmountToPayTextBox.Text = (amount + total) + "";
        }

        private void EnterTotalBillTextBox_TextChanged(object sender, EventArgs e)
        {
            this.enableDisableButton();
            this.ComputeTipButton_Click(sender, e);
        }

        private void EnterTipPercentageTextBox_TextChanged(object sender, EventArgs e)
        {
            this.enableDisableButton();
            this.ComputeTipButton_Click(sender, e);
        }
        /// <summary>
        /// Enables or disables button based on if the text in the corresponding textboxes can be parsed
        /// </summary>
        private void enableDisableButton()
        {
            if (Double.TryParse(EnterTotalBillTextBox.Text, out double total) && Double.TryParse(EnterTipPercentageTextBox.Text, out double percentage))
            {
                ComputeTipButton.Enabled = true;
            }
            else
            {
                ComputeTipButton.Enabled = false;
            }
        }

    }
}

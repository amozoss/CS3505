using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class YesNoWarning : Form
    {
        public delegate void YesOrNo(int choice);
        public YesOrNo Option;


        /// <summary>
        /// Constructor for Yes or No warning form
        /// </summary>
        /// <param name="warningString">The warning displayed to the user.</param>
        public YesNoWarning(String warningString, YesOrNo c)
        {
            Option = c;
            InitializeComponent();
            label1.Text = warningString;
            this.Width = label1.Width + 20;
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            Option(1);
            Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            Option(0);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Option(2);
            Close();
        }


    }
}

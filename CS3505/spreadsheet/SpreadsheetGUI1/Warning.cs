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
    public partial class Warning : Form
    {
        /// <summary>
        /// Creates the warning form.
        /// </summary>
        /// <param name="warningString">The warning that will be shown to the user.</param>
        public Warning(String warningString)
        {
            InitializeComponent();
            label1.Text = warningString;
            this.Width = label1.Width + 20;
        }

        private void Warning_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                this.Close();
        }
    }
}

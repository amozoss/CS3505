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
    /// <summary>
    /// A warning that will be shown when the user tries to exit the current form.
    /// </summary>
    public partial class ExitWarning : Form
    {
        public delegate void YesOrNo(int yOrn);
        public YesOrNo temp;

        public ExitWarning(YesOrNo yon)
        {
            
            temp = yon;
            InitializeComponent();
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            temp(1);
            Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            temp(0);
            Close();
        }
    }
}

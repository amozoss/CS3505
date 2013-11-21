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
    public partial class Save : Form
    {

        public delegate void GetFileName(string filePath);
        public GetFileName filePath;

        public Save(GetFileName files)
        {
            InitializeComponent();
            filePath = files;

        }



        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                Warning warn = new Warning("Cannot save without a valid name.");
                warn.ShowDialog();
            }
            else
            {
                filePath(textBox1.Text);
                Close();
            }
        }
    }
}

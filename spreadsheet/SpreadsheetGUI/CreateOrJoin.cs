using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SS
{
    public partial class CreateOrJoin : Form
    {
        public delegate void Create(string ipadd, string pt, string ssN, string pswd);
        public delegate void Join(string ipadd, string pt, string ssN, string pswd);

        
        public CreateOrJoin(Create cFunk, Join jFunk)
        {
            InitializeComponent();
            createButton.Focus();
            port.GotFocus += OnFocus;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            
            //ipAddress.Text = box.text
        }

        private void OnFocus(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.Text = "";

            //MessageBox.Show("Got focus.");
        }
    }
}

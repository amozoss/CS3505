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
        private Create cfunk;
        private Join jfunk;
        
        public CreateOrJoin(Create a, Join n)
        {
            cfunk = a;
            jfunk = n;
            InitializeComponent();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
           
            cfunk(ipAddress.Text, port.Text, ssName.Text, passWord.Text);
            this.Close();
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            jfunk(ipAddress.Text, port.Text, ssName.Text, passWord.Text);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            this.Close();
        }


    }
}

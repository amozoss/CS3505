using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

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

        private bool crapChecker(string ip, string pt)
        {
            IPAddress ad = null;
            int num = 0;
            if (IPAddress.TryParse(ip, out ad) || ip == "localhost")
                if (Int32.TryParse(pt, out num))
                    return true;
            return false;
        }


        private void createButton_Click(object sender, EventArgs e)
        {
            if (crapChecker(ipAddress.Text, port.Text))
            {
                cfunk(ipAddress.Text, port.Text, ssName.Text, passWord.Text);
                this.Close();
            }
            else
            {

            }
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            if (crapChecker(ipAddress.Text, port.Text))
            {
                jfunk(ipAddress.Text, port.Text, ssName.Text, passWord.Text);
                this.Close();
            }
            else
            {


            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            this.Close();
        }


    }
}

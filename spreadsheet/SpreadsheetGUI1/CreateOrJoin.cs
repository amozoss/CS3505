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

            ipAddress.Text = loadFromRegistry("ipAdress");
            port.Text = loadFromRegistry("port");
            ssName.Text = loadFromRegistry("ssName");
            passWord.Text = loadFromRegistry("passWord");

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

            saveEntries();

            cfunk(ipAddress.Text, port.Text, ssName.Text, passWord.Text);
            this.Close();

        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            saveEntries();
            jfunk(ipAddress.Text, port.Text, ssName.Text, passWord.Text);
            this.Close();

        }

        private void saveEntries()
        {
            saveToRegistry("ipAdress", ipAddress.Text);
            saveToRegistry("port", port.Text);
            saveToRegistry("ssName", ssName.Text);
            saveToRegistry("passWord", passWord.Text);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            this.Close();
        }

        private void saveToRegistry( String forKey,String value)
        {
           
            Microsoft.Win32.RegistryKey exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SpreadSheet");
            exampleRegistryKey.SetValue(forKey, value);
            exampleRegistryKey.Close();
        }

        private string loadFromRegistry(String forKey)
        {
            string value = "";
            Microsoft.Win32.RegistryKey exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SpreadSheet");

            value = (string)exampleRegistryKey.GetValue(forKey);
            if (ReferenceEquals(value, null))
            {
                value = "";
            }
            exampleRegistryKey.Close();
            return value;
        }


    }
}

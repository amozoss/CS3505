using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SS;
using SpreadsheetGUI;
using System.Text.RegularExpressions;
using System.Threading;
using SpreadsheetUtilities;
using Client;
namespace SpreadsheetGUI
{
    public partial class MainGUI : Form
    {

        private int rowSelected = 0;
        private int colSelected = 0;
        private Spreadsheet spreadsheet;
        private ClientSocket clientCommunication;
        private Thread locationThread;
        /// <summary>
        /// Creates the Windows form.
        /// </summary>
        public MainGUI(String newSpreadsheet)          //Have a string passed in that will be used to open a new form.
        {
            InitializeComponent();

            spreadsheet = new Spreadsheet
                (s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6");

            spreadsheetPanel1.SelectionChanged += LocationHandling;
            nameBox.Text = "A" + 1.ToString();

            contentsBox.Focus();
            contentsBox.Select(contentsBox.Text.Length, 0);
            this.Shown += LoadStartupGUIConnection;
            //location
        }

        private void LoadStartupGUIConnection(object o, EventArgs e)
        {
            CreateOrJoin firstForm = new CreateOrJoin(CreateDelegate, JoinDelegate);
            firstForm.ShowDialog();
        }

        private void CreateDelegate(string IPaddress, string port, string ssName, string psword)
        {
            int num = 0;
            if (Int32.TryParse(port, out num))
            {
                if (!ReferenceEquals(clientCommunication, null))
                    clientCommunication.Leave();
                spreadsheet = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6");
                clientCommunication = new ClientSocket(IPaddress, spreadsheet, Update, num);
                EmptySSPanel();
                clientCommunication.CreateSpreadsheet(ssName, psword);
                refreshSpreadsheet();
            }
        }

        private void JoinDelegate(string IPaddress, string port, string ssName, string psword)
        {
            int num = 0;
            if (Int32.TryParse(port, out num))
            {
                if (!ReferenceEquals(clientCommunication, null))
                    clientCommunication.Leave();
                spreadsheet = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6");
                clientCommunication = new ClientSocket(IPaddress, spreadsheet, Update, num);
                EmptySSPanel();
                clientCommunication.JoinSpreadsheet(ssName, psword);
                refreshSpreadsheet();

            }
        }
        private void Update(string message, bool isError)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (!isError)
                    {
                        refreshSpreadsheet();
                    }
                    else
                    {
                        MessageBox.Show(message, "Error");
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void refreshSpreadsheet()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
                set.Add(s);
            RenewCells(set);
            contentsBox.Focus();
            contentsBox.Select(contentsBox.Text.Length, 0);
            LocationHandling(spreadsheetPanel1);
        }

                private void EmptySSPanel()
        {
            for(int i = 0; i < 99; i++)
                for(int j = 00; j < 26; j++)
                {
                    spreadsheetPanel1.SetValue(((int)'A' + j) - 65, i, "");
                }
        }




        /// <summary>
        /// Sets the row and column to the selected row and column.
        /// </summary>
        /// <param name="sender"></param>
        private void LocationHandling(SpreadsheetPanel sender)
        {
            SpreadsheetPanel panel = sender;
            panel.GetSelection(out rowSelected, out colSelected);
            string name = ((char)(rowSelected + 65)).ToString() + (colSelected + 1).ToString();
            object ject = spreadsheet.GetCellContents(name);
            if (ject is Formula)
                contentsBox.Text = "=" + ject.ToString();
            else
                contentsBox.Text = ject.ToString();
            nameBox.Text = name;
            ject = spreadsheet.GetCellValue(name);
            if (ject is FormulaError)
                valueBox.Text = ((FormulaError)ject).Reason;
            else
                valueBox.Text = spreadsheet.GetCellValue(name).ToString();
            contentsBox.Focus();
            contentsBox.Select(contentsBox.Text.Length, 0);
        }



        private void contentsBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !ReferenceEquals(clientCommunication, null))
            {
                object o = null;
                ISet<string> set = null;
                try
                {
                    set = spreadsheet.SetContentsOfCell(nameBox.Text, contentsBox.Text);
                    o = spreadsheet.GetCellValue(nameBox.Text);                //Get the value of the contents just set.
                }
                catch (CircularException)
                {
                    Warning warn = new Warning("Entering " + contentsBox.Text +
                        " in cell " + nameBox.Text + " will cause a circular dependency.");
                    warn.ShowDialog();
                    spreadsheet.SetContentsOfCell(nameBox.Text, "");
                    set = null;
                }
                catch (Exception l)
                {
                    Warning warn = new Warning(l.Message);
                    warn.ShowDialog();
                    spreadsheet.SetContentsOfCell(nameBox.Text, "");
                    contentsBox.Text = "";
                    set = null;
                }
                if (!ReferenceEquals(set, null))
                {
                    if (o is FormulaError)
                        valueBox.Text = ((FormulaError)o).Reason;
                    else
                    {
                        clientCommunication.ChangeCell(nameBox.Text, contentsBox.Text);
                        spreadsheet.SetContentsOfCell(nameBox.Text, "");
                        //valueBox.Text = o.ToString();

                    }
                    Thread t = new Thread(() => RenewCells(set));
                    t.IsBackground = true;
                    t.Start();
                }
            }
            else if (ReferenceEquals(clientCommunication, null))
                MessageBox.Show("You are not connected to the server", "ERROR!");
        }

        private void RenewCells(ISet<string> set)
        {
            if (!ReferenceEquals(set, null))
            {
                foreach (string s in set)
                {
                    string name = s;
                    char letter = s.ElementAt(0);
                    string number;
                    if (s.Length == 3)
                        number = s.ElementAt(1).ToString() + s.ElementAt(2).ToString();
                    else
                        number = s.ElementAt(1).ToString();
                    if (spreadsheet.GetCellValue(name) is FormulaError)
                        spreadsheetPanel1.SetValue(((int)letter) - 65, Int32.Parse(number) - 1,
                            ((FormulaError)spreadsheet.GetCellValue(name)).Reason);
                    else
                        spreadsheetPanel1.SetValue(((int)letter) - 65, Int32.Parse(number) - 1,
                            spreadsheet.GetCellValue(name).ToString());
                }
            }

        }



        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DemoApplicationContext.getAppContext().RunForm(new Form1(""));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToServer();
        }

        private void saveToServer()
        {
            if (!ReferenceEquals(clientCommunication, null))
                clientCommunication.Save();
        }


        /// <summary>
        /// Triggers the close method, which has an option to save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            int yon = 0;
            ExitWarning warn = new ExitWarning              //Delegate to get the option selected by the user.
                (delegate(int x)
            {
                yon = x;
            });
            warn.ShowDialog();
            if (yon == 1)
            {
                saveToServer();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void contentsBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void helpMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.ShowDialog();
        }

        private void newConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            CreateOrJoin firstForm = new CreateOrJoin(CreateDelegate, JoinDelegate);
            firstForm.ShowDialog();
        }
        

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ReferenceEquals(clientCommunication, null))
                clientCommunication.Undo();
        }
    }
}
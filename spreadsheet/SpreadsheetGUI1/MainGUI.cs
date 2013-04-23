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

        /// <summary>
        /// This method displays the prompt for IP Address, port, spreadhsheet name and password.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void LoadStartupGUIConnection(object o, EventArgs e)
        {
            CreateOrJoin firstForm = new CreateOrJoin(CreateDelegate, JoinDelegate);
            firstForm.ShowDialog();
        }

        /// <summary>
        /// This method is one of the delegates sent to CreateOrJoin, it's call means the user wants to create a spreadsheet.
        /// </summary>
        /// <param name="IPaddress"></param>
        /// <param name="port"></param>
        /// <param name="ssName"></param>
        /// <param name="psword"></param>
        private void CreateDelegate(string IPaddress, string port, string ssName, string psword)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                    {
                        int num = 0;
                        if (Int32.TryParse(port, out num))
                        {
                            if (!ReferenceEquals(clientCommunication, null))
                                clientCommunication.Leave();
                            spreadsheet = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6");
                            clientCommunication = new ClientSocket(IPaddress, changeCell, Update, num, this.SendXML);
                            EmptySSPanel();
                            clientCommunication.CreateSpreadsheet(ssName, psword);
                            refreshSpreadsheet();
                        }
                    });
            }
        }

        /// <summary>
        /// This method is one of the delegates sent to CreateOrJoin, it's call means the user wants to join a spreadsheet.
        /// </summary>
        /// <param name="IPaddress"></param>
        /// <param name="port"></param>
        /// <param name="ssName"></param>
        /// <param name="psword"></param>
        private void JoinDelegate(string IPaddress, string port, string ssName, string psword)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                    {
                        int num = 0;
                        if (Int32.TryParse(port, out num))
                        {
                            if (!ReferenceEquals(clientCommunication, null))
                                clientCommunication.Leave();
                            spreadsheet = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6");
                            clientCommunication = new ClientSocket(IPaddress, changeCell, Update, num, this.SendXML);
                            EmptySSPanel();
                            clientCommunication.JoinSpreadsheet(ssName, psword);
                            refreshSpreadsheet();

                        }
                    });
            }
        }

        /// <summary>
        /// This method is called by the ClientSocket class when errors that need to be printed to the user occur.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError"></param>
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
        /// This method is called refresh all cells that are not empty in the spreadsheet.
        /// </summary>
        public void refreshSpreadsheet()
        {
            this.Invoke((MethodInvoker)delegate
                 {
                     HashSet<string> set = new HashSet<string>();
                     foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
                         set.Add(s);
                     RenewCells(set);
                 });
            //contentsBox.Focus();
            //contentsBox.Select(contentsBox.Text.Length, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        public void changeCell(string name, string contents)
        {
            char letter = name.ElementAt(0);
            string number;
            if (name.Length == 3)
                number = name.ElementAt(1).ToString() + name.ElementAt(2).ToString();
            else
                number = name.ElementAt(1).ToString();
            spreadsheet.SetContentsOfCell(name, contents);
            if (spreadsheet.GetCellValue(name) is FormulaError)
                spreadsheetPanel1.SetValue(((int)letter) - 65, Int32.Parse(number) - 1,
                    ((FormulaError)spreadsheet.GetCellValue(name)).Reason);
            else
                spreadsheetPanel1.SetValue(((int)letter) - 65, Int32.Parse(number) - 1,
                    spreadsheet.GetCellValue(name).ToString());
        }

        private int TranslateColumnNameToIndex(string name)
        {
            return (int)name[0] - 65;

        }

        private void SendXML(string xml)
        {

            spreadsheet.ReadXml(xml);
        }

        private void EmptySSPanel()
        {
            for (int i = 0; i < 99; i++)
                for (int j = 00; j < 26; j++)
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
            {
                clientCommunication.Save();
            }
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
            if (!ReferenceEquals(clientCommunication, null))
            {
                clientCommunication.Leave();
            }
            Thread.Sleep(200);
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
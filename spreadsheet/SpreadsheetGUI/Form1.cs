using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using CustomNetworking;
using System.Net.Sockets;



namespace SS
{
    /// <summary>
    /// A form with a spreadsheet GUI and a spreadsheet model to track cell data. Also provides ways to edit the cell's contents
    /// </summary>
    public partial class SpreadsheetGUI : Form
    {
        private Spreadsheet spreadsheet; // The spreadsheet model for the form. Each new form has its own spreadsheet.
        private string filename; // keeps track of the current file name, if filename is null, the saveAs menu is used
      
        private ClientSocketStuff clientCommunication;
       
       
        
        /**
         * This is used for posting on the UI thread from another thread.  
         * 
            this.Invoke((MethodInvoker)delegate
            {
                valueTextBox.Text = message; // runs on UI thread
            });
         */

        /// <summary>
        /// Creates a new spreadsheetGUI and spreadsheet model
        /// </summary>
        public SpreadsheetGUI()
        {
            InitializeComponent(); 

            // To convert the data from the server to regular data, we should just save it as xml then have 
            // our spreadsheet open it for us.


            // Create and join are figured out here.
            spreadsheet = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6");
            CreateOrJoin firstForm = new CreateOrJoin(CreateDelegate, JoinDelegate);
            firstForm.ShowDialog();
            
            // Create the spreadsheet model and the validator to check if the cell names are correct. 
            
            // registering a method so that it is notified when an event happens.
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(2, 3);
            //clientCommunication = new ClientSocketStuff("localhost", spreadsheet, Update, 1984);

            displaySelection(spreadsheetPanel1); // update display when loaded
        }

        private void CreateDelegate(string IPaddress, string port, string ssName, string psword)
        {
            int num = 0;
            if (Int32.TryParse(port, out num))
            {
                clientCommunication = new ClientSocketStuff(IPaddress, spreadsheet, Update, num);
                clientCommunication.CreateSpreadsheet(ssName, psword);
            }
        }

        private void JoinDelegate(string IPaddress, string port, string ssName, string psword)
        {
            int num = 0;
            if (Int32.TryParse(port, out num))
            {
                clientCommunication = new ClientSocketStuff(IPaddress, spreadsheet, Update, num);
                clientCommunication.JoinSpreadsheet(ssName, psword);
            }
        }
        private void Update(string message)
        {      
            this.Invoke((MethodInvoker)delegate
            {
                displaySelection(spreadsheetPanel1);
            }); 
        }


        


        /// <summary>
        /// Creates a spreadsheetGUI with the content in sheet
        /// </summary>
        /// <param name="sheeet"></param>
        public SpreadsheetGUI(Spreadsheet sheet, string nameOfFile)
        {

            InitializeComponent();

            // Create the spreadsheet model and the validator to check if the cell names are correct. 
            spreadsheet = sheet;
            filename = nameOfFile;

            // registering a method so that it is notified when an event happens.
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(2, 3);

            displaySelection(spreadsheetPanel1); // update display when loaded
        }

        /// <summary>
        /// Updates the panel display, cellDisplayTextBox, contentsTextBox, valueTextBox, and all non-empty Cells
        ///  Every time the selection changes, this method is called with the Spreadsheet as its parameter. 
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            // Get col and row of cell.
            int row, col;
            ss.GetSelection(out col, out row);
            string nameOfCell = "" + GetExcelColumnName(col) + (row + 1); // get cell name





            // Cell Display
            cellDisplayBox.Text = nameOfCell;


            // Set the valueTextBox if there is a formula error make the error message be "###########"
            var valueOfCell = spreadsheet.GetCellValue(nameOfCell); // get value of cell
            string valueOfCellString;

            if (valueOfCell is SpreadsheetUtilities.FormulaError)//*******************************************************************************************************************
            {//***********************************************************************************************************************************************************************
                //clientCommunication.
                                                                                // If it is an FormulaError we don't do anytyhing here.
                valueOfCellString = "##########";
            }
            else
            {   
                /**If is isn't a FormulaError, we send "CHANGE VERSION #\n" to the server.
                 * The state of the client’s local spreadsheet should remain unchanged until approved by the server.
                 Socket.BeginSend("message", SendCallback, null/whatever);
                 Socket.BeginReceive(ReceiveCallback, null/whatever);
                 probably a loop with thread.sleep in it while we wait for the message.
                 */
                valueOfCellString = valueOfCell.ToString();                     
            }


            valueTextBox.Text = valueOfCellString;
            ss.SetValue(col, row, valueOfCellString); // update panel


            var contentsOfCell = spreadsheet.GetCellContents(nameOfCell);
            // Set the contentsTextBox, append the '=' for convience 
            if (contentsOfCell is SpreadsheetUtilities.Formula)
            {
                contentsTextBox.Text = "=" + contentsOfCell.ToString();
            }
            else
            {
                contentsTextBox.Text = contentsOfCell.ToString();
            }


            // update all non empty cells
            IEnumerable<string> nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells();
            foreach (var name33 in nonEmptyCells)
            {
                Match m = Regex.Match(name33, @"^([A-Z]+) *(\d)$");// separate cell name
                if (m.Success)
                {
                    // get the column letter and convert it to a number
                    string translated = m.Groups[1].Value;
                    int column = TranslateColumnNameToIndex(translated);
                    int rowNumber = Int32.Parse(m.Groups[2].Value) - 1;

                    string valueOfDependentCellString;
                    var valueOfDependentCell = spreadsheet.GetCellValue(name33);

                    // Set the value if there is a formula error
                    if (valueOfDependentCell is SpreadsheetUtilities.FormulaError)
                    {
                        valueOfDependentCellString = "##########";
                    }
                    else
                    {
                        valueOfDependentCellString = valueOfDependentCell.ToString();
                    }
                    ss.SetValue(column, rowNumber, valueOfDependentCellString); // update panel
                }
            }

            // put cursor in contents text box
            contentsTextBox.Focus();
            contentsTextBox.Select(contentsTextBox.Text.Length, 0);


        }

        /// <summary>
        /// Returns the column letter from the column number
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private String GetExcelColumnName(int columnNumber)
        {
            return String.Format("{0}", (char)(65 + columnNumber));
        }

        /// <summary>
        /// Returns the column number from the column name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int TranslateColumnNameToIndex(string name)
        {
            return (int)name[0] - 65;

        }





        /// <summary>
        /// Deals with the New menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            //SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetGUI());
            spreadsheet = new Spreadsheet();
            CreateOrJoin cj = new CreateOrJoin(CreateDelegate, JoinDelegate);
            cj.ShowDialog();
        }






        // Deals with the Close menu
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (spreadsheet.Changed)
            {
                DialogResult dialogResult = MessageBox.Show("The document has been modified. Would you like to save before you continue?", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    saveFile();
                }
                else if (dialogResult == DialogResult.No)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }

        }

        /// <summary>
        /// Displays a save file dialog and prompts user to save the file
        /// </summary>
        private void saveFile()
        {
            using (var saveFile = new System.Windows.Forms.SaveFileDialog()) // use the save file dialog class
            {
                saveFile.DefaultExt = "*.ss"; // set the extensions
                saveFile.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*"; // filter results choices
                saveFile.DefaultExt = ".ss"; // Default file extension
                saveFile.FileName = filename; // save off the filename


                DialogResult result = saveFile.ShowDialog(); // show the dialog


                if (result == DialogResult.OK)
                {

                    try
                    {
                        spreadsheet.Save(saveFile.FileName);
                        filename = saveFile.FileName;
                    }
                    catch (Exception except) // Catch all exceptions that may happen
                    {
                        MessageBox.Show(except.Message, "Error");
                    }
                }
            }
        }



        /// <summary>
        /// Updates and saves the spreadsheet contents, all dependent cells, then calls displaySelection
        /// </summary>
        private void updateCells()
        {
            bool validFormula = true; // assume its valid, set to false if invalid formula, circular dependant, invalid name
            string nameOfCell = "";
            try
            {
                //get name of cell
                SpreadsheetPanel ss = spreadsheetPanel1;
                int row, col;
                ss.GetSelection(out col, out row);

                // get cell name
                nameOfCell = "" + GetExcelColumnName(col) + (row + 1);

                // set cell contents @@@
                ISet<string> dependentCells = spreadsheet.SetContentsOfCell(nameOfCell, contentsTextBox.Text);


                // update all dependent cells
                foreach (var name33 in dependentCells)
                {
                    Match m = Regex.Match(name33, @"^([A-Z]+) *(\d)$");// separate cell name
                    if (m.Success)
                    {
                        // get the column letter and convert it to a number
                        string translated = m.Groups[1].Value;
                        int column = TranslateColumnNameToIndex(translated);
                        int rowNumber = Int32.Parse(m.Groups[2].Value) - 1;

                        string valueOfDependentCellString;
                        var valueOfDependentCell = spreadsheet.GetCellValue(name33);

                        // Set the value if there is a formula error
                        if (valueOfDependentCell is SpreadsheetUtilities.FormulaError)
                        {
                            valueOfDependentCellString = "##########";
                        }
                        else
                        {
                            valueOfDependentCellString = valueOfDependentCell.ToString();
                            // @todo this is where we can push the changes to the server

                        }
                        ss.SetValue(column, rowNumber, valueOfDependentCellString); // update panel
                    }
                }
            }

            catch (InvalidNameException) // Invalid name
            {
                MessageBox.Show("Invalid variable name", "Error");
                validFormula = false;
            }
            catch (SpreadsheetUtilities.FormulaFormatException e) //Formula Format Exception
            {
                MessageBox.Show(e.Message, "Formula Error");
                validFormula = false;
            }
            catch (SS.CircularException) // Circular Exception
            {
                MessageBox.Show("A cicular dependency was detected. Make sure the cell's formula doesn't depend on itself.", "Circular Error");
                validFormula = false;
            }
            catch (ArgumentException)
            {

            }
            if (validFormula)
            {
                // send the change to the server
                if (clientCommunication != null)
                    clientCommunication.ChangeCell(nameOfCell, contentsTextBox.Text);
            }
            displaySelection(spreadsheetPanel1); // Update Everything on the spreadsheet panel
        }




        // updates cells when enter is pressed
        private void contentsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateCells();
            }
        }


        // deals with meanu item open
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                try
                {
                    openFile.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*"; // filter results choices
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        if (Path.GetExtension(openFile.FileName) == ".ss")
                        {
                            SpreadsheetApplicationContext appContext = SpreadsheetApplicationContext.getAppContext();

                            // create new GUI and pass a new spreadsheet with the filename to constructor
                            appContext.RunForm(new SpreadsheetGUI(new Spreadsheet(openFile.FileName, s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[0-9]{1,2}$"), s => s.ToUpper(), "ps6"), openFile.FileName));

                        }
                        else
                        {
                            throw new FileLoadException("The file must have a .ss extension");
                        }

                    }
                }
                catch (Exception except) // catch all exceptions that may occur when opening and display a message
                {
                    MessageBox.Show(except.Message, "Error");
                }



            }
        }

        // deals with file save menu item
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save file if it exists, else prompt the save as.
            if (filename != null)
            {
                try
                {
                    spreadsheet.Save(filename);

                }
                catch (Exception except)
                {
                    MessageBox.Show(except.Message, "Error");
                }
            }
            else
            {
                saveFile();
            }
        }

        // deals with form closing 
        private void SpreadsheetGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (spreadsheet.Changed)
            {
                DialogResult dialogResult = MessageBox.Show("The document has been modified. Would you like to save before you continue?", "Warning", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    saveFile();
                }
            }
        }

        // deals with help menu item
        private void changeSelectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click on the cell with the mouse to select", "Change Selection");
        }

        // deals with help menu item
        private void editCellContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select the cell to edit, click on the content's textbox, and press enter to update contents", "Edit Cell Contents");
        }

        //deals with file saveAs menu item
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void SpreadsheetGUI_Load(object sender, EventArgs e)
        {

        }

       













    }
}

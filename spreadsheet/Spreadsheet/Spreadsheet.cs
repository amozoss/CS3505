﻿//Dan Willoughby and Michael Banks
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetUtilities;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace SS
{

    /// <summary>
    /// Spreadsheet is a subclass of AbstractSpreadsheet. 
    /// Cell contents can be set as a string, double, or formula. Cell names must not
    /// create a circular dependency
    /// 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        private Dictionary<string, Cell> spreadsheet; // The string in the dictionary is the name of the cell.
        // A cell has an object as a member variable, which can be  
        // get/set. The object can be a double, string, or formula
        private DependencyGraph dependencyGraph; // refer to XML comment, keeps track of cell dependencies

        /// <summary>
        /// Constructs an empty spreadsheet. A spreadsheet consists of cells and tracts the dependencies of 
        /// each cell
        /// </summary>
        public Spreadsheet()
            : base(s => true, s => s, "default")
        {
            spreadsheet = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();

        }

        /// <summary>
        /// Creates an empty spreadsheet
        /// The spreadsheet uses the provided validity delegate, normalization delegate and verson. 
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            spreadsheet = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();

        }

        /// <summary>
        /// Reads a saved spreadsheet from file and uses it to construct a new spreadsheet. 
        /// The spreadsheet uses the provided validity delegate, normalization delegate and verson. 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            spreadsheet = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            ReadFile(file);
        }

        /// <summary>
        /// Read an xml file
        /// </summary>
        /// <param name="file"></param>
        private void ReadFile(string file)
        {
            // check veriosn
            try
            {
                var version = GetSavedVersion(file);
                if (version != Version)
                    throw new SpreadsheetReadWriteException("Version string does not match");

                // load file
                var doc = XDocument.Load(file);
                var cells = doc.Descendants("cell");

                // read in each cell into spreadsheet
                foreach (var cell in cells)
                {
                    var name = cell.Descendants("name").First().Value;
                    var content = cell.Descendants("contents").First().Value;
                    SetContentsOfCell(name, content);
                }
                // created so its false
                Changed = false;
            }
            // if anything goes wrong the constructor should throw a Spreadsheetreadwriteexception
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

        }

        /// <summary>
        /// Read an xml file
        /// </summary>
        /// <param name="file"></param>
        public void ReadXml(string xml)
        {
            // check veriosn
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNodeList parentNode = xmlDoc.GetElementsByTagName("cell");
                foreach (XmlNode childrenNode in parentNode)
                {
                    var name = childrenNode.SelectSingleNode("name").InnerText;
                    var content = childrenNode.SelectSingleNode("contents").InnerText;
                    Debug.WriteLine("Write cell:", name, content);
                    if(!name.Equals(String.Empty)) 
                        SetContentsOfCell(name, content);
                }
               
                // created so its false
                Changed = false;
            }
            // if anything goes wrong the constructor should throw a Spreadsheetreadwriteexception
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }


        /// <summary>
        /// This doesn't have comments.
        /// </summary>
        public override bool Changed { get; protected set; }


        public override string GetSavedVersion(String filename)
        {
            // spreadsheet version=
            using (XmlReader reader = XmlReader.Create(filename))
            {
                // Skip header and whitespace
                while (reader.NodeType != System.Xml.XmlNodeType.Element)
                    reader.Read();

                // doesn't equal spreadsheet so throw exception
                if (reader.Name != "spreadsheet")
                {
                    throw new SpreadsheetReadWriteException("Unexpected tag! The name of first element was not spreadsheet");
                }
                else
                {
                    //read the version
                    return reader.GetAttribute("version");
                }
            }
        }

        public override void Save(String filename)
        {
            //create file
            try
            {
                var output = XmlWriter.Create(filename);
                // setup top of document
                output.WriteStartDocument();
                output.WriteStartElement("spreadsheet");
                output.WriteAttributeString("version", Version);

                // Only write out non-empty cells
                foreach (var name in GetNamesOfAllNonemptyCells())
                {
                    output.WriteStartElement("cell");

                    output.WriteStartElement("name");
                    output.WriteString(name);
                    output.WriteEndElement(); // name

                    output.WriteStartElement("contents");
                    var content = GetCellContents(name); // get contents
                    if (content is string)  // stirng
                        output.WriteString((string)content);
                    else if (content is double) // dobule
                        output.WriteString(((double)content).ToString());
                    else if (content is Formula)  // formula
                        output.WriteString("=" + ((Formula)content).ToString());
                    else // somethign when wrong
                        throw new SpreadsheetReadWriteException("Invalid content inside of our spreadsheet");
                    output.WriteEndElement();// content

                    output.WriteEndElement(); // cell
                }
                output.WriteEndElement(); // spreadsheet
                output.Close();
                // saved so its false
                Changed = false;
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("The file cannot be found");
            }
        }


        public override object GetCellValue(String name)
        {
            name = Normalize(name);
            checkForValidName(name);
            Cell cell;

            if (spreadsheet.TryGetValue(name, out cell))
            {
          
                return cell.getValue();
            }
            else
            {
                // it is an empty cell so its contents is an empty string.
                return "";
            }
        }


        public override ISet<String> SetContentsOfCell(String name, String content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            checkForValidName(name);
            name = Normalize(name);

            // try to parse double
            double outDouble;
            if (Double.TryParse(content, out outDouble))
            {
                recalc(SetCellContents(name, outDouble));

            }
            // begins with '='
            else if (content.Length > 0 && content[0] == '=')
            {

                recalc(SetCellContents(name, new Formula(content.Substring(1), base.Normalize, base.IsValid)));

            }
            else
            {
                // otherwise its a string
                recalc(SetCellContents(name, content));
            }
            // changed so its true
            Changed = true;

            return generateReturnSetForName(name);
        }

        /// <summary>
        /// Recalculates Cells that contain formulas
        /// </summary>
        /// <param name="namesInSet"></param>
        private void recalc(ISet<string> namesInSet)
        {
            var val = namesInSet;

            // names of all the cells it depends on
            foreach (string s in val)
            {
                var foo = GetCellContents(s);
                if (foo is Formula)
                {
                    SetCellContents(s, (Formula)foo); // reset the contents
                }
            }
        }


        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<string> nonEmpty = new List<string>();
            Dictionary<string, Cell>.KeyCollection someKeys = spreadsheet.Keys;
            foreach (string c in someKeys.ToList())
            {
                if (!"".Equals(GetCellContents(c).ToString())) // if its not empty
                {
                    nonEmpty.Add(c);
                }
            }
            return nonEmpty;
        }


        public override object GetCellContents(string name)
        {
            checkForValidName(name);
            name = Normalize(name);

            Cell cell;

            if (spreadsheet.TryGetValue(name, out cell))
            {
                // return the contents of the cell
                return cell.getContents();
            }
            else
            {
                // it is an empty cell so its contents is an empty string.
                return "";
            }
        }


        protected override ISet<string> SetCellContents(string name, double number)
        {
            checkForValidName(name);
            if (spreadsheet.ContainsKey(name))
            {
                dependencyGraph.ReplaceDependees(name, new List<string>()); // get rid of old dependancies
                // if name already exists change its contents
                spreadsheet[name].setContents(number);
                spreadsheet[name].setValue(number);

            }
            else
            {

                // create a new cell with contents
                spreadsheet.Add(name, new Cell(number, number));
            }

            return generateReturnSetForName(name);
        }


        protected override ISet<string> SetCellContents(string name, string text)
        {
            checkForValidName(name);


            if (spreadsheet.ContainsKey(name))
            {
                // if name already exists change its contents
                dependencyGraph.ReplaceDependees(name, new List<string>()); // get rid of old dependencies
                spreadsheet[name].setContents(text);
                spreadsheet[name].setValue(text);
            }
            else
            {
                // create a new cell with contents
                spreadsheet.Add(name, new Cell(text, text));
            }

            return generateReturnSetForName(name);
        }


        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            checkForValidName(name);


            object savedContents = GetCellContents(name);


            try
            {
                if (spreadsheet.ContainsKey(name))
                {
                    // if name already exists change its contents
                    dependencyGraph.ReplaceDependees(name, formula.GetVariables());
                    spreadsheet[name].setContents(formula);
                    spreadsheet[name].setValue(formula.Evaluate(calculateValueOfCell));

                }
                else
                {
                    // create a new cell with contents
                    spreadsheet.Add(name, new Cell(formula, formula.Evaluate(calculateValueOfCell)));
                    dependencyGraph.ReplaceDependees(name, formula.GetVariables());
                }

                return generateReturnSetForName(name);
            }
            // reset the value back to the old one and then throw the circular exception
            catch (CircularException e)
            {
                // its a string 
                if (savedContents is string)
                {
                    string stringContent = (string)savedContents;
                    SetContentsOfCell(name, stringContent);
                }
                // its a double call the set double
                else if (savedContents is double)
                {
                    double doubleContent = (double)savedContents;
                    SetCellContents(name, doubleContent);
                }
                // its a formula call the set formula 
                else if (savedContents is Formula)
                {
                    Formula formulaContent = (Formula)savedContents;
                    SetCellContents(name, formulaContent);
                }
                throw e;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        /// <summary>
        ///  Given a variable symbol as its parameter, calculateValueOfCell returns the
        /// cell's value (if it has one) or throws an ArgumentException (otherwise).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double calculateValueOfCell(string name)
        {
            if (this.GetCellValue(name) is double )
            {
                return (double)this.GetCellValue(name);
            }
            else
            {
                   throw new ArgumentException();
            }
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // This method has specific Exceptions thrown..
            Cell cell;
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            else if (!Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$") || !base.IsValid(name))
            {
                throw new InvalidNameException();
            }
            else if (!spreadsheet.TryGetValue(name, out cell))
            {
                throw new InvalidNameException();
            }

            // get the dependents 
            IEnumerable<string> set = dependencyGraph.GetDependents(name);
            return set;
        }


        /// <summary>
        /// Returns true if name is valid and the delegate isValid is true, throws InvalidNameException otherwise
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool checkForValidName(string name)
        {
            if (name == null)
            {
                throw new InvalidNameException();
            }

            if (Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$") && base.IsValid(name))
            {
                return true;
            }
            else
            {
                throw new InvalidNameException();
            }
        }


        /// <summary>
        /// Generates the Set that should be returned for SetCellContents
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ISet<string> generateReturnSetForName(string name)
        {
            ISet<string> setToReturn = new HashSet<string>();

            // add items to setToReturn
            IEnumerable<string> moreValues = GetCellsToRecalculate(name);
            foreach (string s in moreValues)
            {
                setToReturn.Add(s);
            }
            return setToReturn;
        }


    }
}

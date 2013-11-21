using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    
    
    /// <summary>
    ///This is a test class for AbstractSpreadsheetTest and is intended
    ///to contain all AbstractSpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AbstractSpreadsheetTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        internal virtual AbstractSpreadsheet CreateAbstractSpreadsheet()
        {
            // TODO: Instantiate an appropriate concrete class.
            AbstractSpreadsheet target = null;
            return target;
        }

        internal virtual AbstractSpreadsheet_Accessor CreateAbstractSpreadsheet_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            AbstractSpreadsheet_Accessor target = null;
            return target;
        }
        /*
        /// <summary>
        ///A test for GetCellsToRecalculate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Spreadsheet.dll")]
        public void GetCellsToRecalculateTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AbstractSpreadsheet_Accessor target = new AbstractSpreadsheet_Accessor(param0); // TODO: Initialize to an appropriate value
            ISet<string> names = null; // TODO: Initialize to an appropriate value
            IEnumerable<string> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<string> actual;
            actual = target.GetCellsToRecalculate(names);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCellsToRecalculate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Spreadsheet.dll")]
        public void GetCellsToRecalculateTest1()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AbstractSpreadsheet_Accessor target = new AbstractSpreadsheet_Accessor(param0); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            IEnumerable<string> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<string> actual;
            actual = target.GetCellsToRecalculate(name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
       
        /// <summary>
        ///A test for Visit
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Spreadsheet.dll")]
        public void VisitTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AbstractSpreadsheet_Accessor target = new AbstractSpreadsheet_Accessor(param0); // TODO: Initialize to an appropriate value
            string start = string.Empty; // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            ISet<string> visited = null; // TODO: Initialize to an appropriate value
            LinkedList<string> changed = null; // TODO: Initialize to an appropriate value
            target.Visit(start, name, visited, changed);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
       * */
    }
}

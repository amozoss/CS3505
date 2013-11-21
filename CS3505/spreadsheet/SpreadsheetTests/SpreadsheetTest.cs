using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.IO;
using System.Xml;
using System.Threading;

namespace SpreadsheetTests
{


    /// <summary>
    ///This is a test class for SpreadsheetTest and is intended
    ///to contain all SpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpreadsheetTest
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



        /// <summary>
        ///A test for Spreadsheet Constructor
        ///</summary>
        [TestMethod()]
        public void SpreadsheetConstructorTest()
        {
            AbstractSpreadsheet target = new Spreadsheet();
            Assert.IsTrue(target != null);

            AbstractSpreadsheet target1 = new Spreadsheet(s=>true,s=>s.ToUpper(),"default");
            Assert.IsTrue(target1 != null);

        }

        /// <summary>
        ///A test for GetCellContents string
        ///</summary>
        [TestMethod()]
        public void GetCellContentsTest1()
        {
            // string
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("A1", "This is Text:");
            Assert.AreEqual("This is Text:", target.GetCellContents("A1"));

            // double
            target = new Spreadsheet();
            target.SetContentsOfCell("A1", "2.33333");
            Assert.AreEqual(2.33333, (double)target.GetCellContents("A1"), 1e-6);

            //formula
            target = new Spreadsheet();
            target.SetContentsOfCell("A1", "=1+B1+B1");
            target.SetContentsOfCell("B1", "1.5");
            Assert.AreEqual(new Formula("1+B1+B1"), target.GetCellContents("A1"));

            // empty test
            target = new Spreadsheet();
            Assert.AreEqual("", target.GetCellContents("A1"));

        }




        /// <summary>
        ///A test for GetCellContents invalid name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest2()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("A1", "This is Text:");
            Assert.AreEqual("This is Text:", target.GetCellContents("A1ad"));
        }




        /// <summary>
        ///A test for GetDirectDependents
        ///For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        ///</summary>
        [TestMethod()]
        public void GetDirectDependentsTest1()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor(); // TODO: Initialize to an appropriate value
            target.SetContentsOfCell("A1", "3");
            target.SetContentsOfCell("B1", "=A1 * A1");
            target.SetContentsOfCell("C1", "=    B1 + A1");
            target.SetContentsOfCell("D1", "=       B1 - C1");
            List<string> expected = new List<string>();
            expected.Add("B1");
            expected.Add("C1");
            IEnumerable<string> actual = target.GetDirectDependents("A1");
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for GetDirectDependents doesn't exisit 
        ///
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependentsTest2()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor();

            List<string> expected = new List<string>();
            IEnumerable<string> actual = target.GetDirectDependents("A1");
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for GetDirectDependents name is null
        ///
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectDependentsTest3()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor(); // TODO: Initialize to an appropriate value

            List<string> expected = new List<string>();
            string nullName = null;
            IEnumerable<string> actual = target.GetDirectDependents(nullName);
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for GetDirectDependents invalid name
        ///
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependentsTest4()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor(); // TODO: Initialize to an appropriate value

            List<string> expected = new List<string>();
            string name = "A22";
            target.SetContentsOfCell("A2", "4");
            IEnumerable<string> actual = target.GetDirectDependents(name);


        }

        /// <summary>
        ///A test for GetDirectDependents invalid name
        ///
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependentsTest5()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor(); // TODO: Initialize to an appropriate value

            List<string> expected = new List<string>();
            string name = "A022";
            //   target.SetContentsOfCell("A02", 4);
            IEnumerable<string> actual = target.GetDirectDependents(name);


        }


        /// <summary>
        ///A test for GetNamesOfAllNonemptyCells
        ///</summary>
        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest1()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            expected.Add("D1");
            expected.Add("E1");
            expected.Add("F1");
            expected.Add("G1");
            target.SetContentsOfCell("A1", "4");
            target.SetContentsOfCell("B1", "4");
            target.SetContentsOfCell("C1", "4");
            target.SetContentsOfCell("D1", " 4");
            target.SetContentsOfCell("E1", "HI");
            target.SetContentsOfCell("F1", "=1+2");
            target.SetContentsOfCell("G1", "4");
            IEnumerable<string> actual = target.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, new List<string>(actual));

            // nothing at all
            target = new Spreadsheet();
            expected = new List<string>();
            actual = target.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }



        /// <summary>
        ///A test for SetContentsOfCell
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTestText1()
        {
            Spreadsheet target = new Spreadsheet();
            string name = "A1";
            string text = "This is Text:";
            List<string> expected = new List<string>();
            expected.Add("A1");
            ISet<string> actual;
            actual = target.SetContentsOfCell(name, text);
            CollectionAssert.AreEqual(expected, new List<string>(actual));

            // replace 
            target = new Spreadsheet();
            expected = new List<string>();
            expected.Add("A1");
            target.SetContentsOfCell("A1", "Original!");
            actual = target.SetContentsOfCell("A1", "Not Original!");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual("Not Original!", target.GetCellContents("A1"));

        }

        /// <summary>
        ///A test for SetContentsOfCell invalid name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestText2()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("A1");
            ISet<string> actual = target.SetContentsOfCell("A1A", "This is Text:");
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for SetContentsOfCell null name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestText3()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("A1");
            string nullString = null;
            ISet<string> actual = target.SetContentsOfCell(nullString, "This is Text:");
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for SetContentsOfCell null text
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTestText4()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            string nullText = null;
            expected.Add("A1");
            ISet<string> actual = target.SetContentsOfCell("a1", nullText);
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }



        /// <summary>
        ///A test for SetContentsOfCell replace contents null
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTestText5_1()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            string nullText = null;
            expected.Add("A1");
            target.SetContentsOfCell("A1", "Original!");
            ISet<string> actual = target.SetContentsOfCell("A1", nullText);
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual("Not Original!", target.GetCellContents("A1"));

        }

        /// <summary>
        ///A test for SetContentsOfCell
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTestDouble1()
        {
            Spreadsheet target = new Spreadsheet();
            string name = "B1";
            string number = "=5";
            List<string> expected = new List<string>();
            expected.Add("B1");
            ISet<string> actual;
            actual = target.SetContentsOfCell(name, number);
            CollectionAssert.AreEqual(expected, new List<string>(actual));

            // replace contents
            target = new Spreadsheet();
            expected = new List<string>();
            expected.Add("B1");
            target.SetContentsOfCell("B1", " 2.555");
            actual = target.SetContentsOfCell("B1", " 2.2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual(2.2, (double)target.GetCellContents("B1"), 1e-9);

            target = new Spreadsheet();
            expected = new List<string>();
            expected.Add("B1");
            target.SetContentsOfCell("B1", "2.555");
            actual = target.SetContentsOfCell("B1", "2.2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual(2.2, target.GetCellContents("B1"));
        }


        /// <summary>
        ///A test for SetContentsOfCell invalid name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestDouble2()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("B1");
            ISet<string> actual = target.SetContentsOfCell("B#1", "2.2");
          //  CollectionAssert.AreEqual(expected, new List<string>(actual));
        }

        /// <summary>
        ///A test for SetContentsOfCell invalid name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestDouble2_1()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("B1");
            ISet<string> actual = target.SetContentsOfCell("B01", "2.2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
        }

        /// <summary>
        ///A test for SetContentsOfCell invalid name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestDouble2_2()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("B1");
            ISet<string> actual = target.SetContentsOfCell("B", "2.2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
        }

      

        /// <summary>
        ///A test for SetContentsOfCell invalid name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestDouble2_4()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("B1");
            ISet<string> actual = target.SetContentsOfCell("1B000001", "2.2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
        }
        /// <summary>
        ///A test for SetContentsOfCell null
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestDouble3()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("A1");
            string nullString = null;
            ISet<string> actual = target.SetContentsOfCell(nullString, "2.2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
        }





        /// <summary>
        ///A test for SetContentsOfCell
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTestFormula1()
        {
            Spreadsheet target = new Spreadsheet();
            string name = "C1";
            string formula = "1+B1+B1";
            List<string> expected = new List<string>();
            expected.Add("C1");
            ISet<string> actual;
            target.SetContentsOfCell("B1", "5");
            actual = target.SetContentsOfCell(name, formula);
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            //dependency in formula
            target = new Spreadsheet();
            expected = new List<string>();
            expected.Add("D1");
            expected.Add("C1");
            target.SetContentsOfCell("B1", "5");
            target.SetContentsOfCell("D1", "=1+B1");
            target.SetContentsOfCell("C1", "=1+B1+D1");
            actual = target.SetContentsOfCell("D1", "=1+B1");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
        }



        /// <summary>
        ///A test for SetContentsOfCell change existing formula
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTestFormula2_1()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("B1");
            expected.Add("N1");
            expected.Add("M1");
            expected.Add("L1");
            expected.Add("K1");
            expected.Add("J1");
            expected.Add("I1");
            expected.Add("H1");
            expected.Add("G1");
            expected.Add("F1");
            expected.Add("E1");
            expected.Add("D1");

            target.SetContentsOfCell("B1", "5");
            target.SetContentsOfCell("D1", "=1+B1");
            target.SetContentsOfCell("E1", "=22+B1");
            target.SetContentsOfCell("F1", "=22+B1");
            target.SetContentsOfCell("G1", "=22+B1");
            target.SetContentsOfCell("H1", "=22+B1");
            target.SetContentsOfCell("I1", "=22+B1");
            target.SetContentsOfCell("J1", "=22+B1");
            target.SetContentsOfCell("K1", "=22+B1");
            target.SetContentsOfCell("L1", "=22+B1");
            target.SetContentsOfCell("M1", "=22+B1");
            target.SetContentsOfCell("N1", "=22+B1");
            ISet<string> actual = target.SetContentsOfCell("B1", "7");
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for SetContentsOfCell null name test
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTestFormula3()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("C1");
            expected.Add("B1");
            string nullString = null;
            ISet<string> actual = target.SetContentsOfCell(nullString, "=1+B1+B1");
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for SetContentsOfCell null formula test
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTestFormula4()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("C1");
            expected.Add("B1");
            string nullFormula = null;
            ISet<string> actual = target.SetContentsOfCell("a1", nullFormula);
            CollectionAssert.AreEqual(expected, new List<string>(actual));

        }

        /// <summary>
        ///A test for SetContentsOfCell replace contents
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTestFormula5()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("C1");
            target.SetContentsOfCell("C1", "=1+B1+D1");
            ISet<string> actual = target.SetContentsOfCell("C1", "= 1+B1+B1");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual(new Formula("1+B1+B1"), target.GetCellContents("C1"));

        }

        /// <summary>
        ///A test for SetContentsOfCell replace contents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTestFormula5_1()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("C1");
            expected.Add("B1");
            target.SetContentsOfCell("C1", "= 1+B1+D1");
            string nullForm = null;
            ISet<string> actual = target.SetContentsOfCell("C1", nullForm);
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual(new Formula("1+B1+B1"), target.GetCellContents("C1"));

        }

        /// <summary>
        ///A test for SetContentsOfCell circular dependency
        ///
        /// A circular dependency exists when a cell depends on itself.
        /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
        /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
        /// dependency.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCellTestFormula6()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("C1");
            expected.Add("B1");
            ISet<string> actual = target.SetContentsOfCell("C1", "5");
            target.SetContentsOfCell("B1", "6");
            target.SetContentsOfCell("A1", "7");

            target.SetContentsOfCell("C1", "=A1*2");
            target.SetContentsOfCell("B1", "=C1*2");
            target.SetContentsOfCell("A1", "=B1*2");
            Assert.AreEqual(expected, new List<string>(actual));
            //          Assert.AreEqual("A1*2"), target.GetCellContents("C1"));

        }

        /// <summary>
        ///A test for SetContentsOfCell dependecies in other formulas
        ///
        ///For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTestFormula7()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> expected = new List<string>();
            expected.Add("B1");
            expected.Add("C1");
            target.SetContentsOfCell("A1", "5");
            target.SetContentsOfCell("A3", "15");
            target.SetContentsOfCell("A4", "15");
            target.SetContentsOfCell("A5", "15");
            target.SetContentsOfCell("A6", " 15");
            target.SetContentsOfCell("A2", "=(A1*2)+A3+A4+A5+A6");
            target.SetContentsOfCell("B1", "=(A1*2)+A2");
            target.SetContentsOfCell("C1", "=B1+A1");
            ISet<string> actual = target.SetContentsOfCell("B1", "=(A1*2)+A2");
            CollectionAssert.AreEqual(expected, new List<string>(actual));
            Assert.AreEqual(new Formula("B1+A1"), target.GetCellContents("C1"));

        }
        /*
                /// <summary>
                ///A test for SetContentsOfCell invalid formula
                ///
                ///For example, if name is A1 is "hi", B1 contains A1*2, and C1 contains B1+A1, the
                /// set {A1, B1, C1} is returned.
                ///</summary>
                [TestMethod()]
                [ExpectedException(typeof(FormatException))]
                public void SetContentsOfCellTestFormula8()
                {
                    Spreadsheet target = new Spreadsheet();
                    List<string> expected = new List<string>();
                    expected.Add("C1");
                    target.SetContentsOfCell("A1", "hi");
                    target.SetContentsOfCell("B1", new Formula("A1*2"));

                    ISet<string> actual = target.SetContentsOfCell("C1", new Formula("B1+A1"));
                    CollectionAssert.AreEqual(expected, new List<string>(actual));
                    Assert.AreEqual(new Formula("B1+A1"), target.GetCellContents("C1"));

                }*/
        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("AA");
        }

        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1A", "1.5");
        }

        [TestMethod()]
        public void Test6()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test11()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test13()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "2");
        }



        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test17()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15.0, s.GetCellContents("A2"));
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test18()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test21()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test22()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test23()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SetEquals(new HashSet<string>() { "A1" }));
        }

        [TestMethod()]
        public void Test25()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", " 17.2");
            s.SetContentsOfCell("C1", "5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test26()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "5").SetEquals(new HashSet<string>() { "C1" }));
        }

        [TestMethod()]
        public void Test27()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SetEquals(new HashSet<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "= A2+A3");
            s.SetContentsOfCell("A1", " 2.5");
            Assert.AreEqual(2.5, s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test29()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test30()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        [TestMethod()]
        public void Test35()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
            }
        }

        [TestMethod()]
        public void Test39()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try
            {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        [TestMethod()]
        public void Test43()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            HashSet<string> firstCells = new HashSet<string>();
            HashSet<string> lastCells = new HashSet<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.Add("A1" + i);
                lastCells.Add("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SetEquals(firstCells));
            ISet<string> mine = s.SetContentsOfCell("A1499", " 0.0");
            ISet<string> theirs = lastCells;
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0.0").SetEquals(lastCells));
        }


        [TestMethod()]
        public void Test47()
        {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test48()
        {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test49()
        {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test50()
        {
            RunRandomizedTest(50, 2521);
        }

        public void RunRandomizedTest(int seed, int size)
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }


        /// <summary>
        ///A test for Spreadsheet Constructor valid
        ///</summary>
        [TestMethod()]
        public void SpreadsheetConstructorTest1()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\validspreadsheet.xml"; // the .. means up a directory

           
            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
            Assert.AreEqual(2.0, (double)target.GetCellValue("a2"));

        }

        /// <summary>
        ///A test for Spreadsheet Constructor invalid
        ///</summary>
        [TestMethod()]
        
        public void SpreadsheetConstructorTest1_1()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\invalidxmlfile1.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
           // Assert.AreEqual(2.0, (double)target.GetCellValue("a2"));

        }

        /// <summary>
        ///A test for Spreadsheet Constructor invalid version
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetConstructorTest1_2()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\invalidxmlfile2.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
           // Assert.AreEqual(2.0, (double)target.GetCellValue("a2"));

        }

        /// <summary>
        ///A test for Spreadsheet Constructor invalid version
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SpreadsheetConstructorTest1_3()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\invalidxmlfile3.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
            //Assert.AreEqual(2.0, (double)target.GetCellValue("a2"));

        }
        /// <summary>
        ///A test for Spreadsheet C
        ///</summary>
        [TestMethod()]
        public void SpreadsheetConstructorTest2()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\validspreadsheet.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
            Assert.AreEqual(2.0, (double)target.GetCellValue("a2"));
            target.Save(@"output22.xml");
            var output = File.ReadAllText("output22.xml");
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet version=\"dan1\"><cell><name>A1</name><contents>2</contents></cell><cell><name>A2</name><contents>=A1</contents></cell></spreadsheet>", output);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\validspreadsheet.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
            Assert.AreEqual(2.0, (double)target.GetCellValue("a2"));
            target.Save(@"output22.xml");
            var output = File.ReadAllText("output22.xml");
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet version=\"dan1\"><cell><name>A1</name><contents>2</contents></cell><cell><name>A2</name><contents>=A1</contents></cell></spreadsheet>", output);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void loadStringXmlTest()
        {
            Spreadsheet target = new Spreadsheet();
            string s = "<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet version=\"dan1\"><cell>" +
               "<name>A1</name><contents>2</contents></cell><cell><name>A2</name><contents>=A1</contents>" +
                "</cell><cell><name>A3</name><contents>Hello</contents></cell></spreadsheet>";
            target.ReadXml(s);
           
        }

        /// <summary>
        ///A test for GetSavedVersion
        ///</summary>
        [TestMethod()]
        public void GetSavedVersionTest()
        {

            string existing_path = @"..\..\..\SpreadSheetTests\validspreadsheet.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
            Assert.AreEqual("dan1", target.GetSavedVersion(existing_path));
           
        }

        /// <summary>
        ///A test for GetCellValue
        ///</summary>
        [TestMethod()]

        public void GetCellValueTest()
        {
            Spreadsheet target = new Spreadsheet();
            string name = "A1";
            target.SetContentsOfCell(name, "88.0");

            Assert.AreEqual(88.0, (double)target.GetCellValue(name), 1e-9);


            target = new Spreadsheet();
            name = "A1";
            target.SetContentsOfCell(name, "88.0");

            Assert.AreEqual(88.0, (double)target.GetCellValue(name), 1e-9);


            target = new Spreadsheet();
            name = "A1";
            target.SetContentsOfCell(name, "10.0");
            target.SetContentsOfCell("A2", "=A1 + A1");
            Assert.AreEqual(20.0, (double)target.GetCellValue("A2"), 1e-9);

            target = new Spreadsheet();
            name = "A1";
            target.SetContentsOfCell(name, "10.0");
            target.SetContentsOfCell("A4", "5");
            target.SetContentsOfCell("A3", "=A1 + A4");
            target.SetContentsOfCell("A2", "=A1 + A3");

            Assert.AreEqual(25.0, (double)target.GetCellValue("A2"), 1e-9);

            target = new Spreadsheet();
            name = "A1";
            target.SetContentsOfCell(name, "10.0");
            target.SetContentsOfCell("A4", "5");
            target.SetContentsOfCell("A3", "=A1 + A4");
            target.SetContentsOfCell("A2", "=A1 + A3");
            target.SetContentsOfCell("A3", "5");
            Assert.AreEqual(15.0, (double)target.GetCellValue("A2"), 1e-9);

        }

        /// <summary>
        ///A test for GetCellValue formula error undefined variable
        ///</summary>
        [TestMethod()]

        public void GetCellValueTest1()
        {
            Spreadsheet target = new Spreadsheet();
            target = new Spreadsheet();

            target.SetContentsOfCell("A1", "10.0");

            target.SetContentsOfCell("A3", "=A1 + A4");
            target.SetContentsOfCell("A4", "5");
            target.SetContentsOfCell("A2", "=A1 + A3");
            Assert.AreEqual(15.0, (double)target.GetCellValue("A3"), 1e-9);

            Assert.AreEqual(25.0, (double)target.GetCellValue("A2"), 1e-9);
        }

        /// <summary>
        ///A test for Changed
        ///</summary>
        [TestMethod()]
        //[DeploymentItem("Spreadsheet.dll")]
        public void ChangedTest()
        {
            string existing_path = @"..\..\..\SpreadSheetTests\validspreadsheet.xml"; // the .. means up a directory


            Spreadsheet target = new Spreadsheet(existing_path, s => true, s => s.ToUpper(), "dan1");
            Assert.AreEqual(false, target.Changed);
            target.SetContentsOfCell("A1", "33");
            Assert.AreEqual(true, target.Changed);
        }

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod()]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod()]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod()]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod()]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod()]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod()]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod()]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod()]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod()]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod()]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod()]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod()]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod()]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod()]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod()]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("q:\\missing\\save.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
        }

        [TestMethod()]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod()]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod()]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod()]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod()]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod()]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod()]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod()]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
        [TestMethod()]
        public void LongFormulaTest()
        {
            object result = "";
            Thread t = new Thread(() => LongFormulaHelper(out result));
            t.Start();
            t.Join(60 * 1000);
            if (t.IsAlive)
            {
                t.Abort();
                Assert.Fail("Computation took longer than 60 seconds");
            }
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a0 + a1");
                int i = 0;
                int depth = 100;
                for (i = 0; i < depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1e-1);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), s.GetCellValue("sum1"));
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 1e-1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

    }
}

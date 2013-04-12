using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace SpreadsheetTests
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CodedUITest1
    {
        ApplicationUnderTest app;
        public CodedUITest1()
        {
        }

        [TestMethod]
        public void CodedUITest_simpleAdd()
        {

            this.UIMap.adding_formula();

           

            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        }

        [TestMethod]
        public void CodedUITest_saveAs()
        {

          
            this.UIMap.save_test_1();



            this.UIMap.Testing_saveas();
            this.UIMap.save_existing();
        }

    

        [TestMethod]
        public void CodedUITest_saveAs_extensions()
        {

           // this.UIMap.save_with_different_ext();
            this.UIMap.insure_ss_ext();

           
        }

        [TestMethod]
        public void CodedUITest_openAndSave()
        {

            this.UIMap.open_wrong_ext();

            this.UIMap.save_open2();

            //  this.UIMap.open_save1();



        }

        [TestMethod]
        public void CodedUITest_invalidFormula()
        {

            this.UIMap.invalidFormula_syntax();

            this.UIMap.circular_ex();

            // this.UIMap.invalidSyntax_circularException();

            this.UIMap.invalidFormula();
            this.UIMap.Test_close_button1();
        }

        [TestMethod]
        public void CodedUITest_menuTesting()
        {

           
            this.UIMap.file_new();

            this.UIMap.help_menu();


            this.UIMap.close();
         
        }
        [TestMethod]
        public void CodedUITest_closeMenuTesting1()
        {


            
            this.UIMap.closeButton_save();
           
        }
        [TestMethod]
        public void CodedUITest_closeMenuTesting2()
        {



            
            this.UIMap.menuClose_save();
        }
        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {        
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            app = ApplicationUnderTest.Launch(@"..\..\..\SpreadsheetGUI\bin\Debug\SpreadsheetGUI.exe");

        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {        
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            app.Close();
        }

        #endregion

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
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}

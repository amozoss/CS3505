using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpreadsheetUtilities;

namespace SS
{
    
    /// <summary>
    /// A Cell has contents, which can be a double, string, or formula
    /// </summary>
    public class Cell 
    {
        private object cellContents; // the contents can be a double, string, or formula
        private object cellValue; // the value can be a string, double or formula error
        /// <summary>
        /// Consturcts a cell with contents and value
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="value"></param>
        public Cell (object contents, object value)
        {
            if (contents != null)
            {
                cellContents = contents;
                cellValue = value;
               
            }
          
        }
        /// <summary>
        /// Get the contents of the cell
        /// </summary>
        /// <returns></returns>
        public object getContents()
        {
            return cellContents;
        }


        /// <summary>
        /// Sets the contents of the cell to contents
        /// </summary>
        /// <param name="contents"></param>
        public void setContents(object contents)
        {
            cellContents = contents;
            // evaluate if it is a formula
           
        }

        /// <summary>
        /// Get the value of the cell
        /// </summary>
        /// <returns></returns>
        public object getValue()
        {

            return cellValue;
        }

        /// <summary>
        /// Sets the value of the cell to value
        /// </summary>
        /// <param name="value"></param>
        public void setValue(object value)
        {
            cellValue = value;
           

        }
    }

    

}

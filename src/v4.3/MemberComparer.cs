using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAS.Tasks.CatalogFormats
{
    /// <summary>
    /// For sorting the items in the Members list.  
    /// Straight comparisons except for DateTime columns
    /// Need to cast those to the proper values for comparing
    /// </summary>
    class MemberComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public MemberComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public MemberComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }

        public int Compare(object x, object y)
        {
            int returnVal = -1;

            // If DateTime is in the Tag, cast it
            if (((ListViewItem)x).SubItems[col].Tag is DateTime)
            {
                DateTime val1 = (DateTime)((ListViewItem)x).SubItems[col].Tag;
                DateTime val2 = (DateTime)((ListViewItem)y).SubItems[col].Tag;
                returnVal = val1 > val2 ? 1 : -1;
            }
            else
            returnVal =
              String.Compare(((ListViewItem)x).SubItems[col].Text,
                            ((ListViewItem)y).SubItems[col].Text);
  
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;

            return returnVal;
        }
    }
}

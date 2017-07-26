using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CatalogExplorer
{
    public partial class ListingViewForm : Form
    {
        public string Content { set; get;  }

        public ListingViewForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            txtFormatOut.Text = Content;
            base.OnLoad(e);
        }

        /// <summary>
        /// Copy the contents of the window to the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(txtFormatOut.Text);
            }
            catch { }
        }
    }
}

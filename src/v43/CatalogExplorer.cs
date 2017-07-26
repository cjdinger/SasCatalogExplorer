// ---------------------------------------------------------------
// Copyright 2004, SAS Institute Inc.
// ---------------------------------------------------------------

// for XMLTextWriter and XMLTextReader
	// for StreamReader and StreamWriter
	// for StringBuilder
using System.Windows.Forms;
using SAS.Shared.AddIns;
using SAS.Tasks.Toolkit;
// for access to the Add-in interfaces

namespace CatalogExplorer
{
	/// <summary>
	/// Present a UI for viewing SAS catalog entries
	/// </summary>
    [ClassId("6E2E0060-D11A-499C-BC68-20ECD7AA06EA")]
    [InputRequired(InputResourceType.None)]
    [IconLocation("CatalogExplorer.icons.catalog.ico")]
	public class CatalogExplorer : SAS.Tasks.Toolkit.SasTask
	{
		public CatalogExplorer()
		{
            InitializeComponent();
		}

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CatalogExplorer));
            // 
            // CatalogExplorer
            // 
            this.ProcsUsed = "CATALOG, FORMAT";
            this.ProductsRequired = "Base";
            this.RequiresData = false;
            resources.ApplyResources(this, "$this");

        }

        // use a static flag to ensure we show only one instance (per process)
        internal static bool isOneShowing = false;
        public override ShowResult Show(IWin32Window Owner)
		{
            if (!isOneShowing)
            {
                CatalogExplorerForm dlg = new CatalogExplorerForm();
                dlg.Consumer = Consumer;
                // allow modeless operation
                dlg.Show(Owner);
            }
			return ShowResult.Canceled;
		}

	}
}

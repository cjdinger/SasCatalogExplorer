using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SAS.Tasks.Toolkit;
using SAS.Shared.AddIns;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    [ClassId("5175FEA8-BD1E-4735-894E-E443DD1507DC")]
    [Version(4.2)]
    [InputRequired(InputResourceType.None)]
    [IconLocation("SAS.Tasks.Examples.CatalogExplorer.catalog.ico")]
    public class CatalogExplorer : SasTask
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
            this.GeneratesReportOutput = false;
            this.GeneratesSasCode = false;
            this.ProductsRequired = "BASE";
            this.RequiresData = false;
            resources.ApplyResources(this, "$this");
        }

        public override ShowResult Show(System.Windows.Forms.IWin32Window Owner)
        {
            CatalogExplorerControl c = new CatalogExplorerControl(Consumer);
    
            c.Show();
            return ShowResult.Canceled;
        }
    }
}

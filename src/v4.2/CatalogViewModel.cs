using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAS.Tasks.Toolkit.Data;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    public class CatalogViewModel : TreeViewItemViewModel
    {
        readonly SasCatalog _catalog;

        public CatalogViewModel(SasCatalog catalog, LibraryViewModel parentLibrary)
            : base(parentLibrary, false)
        {
            _catalog = catalog;
        }

        public string CatalogName
        {
            get { return _catalog.Name; }
        }

        internal SasCatalog Catalog
        {
            get { return _catalog; }
        }

    }
}

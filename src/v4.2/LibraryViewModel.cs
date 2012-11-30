using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAS.Tasks.Toolkit.Data;
using System.Collections.ObjectModel;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    public class LibraryViewModel : TreeViewItemViewModel
    {
        readonly SasLibrary _library;

        public LibraryViewModel(SasLibrary library)
            : base(null, true)
        {
            _library = library;
        }

        public SasLibrary SasLibrary
        {
            get { return _library; }
        }

        public string LibraryName
        {
            get { return _library.Name; }
        }

        protected override void LoadChildren()
        {
            foreach (SasCatalog cat in _library.GetSasCatalogMembers())
            {
                base.Children.Add(new CatalogViewModel(cat, this));
            }
        }
    }
}

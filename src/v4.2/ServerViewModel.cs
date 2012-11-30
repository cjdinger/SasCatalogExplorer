using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAS.Tasks.Toolkit.Data;
using System.Collections.ObjectModel;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    /// <summary>
    /// The ViewModel for the Server.  This simply
    /// exposes a read-only collection of Library objects.
    /// </summary>
    public class ServerViewModel 
    {
        readonly ReadOnlyCollection<LibraryViewModel> _libraries;

        public ServerViewModel(SasLibrary[] libraries)
        {
            _libraries = new ReadOnlyCollection<LibraryViewModel>(
                (from library in libraries
                 select new LibraryViewModel(library))
                .ToList());
        }

        public ReadOnlyCollection<LibraryViewModel> Libraries
        {
            get { return _libraries; }
        }
    }
}

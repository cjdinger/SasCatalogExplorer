using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAS.Tasks.Toolkit.Data;
using System.Collections.ObjectModel;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    public class EntryViewModel
    {
        readonly ReadOnlyCollection<SasCatalogEntry> _entries;

        public EntryViewModel(SasCatalogEntry[] entries)
        {
            _entries = new ReadOnlyCollection<SasCatalogEntry>(entries.ToList());
        }

        public ReadOnlyCollection<SasCatalogEntry> Entries
        {
            get { return _entries; }
        }
    }
}

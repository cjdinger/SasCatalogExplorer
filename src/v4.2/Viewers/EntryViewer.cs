using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using SAS.Shared.AddIns;

namespace SAS.Tasks.Examples.CatalogExplorer.Viewers
{
    public class EntryViewer : UserControl
    {
        internal ISASTaskConsumer3 Consumer { get; set; }
        virtual public void ReadEntry(ISASTaskConsumer3 consumer, string serverName, string catEntry)
        { }
        
    }
}

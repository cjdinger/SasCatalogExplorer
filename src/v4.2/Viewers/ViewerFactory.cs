using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAS.Tasks.Toolkit.Data;

namespace SAS.Tasks.Examples.CatalogExplorer.Viewers
{
    public class ViewerFactory
    {
        public static EntryViewer GetEntryViewer(SasCatalogEntry entry)
        {
            switch (entry.ObjectType.ToLower().Trim())
            {
                case "source":
                case "csv":
                case "log":
                case "output":
                case "scl":
                case "program":
                    return new SourceEntryViewer();

                case "gif":
                case "jpg":
                case "jpeg":
                case "grseg":
                    return new ImageEntryViewer();

                default:
                    return new EntryViewer();
            }            
        }
    }
}

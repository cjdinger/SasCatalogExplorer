using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SAS.Tasks.Toolkit;
using SAS.Shared.AddIns;

namespace SAS.Tasks.Examples.CatalogExplorer.Viewers
{
    /// <summary>
    /// Interaction logic for SourceEntryViewer.xaml
    /// </summary>
    public partial class SourceEntryViewer : EntryViewer
    {
        public SourceEntryViewer()
        {
            InitializeComponent();
        }

        // download the catalog entry contents and view in an editor

        override public void ReadEntry(ISASTaskConsumer3 consumer, string serverName, string catEntry)
        {
            Consumer = consumer;

            SAS.FileService fs = null;
            SAS.IFileref fr = null;
            SAS.Workspace ws = null;
            SAS.TextStream ts = null;
            string fileref;

            try
            {
                // use SAS workspace and fileservice to download the file

                ws = consumer.Workspace(serverName) as SAS.Workspace;
                fs = ws.FileService;

                // using the FILENAME CATALOG access method
                fr = fs.AssignFileref("", "CATALOG", catEntry, "", out fileref);
                ts = fr.OpenTextStream(SAS.StreamOpenMode.StreamOpenModeForReading, 16500);
                ts.Separator = Environment.NewLine;

                StringBuilder sb = new StringBuilder();

                // downloading catalog entry contents 
                int lines = 1;
                Array truncLines, readLines;
                // iterate through until all lines are read in
                while (lines > 0)
                {
                    ts.ReadLines(100, out truncLines, out readLines);
                    lines = readLines.GetLength(0);
                    for (int i = 0; i < lines; i++)
                        sb.AppendLine(readLines.GetValue(i).ToString());
                }

                txtEntry.Text = sb.ToString();

                // cleanup
                ts.Close();
                fs.DeassignFileref(fr.FilerefName);
            }

            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot open the catalog entry {0}.  Reason: {1}", catEntry, ex.Message));
            }

        }

        private void btnAddToProject_Clicked(object sender, RoutedEventArgs e)
        {
            if (Consumer != null && !string.IsNullOrEmpty(txtEntry.Text))
            {
                // need a way to add new code node to the flow!
                
            }
        }
    }
}

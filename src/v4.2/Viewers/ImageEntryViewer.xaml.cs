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
using System.IO;

namespace SAS.Tasks.Examples.CatalogExplorer.Viewers
{
    /// <summary>
    /// Interaction logic for ImageEntryViewer.xaml
    /// </summary>
    public partial class ImageEntryViewer : EntryViewer
    {
        public ImageEntryViewer()
        {
            InitializeComponent();
        }

        // download the catalog entry contents and view in an editor

        override public void ReadEntry(ISASTaskConsumer3 consumer, string serverName, string catEntry)
        {

            SAS.FileService fs = null;
            SAS.IFileref fr = null;
            SAS.Workspace ws = null;
            SAS.BinaryStream bs = null;
            string fileref;

            try
            {
                // use SAS workspace and fileservice to download the file

                ws = consumer.Workspace(serverName) as SAS.Workspace;
                fs = ws.FileService;

                // using the FILENAME CATALOG access method
                fr = fs.AssignFileref("", "CATALOG", catEntry, "", out fileref);
                bs = fr.OpenBinaryStream(StreamOpenMode.StreamOpenModeForReading);

                using (MemoryStream ms = new MemoryStream())
                {
                    // downloading catalog entry contents 
                    int bytes = 1;
                    Array buffer;
                    // iterate through until all lines are read in
                    while (bytes > 0)
                    {
                        bs.Read(32767, out buffer);
                        bytes = buffer.GetLength(0);
                        for (int i = 0; i < bytes; i++)
                            ms.WriteByte((byte)buffer.GetValue(0));
                    }

                    ms.Close();
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.StreamSource = ms;
                    b.EndInit();

                    imgContent.Source = b;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot open the catalog entry {0}.  Reason: {1}", catEntry, ex.Message));
            }

        }
    }
}

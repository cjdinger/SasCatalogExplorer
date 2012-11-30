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

using SAS.Shared.AddIns;
using SAS.Tasks.Toolkit;
using SAS.Tasks.Toolkit.Data;
using System.Collections.ObjectModel;

namespace SAS.Tasks.Examples.CatalogExplorer
{
    /// <summary>
    /// Interaction logic for CatalogExplorerControl.xaml
    /// </summary>
    public partial class CatalogExplorerControl : Window
    {

        private ISASTaskConsumer3 consumer3 = null;

        static log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(CatalogExplorerControl));

        public CatalogExplorerControl(ISASTaskConsumer3 consumer)
        {
            InitializeComponent();
            
            Title = "SAS Catalog Explorer";
            consumer3 = consumer;

            // get the list of SAS servers for use in the context menu
            foreach (SasServer s in SasServer.GetSasServers())
            {
                MenuItem mi = new MenuItem();
                mi.Header = s.Name;
                mi.Tag = s;
                mi.Click += new RoutedEventHandler(serverSelected);
                serverContext.Items.Add(mi);
            }

            serverButton.Click += new RoutedEventHandler(serverButton_Click);
            viewButton.Click += new RoutedEventHandler(viewButton_Click);
            deleteButton.Click += new RoutedEventHandler(deleteButton_Click);

            // get notified when the tree view selection changes
            treeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeView_SelectedItemChanged);
                          
            SasServer server = new SasServer(consumer3.AssignedServer);
            InitializeWithServer(server);

            deleteButton.IsEnabled = false;
            viewButton.IsChecked = true;
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (listEntries.SelectedItems.Count > 0)
            {
                if (MessageBoxResult.Yes ==
                    MessageBox.Show("Are you sure that you want to delete the selected items?", "Confirm delete", MessageBoxButton.YesNo))
                {
                    // delete the selected catalog
                    DeleteCatalogEntries(listEntries.SelectedItems);
                }
            }
        }



        private void DeleteCatalogEntries(System.Collections.IList iList)
        {
            if (iList.Count == 0) return;

            _logger.Info("Deleting selected catalog entries");

            SasCatalogEntry e = ((SasCatalogEntry)iList[0]);
            
            // remember which catalog this is so that we can update the list when complete
            SasCatalog cat = new SasCatalog( consumer3.AssignedServer, e.Libref, e.Member );

            // build PROC CATALOG statements for each catalog entry to delete.
            string code = string.Format("proc catalog catalog={0}.{1}; \n",e.Libref,e.Member);
            foreach (SasCatalogEntry entry in iList)
            {
                code += string.Format("  delete {0}.{1}; \n", entry.Entry, entry.ObjectType);
            }

            code += "run;\n";

            _logger.InfoFormat("SAS job to delete entries: \n{0}", code);

            // submit the PROC CATALOG job and wait for it to complete
            consumer3.Submit.SubmitCode(code, consumer3.AssignedServer, false);

            // update the list view with remaining entries
            UpdateEntriesList(cat);
        }

        GridLength contentHeight = GridLength.Auto;
        void viewButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (dockContentView.Visibility == Visibility.Visible)
            {
                contentHeight = contentView.Height;
                dockContentView.Visibility = Visibility.Collapsed;
                contentView.Height = new GridLength(0);
            }
            else
            {
                contentView.Height = contentHeight;
                dockContentView.Visibility = Visibility.Visible;
                UpdateContentView();
            }
        }

        void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && e.NewValue is CatalogViewModel)
            {
                Cursor c = Cursor;
                try
                {
                    Cursor = Cursors.Wait;
                    UpdateEntriesList(((CatalogViewModel)e.NewValue).Catalog);
                }
                finally
                {
                    Cursor = c;
                }
            }
            else if (e.NewValue != null && e.NewValue is LibraryViewModel)
            {
                statusText.Text = string.Format("{1} catalogs in library {0}", 
                    ((LibraryViewModel)e.NewValue).LibraryName, 
                    ((LibraryViewModel)e.NewValue).SasLibrary.GetSasCatalogMembers().Count);
                UpdateContentView();
            }
        }

        private void UpdateEntriesList(SasCatalog cat)
        {
            IList<SasCatalogEntry> le = cat.GetSasCatalogEntries();
            listEntries.DataContext = new EntryViewModel(le.ToArray());
            statusText.Text = string.Format("{0} catalog entries in {1}.{2}", le.Count, cat.Libref, cat.Member);
        }

        void serverButton_Click(object sender, RoutedEventArgs e)
        {
            if (serverButton.ContextMenu != null)
            {
                serverButton.ContextMenu.PlacementTarget = serverButton;
                serverButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                ContextMenuService.SetPlacement(serverButton, System.Windows.Controls.Primitives.PlacementMode.Bottom);
                serverButton.ContextMenu.IsOpen = true;
            }
        }

        void serverSelected(object sender, RoutedEventArgs e)
        {
            InitializeWithServer(((MenuItem)sender).Tag as SasServer);
        }

        private void InitializeWithServer(SasServer server)
        {
            Cursor c = Cursor;
            try
            {
                Cursor = Cursors.Wait;

                serverLabel.Text = server.Name;

                SasLibrary[] libraries = server.GetSasLibraries().ToArray();
                ServerViewModel model = new ServerViewModel(libraries);
                treeView.DataContext = model;
            }
            finally
            {
                Cursor = c;
            }
        }

        private void listEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            deleteButton.IsEnabled = listEntries.SelectedItems.Count > 0;
            UpdateContentView();
        }

        UIElement currentContentViewer = null;
        private void UpdateContentView()
        {
            if (listEntries.SelectedItems.Count == 1 && dockContentView.Visibility == Visibility.Visible)
            {
                if (currentContentViewer != null)
                    dockContentView.Children.Remove(currentContentViewer);

                // show the correct content viewer for the entry type
                Viewers.EntryViewer viewer = Viewers.ViewerFactory.GetEntryViewer((SasCatalogEntry)listEntries.SelectedItem);
                currentContentViewer = viewer;

                makeSelectionMessage.Visibility = Visibility.Collapsed;
                dockContentView.Children.Add(viewer);
                viewer.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                viewer.VerticalContentAlignment = VerticalAlignment.Stretch;
                viewer.Width = dockContentView.Width;
                viewer.Height = dockContentView.Height;

                viewer.ReadEntry(consumer3, consumer3.AssignedServer, ((SasCatalogEntry)listEntries.SelectedItem).SasReference);

            }
        }
    }
}

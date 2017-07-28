// ---------------------------------------------------------------
// Originally written 2004, SAS Institute Inc.
// Copyright 2017, SAS Institute Inc.
// ---------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using System.Text;

using SAS.Shared.AddIns;
using SAS.Tasks.Toolkit;
using SAS.Tasks.Toolkit.Data;


namespace SAS.Tasks.CatalogFormats
{
	/// <summary>
	/// Present a UI for viewing SAS catalog entries
	/// </summary>
	public class CatalogExplorerForm : System.Windows.Forms.Form
	{
        const string TaskClassID = "6E2E0060-D11A-499C-BC68-20ECD7AA06EA";
        private ToolBarButton btnRefresh;

        // extract icon as bitmap for use in an imagelist
        public static Bitmap GetImageFromResources(string iconname, Size sz)
        {
            using (Icon icon = new Icon(typeof(CatalogExplorerForm), iconname))
            {
                using (Icon i = new Icon(icon, sz.Width, sz.Height))
                {
                    Bitmap bmp = Bitmap.FromHicon(i.Handle);
                    return bmp;
                }
            }
        }

		// the ISASTaskConsumer from the host application
		public ISASTaskConsumer Consumer
		{
			set
			{
				consumer = value;
			}
		}

        private string FmtSearch = "";

		public CatalogExplorerForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}


		// enumeration of images that we use in this form
		private enum CatImages 
		{
			Catalog = 0,
			Library = 1,
			Open = 2,
			Server = 3,

			Entry = 4,
			Frame = 5,
			Scl = 6,
			Class = 7,
			Menu = 8,
			Code = 9,
			Slist = 10,
			Wsave = 11,
			Delete = 12,
            Format = 13,
            Folder = 14,
            Refresh=15
		}

		protected override void OnLoad(EventArgs e)
		{
            RestorePosition();
            InitImageLists();
            this.Icon = new Icon(typeof(CatalogExplorerForm), "icons.catalog.ico");
			
            base.OnLoad (e);
            		
			if (consumer!=null)
			{
				// add servers to our dropdown toolbar button
				string[] servers;
				currentServer = consumer.AssignedServer;
				int count = consumer.Servers(out servers);

				// in case application does not support surfacing a list of servers
				if (count==0)
				{
					servers = new string[1] { consumer.AssignedServer };
					count=1;
				}
				for (int i=0; i<count; i++)
				{
					btnServers.DropDownMenu.MenuItems.Add(servers[i]);
					btnServers.DropDownMenu.MenuItems[i].Click +=new EventHandler(ServerMenu_Select);
				}	
				
				// add toolbar button handler
				tbTools.ButtonClick += new ToolBarButtonClickEventHandler(tbTools_ButtonClick);

				// add list view selection handler
				lvMembers.SelectedIndexChanged +=new EventHandler(lvMembers_SelectedIndexChanged);
                lvMembers.ColumnClick += new ColumnClickEventHandler(lvMembers_ColumnClick);
				
				// get the initial catalog list based on default servers
				NavigateToServer();

			}
		}

        private void RestorePosition()
        {
            // check if there are any settings stored for this task
            if (!string.IsNullOrEmpty
                (SAS.Tasks.Toolkit.Helpers.TaskUserSettings.ReadValue
                  (TaskClassID, "XCOORD")
                )
               )
            {
                // restore settings, if any, from previous invocations
                try
                {
                    int x = Convert.ToInt32
                        (SAS.Tasks.Toolkit.Helpers.TaskUserSettings.ReadValue
                          (TaskClassID, "XCOORD")
                        );
                    int y = Convert.ToInt32
                        (SAS.Tasks.Toolkit.Helpers.TaskUserSettings.ReadValue
                          (TaskClassID, "YCOORD")
                        );
                    int split = Convert.ToInt32
                        (SAS.Tasks.Toolkit.Helpers.TaskUserSettings.ReadValue
                          (TaskClassID, "SPLIT")
                        );

                    Point p = new Point(x, y);
                    if (isPointOnScreen(p))
                    {
                        int w = Convert.ToInt32
                            (SAS.Tasks.Toolkit.Helpers.TaskUserSettings.ReadValue
                              (TaskClassID, "WIDTH")
                            );
                        int h = Convert.ToInt32
                            (SAS.Tasks.Toolkit.Helpers.TaskUserSettings.ReadValue
                              (TaskClassID, "HEIGHT")
                            );
                        this.Width = w;
                        this.Height = h;
                        this.Location = p;
                        this.ctlSplitter.SplitPosition = split;
                    }


                }
                catch
                { }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            CatalogExplorer.isOneShowing = false;
            base.OnClosed(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SAS.Tasks.Toolkit.Helpers.TaskUserSettings.WriteValue
                (TaskClassID, "XCOORD", Convert.ToString(this.Location.X));
            SAS.Tasks.Toolkit.Helpers.TaskUserSettings.WriteValue
                (TaskClassID, "YCOORD", Convert.ToString(this.Location.Y));
            SAS.Tasks.Toolkit.Helpers.TaskUserSettings.WriteValue
                (TaskClassID, "WIDTH", Convert.ToString(this.Size.Width));
            SAS.Tasks.Toolkit.Helpers.TaskUserSettings.WriteValue
                (TaskClassID, "HEIGHT", Convert.ToString(this.Size.Height));
            SAS.Tasks.Toolkit.Helpers.TaskUserSettings.WriteValue
              (TaskClassID, "SPLIT", Convert.ToString(this.ctlSplitter.SplitPosition));
 
            base.OnClosing(e);
        }

        private void InitImageLists()
        {
            // add the images in the same sequence as defined in the CatImages enumeration
            imgList.Images.Add(GetImageFromResources("icons.catalog.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.library.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.open.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.server.ico", new Size(16, 16)));

            imgList.Images.Add(GetImageFromResources("icons.entry.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.frame.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.scl.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.class.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.menu.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.sascode.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.slist.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.wsave.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.delete.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.format.ico", new Size(16, 16)));
            imgList.Images.Add(GetImageFromResources("icons.folderview.ico", new Size(16, 16)));

            imgList.Images.Add(GetImageFromResources("icons.refresh.ico", new Size(16, 16)));

            // set up the treeview imagelists
            tvLibsCats.ImageList = imgList;
            lvMembers.SmallImageList = imgList;
            lvMembers.LargeImageList = imgList;

            // set up the toolbar imagelist
            tbTools.ImageList = imgList;
            btnServers.ImageIndex = (int)CatImages.Server;
            btnView.ImageIndex = (int)CatImages.Open;
            btnDelete.ImageIndex = (int)CatImages.Delete;
            btnRefresh.ImageIndex = (int)CatImages.Refresh;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

        /// <summary>
        /// Helper to determine of a coordinate point
        /// is visible on the screen
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool isPointOnScreen(Point p)
        {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens)
            {
                if (screen.WorkingArea.Contains(p))
                {
                    return true;
                }
            }
            return false;
        }

		#region Windows Form Designer generated code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.ToolBar tbTools;
        private System.Windows.Forms.ToolBarButton btnServers;
        private System.Windows.Forms.ToolBarButton btnView;
        private System.Windows.Forms.ContextMenu mnuServers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView tvLibsCats;
        private System.Windows.Forms.ListView lvMembers;

        private string currentServer = "";
        private ISASTaskConsumer consumer = null;
        private System.Windows.Forms.Splitter ctlSplitter;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.ColumnHeader objName;
        private System.Windows.Forms.ColumnHeader objType;
        private System.Windows.Forms.ColumnHeader objDesc;
        private System.Windows.Forms.ColumnHeader objCreated;
        private System.Windows.Forms.ColumnHeader objModified;
        private System.Windows.Forms.ToolBarButton btnDelete;
        private ImageList imgList = new ImageList();

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CatalogExplorerForm));
            this.tbTools = new System.Windows.Forms.ToolBar();
            this.btnServers = new System.Windows.Forms.ToolBarButton();
            this.mnuServers = new System.Windows.Forms.ContextMenu();
            this.btnView = new System.Windows.Forms.ToolBarButton();
            this.btnDelete = new System.Windows.Forms.ToolBarButton();
            this.btnRefresh = new System.Windows.Forms.ToolBarButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvMembers = new System.Windows.Forms.ListView();
            this.objName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.objType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.objDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.objCreated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.objModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctlSplitter = new System.Windows.Forms.Splitter();
            this.tvLibsCats = new System.Windows.Forms.TreeView();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbTools
            // 
            resources.ApplyResources(this.tbTools, "tbTools");
            this.tbTools.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.btnServers,
            this.btnView,
            this.btnDelete,
            this.btnRefresh});
            this.tbTools.Name = "tbTools";
            this.tbTools.TabStop = true;
            // 
            // btnServers
            // 
            this.btnServers.DropDownMenu = this.mnuServers;
            this.btnServers.Name = "btnServers";
            this.btnServers.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            resources.ApplyResources(this.btnServers, "btnServers");
            // 
            // btnView
            // 
            this.btnView.Name = "btnView";
            resources.ApplyResources(this.btnView, "btnView");
            // 
            // btnDelete
            // 
            this.btnDelete.Name = "btnDelete";
            resources.ApplyResources(this.btnDelete, "btnDelete");
            // 
            // btnRefresh
            // 
            this.btnRefresh.Name = "btnRefresh";
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvMembers);
            this.panel1.Controls.Add(this.ctlSplitter);
            this.panel1.Controls.Add(this.tvLibsCats);
            this.panel1.Controls.Add(this.statusBar);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lvMembers
            // 
            this.lvMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.objName,
            this.objType,
            this.objDesc,
            this.objCreated,
            this.objModified});
            resources.ApplyResources(this.lvMembers, "lvMembers");
            this.lvMembers.FullRowSelect = true;
            this.lvMembers.MultiSelect = false;
            this.lvMembers.Name = "lvMembers";
            this.lvMembers.UseCompatibleStateImageBehavior = false;
            this.lvMembers.View = System.Windows.Forms.View.Details;
            this.lvMembers.ItemActivate += new System.EventHandler(this.OnItemDoubleClicked);
            this.lvMembers.DoubleClick += new System.EventHandler(this.OnItemDoubleClicked);
            // 
            // objName
            // 
            resources.ApplyResources(this.objName, "objName");
            // 
            // objType
            // 
            resources.ApplyResources(this.objType, "objType");
            // 
            // objDesc
            // 
            resources.ApplyResources(this.objDesc, "objDesc");
            // 
            // objCreated
            // 
            resources.ApplyResources(this.objCreated, "objCreated");
            // 
            // objModified
            // 
            resources.ApplyResources(this.objModified, "objModified");
            // 
            // ctlSplitter
            // 
            resources.ApplyResources(this.ctlSplitter, "ctlSplitter");
            this.ctlSplitter.Name = "ctlSplitter";
            this.ctlSplitter.TabStop = false;
            // 
            // tvLibsCats
            // 
            resources.ApplyResources(this.tvLibsCats, "tvLibsCats");
            this.tvLibsCats.Name = "tvLibsCats";
            this.tvLibsCats.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeNodeSelected);
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.Name = "statusBar";
            // 
            // CatalogExplorerForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tbTools);
            this.Name = "CatalogExplorerForm";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region Catalog content

		// Clear the form of all info, and populate the tree with
		// libraries and catalogs for the current server
		private void NavigateToServer()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			tvLibsCats.Nodes.Clear();
			lvMembers.Items.Clear();

			TreeNode sn = tvLibsCats.Nodes.Add(currentServer);
			sn.ImageIndex = (int)CatImages.Server;
			sn.SelectedImageIndex = (int)CatImages.Server;
			sn.Tag = "SERVER";

			try
			{
                SasServer s = new SasServer(currentServer);
                foreach (SasLibrary lib in s.GetSasLibraries())
                {                   
                    if (lib.IsAssigned)
                    { 
                        TreeNode tn = new TreeNode(lib.Libref);
                        tn.ImageIndex = (int)CatImages.Library;
                        tn.SelectedImageIndex = (int)CatImages.Library;
                        tn.Tag = "LIBRARY";
                        sn.Nodes.Add(tn);

                        // add catalogs for this library
                        PopulateCatalogs(tn);
                    }
                }

                TreeNode tnf = new TreeNode(Translate.LabelUserDefinedFormats);
                tnf.ImageIndex = (int)CatImages.Folder;
                tnf.SelectedImageIndex = (int)CatImages.Folder;
                tnf.Tag = "FMTSEARCH";
                sn.Nodes.Add(tnf);

                FmtSearch = s.GetSasSystemOptionValue("FMTSEARCH");

                PopulateUserFormats(tnf);

                // open to the Library list
                sn.Expand();
			}
			catch
			{
			}

			Cursor.Current = c;
			UpdateToolbar();
		}

        private void PopulateUserFormats(TreeNode tn)
        {
            SasServer s = new SasServer(currentServer);
            using (System.Data.OleDb.OleDbConnection dbConnection = s.GetOleDbConnection())
            {

                //----- make provider connection
                dbConnection.Open();

                //----- Read values from query command
                string sql = @"select distinct libname, memname from sashelp.vformat where libname NOT is missing";
                OleDbCommand cmdDB = new OleDbCommand(sql, dbConnection);
                OleDbDataReader dataReader = cmdDB.ExecuteReader(CommandBehavior.CloseConnection);
                try
                {
                    while (dataReader.Read())
                    {
                        TreeNode n = new TreeNode(string.Format("{0}.{1}", dataReader["libname"].ToString(), dataReader["memname"].ToString()));
                        n.ImageIndex = (int)CatImages.Catalog;
                        n.SelectedImageIndex = (int)CatImages.Catalog;
                        n.Tag = "LIBCAT";
                        tn.Nodes.Add(n);
                    }
                }

                finally
                {
                    dataReader.Close();
                    dbConnection.Close();
                }

            }
 	        
        }

		// read list of catalogs for the given library (in the tree node)
		private void PopulateCatalogs(TreeNode tn)
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			tvLibsCats.BeginUpdate();

			if (tn!=null)
			{
                SasLibrary l = new SasLibrary(currentServer, tn.Text);
                if (l != null)
                {
                    foreach (SasCatalog cat in l.GetSasCatalogMembers())
                    {
                        TreeNode catNode = tn.Nodes.Add(cat.Name);
                        catNode.ImageIndex = (int)CatImages.Catalog;
                        catNode.SelectedImageIndex = (int)CatImages.Catalog;
                        catNode.Tag = "CATALOG";
                    }
                }			
			}

			tvLibsCats.EndUpdate();

			Cursor.Current = c;

			UpdateToolbar();
		}

		// read list of catalog entries for the given catalog
		private void PopulateMembers(string lib, string cat)
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			lvMembers.BeginUpdate();
			lvMembers.Items.Clear();

            SasCatalog catalog = new SasCatalog(currentServer, lib, cat);
            foreach (SasCatalogEntry entry in catalog.GetSasCatalogEntries())
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = entry.Name;
                lvi.SubItems.Add(entry.ObjectType);
                lvi.SubItems.Add(entry.Description);
                (lvi.SubItems.Add(entry.Created.ToString())).Tag=entry.Created;
                (lvi.SubItems.Add(entry.LastModified.ToString())).Tag=entry.LastModified;
                lvi.ImageIndex = GetImageIndexForEntry(entry.ObjectType.ToString());
                lvi.Tag = string.Format("{0}.{1}.{2}.{3}", lib, cat, lvi.Text, entry.ObjectType);
                lvMembers.Items.Add(lvi);
            }

		    lvMembers.EndUpdate();

			Cursor.Current = c;

			UpdateToolbar();

		}

		private int GetImageIndexForEntry(string type)
		{
			int index = (int)CatImages.Entry;
			switch (type.ToLower())
			{
				case "frame": index = (int)CatImages.Frame; break;
				case "scl": index = (int)CatImages.Scl; break;
				case "class": index = (int)CatImages.Class; break;
				case "pmenu": index = (int)CatImages.Menu; break;
				case "slist": index = (int)CatImages.Slist; break;
				case "wsave": index = (int)CatImages.Wsave; break;
				case "source": index = (int)CatImages.Code; break;
                case "format": index = (int)CatImages.Format; break;
                case "formatc": index = (int)CatImages.Format; break;
                case "infmt": index = (int)CatImages.Format; break;
                case "infmtc": index = (int)CatImages.Format; break;
				default: break;
			}

			return index;
			
		}

		#endregion

		#region Event handlers

		private bool CanOpen(string entryType)
		{
            return (entryType == "source" || 
                    entryType == "slist" || 
                    entryType == "scl" || 
                    entryType == "list" || 
                    entryType == "format" ||
                    entryType == "formatc" ||
                    entryType == "infmt" ||
                    entryType == "infmtc");
		}

		private void UpdateToolbar()
		{
			string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].SubItems[1].Text.ToLower() : "";
			btnView.Enabled = CanOpen(selectedEntry);
			if (selectedEntry.Length>0)
				btnDelete.Enabled = true;
		}

		private void ServerMenu_Select(object sender, EventArgs e)
		{

			MenuItem mi = sender as MenuItem;
            if (mi.Text != currentServer)
            { 
			    currentServer = mi.Text;
			    NavigateToServer();
            }
		}

		private void OnTreeNodeSelected(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			switch (e.Node.Tag.ToString())
			{
				case "SERVER":
				{
					lvMembers.Items.Clear();
					statusBar.Text = string.Format(Translate.MessageServer, e.Node.Nodes.Count);
					break;
				}
				case "LIBRARY":
				{
					lvMembers.Items.Clear();
					statusBar.Text = string.Format(Translate.MessageLibrary, e.Node.Nodes!=null ? e.Node.Nodes.Count : 0);
					break;
				}
				case "CATALOG":
				{
					if (e.Node.Parent!=null)
						PopulateMembers(e.Node.Parent.Text, e.Node.Text);
                    statusBar.Text = string.Format(Translate.MessageCatalog, lvMembers.Items.Count, e.Node.Parent.Text, e.Node.Text);
					break;
				}
                case "LIBCAT":
                {
                    string[] parts = e.Node.Text.Split('.');
                    if (parts.Length==2)
                        PopulateMembers(parts[0], parts[1]);
                    statusBar.Text = string.Format(Translate.MessageCatalog, lvMembers.Items.Count, parts[0], parts[1]);
                    break;
                }
                case "FMTSEARCH":
                {
                    lvMembers.Items.Clear();
                    statusBar.Text = string.Format(Translate.MessageFmtSearch, FmtSearch);
                    break;
                }
				default:
					break;
			}
		}

		private void lvMembers_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateToolbar();
		}

        // Sort handlers for the list view columns
        private int sortColumn = -1;

        void lvMembers_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                lvMembers.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (lvMembers.Sorting == SortOrder.Ascending)
                    lvMembers.Sorting = SortOrder.Descending;
                else
                    lvMembers.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            lvMembers.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.lvMembers.ListViewItemSorter = new MemberComparer(e.Column,
                                                              lvMembers.Sorting);
        }

		private void tbTools_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button==btnView)
			{
				string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].SubItems[1].Text.ToLower() : "";
				if (CanOpen(selectedEntry))
				{
                    if (lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".FORMAT")
                        || lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".FORMATC")
                        || lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".INFMT")
                        || lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".INFMTC"))
                        ViewFormatDetails(lvMembers.SelectedItems[0].Tag.ToString());
                    else
					    ViewInEditor(lvMembers.SelectedItems[0].Tag.ToString());
				}
			}
			else if (e.Button == btnDelete)
			{
				string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].Tag.ToString() : "";
				DeleteEntry(selectedEntry);
			}
            else if (e.Button == btnRefresh)
            {
                NavigateToServer();
            }
		}

        // code to output Format "report"
        const string fmtProgram = 
            "filename fmtout temp; "+ 
            "%let _catexp1 = %sysfunc(getoption(center)); " +
            "%let _catexp2 = %sysfunc(getoption(date)); " +
            "%let _catexp3 = %sysfunc(getoption(number)); " +
            "%let _catexp4 = %sysfunc(getoption(pagesize)); " +
            "options nocenter nodate nonumber pagesize=max; " +
            "ods listing file=fmtout;" +
            "proc format fmtlib lib={0}.{1};" +
            " select {3}{2};" +
            "run;" +
            "ods listing close;" +
            "options &_catexp1. &_catexp2. &_catexp3. PS=&_catexp4.;" + 
            ";*';*\";*/;quit;run;";

        private void ViewFormatDetails(string p)
        {
            SasSubmitter s = new SasSubmitter(currentServer);
            if (s.IsServerBusy())
            {
                MessageBox.Show(string.Format(Translate.MessageServerBusy, currentServer), Translate.TitleError);
                return;
            }

            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            string[] parts = p.Split('.');
            if (parts.Length == 4)
            {
                // if this is an INFORMAT, the SELECT needs an @ symbol
                // if Char, then the $ symbol
                string prefix = "";
                switch (parts[3])
                {
                    case "INFMT": prefix = "@";
                        break;
                    case "FORMATC": prefix = "$";
                        break;
                    case "INFMTC": prefix = "@$";
                        break;
                    default: prefix = "";
                        break;
                };

                string code = string.Format(fmtProgram, parts[0], parts[1], parts[2], prefix);

                string log = "";
                bool success = s.SubmitSasProgramAndWait(code, out log);
                
                Cursor.Current = c;

                if (success)
                {
                    string formatReport = GetTextFromFileref("fmtout");

                    ListingViewForm dlg = new ListingViewForm();
                    dlg.Text = string.Format(Translate.LabelFormatDefinition, parts[2], parts[0], parts[1]);
                    dlg.Content = formatReport;
                    dlg.ShowDialog(this);
                    s = null;
                }
                else MessageBox.Show(Translate.MessageErrorFormat,Translate.TitleError);
            }
        }
		
		private void OnItemDoubleClicked(object sender, System.EventArgs e)
		{
			string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].SubItems[1].Text.ToLower() : "";
			if (CanOpen(selectedEntry))
			{
                if (lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".FORMAT")
                    || lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".FORMATC")
                    || lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".INFMT")
                    || lvMembers.SelectedItems[0].Tag.ToString().EndsWith(".INFMTC"))
                    ViewFormatDetails(lvMembers.SelectedItems[0].Tag.ToString());
                else
                    ViewInEditor(lvMembers.SelectedItems[0].Tag.ToString());

			}	
		}

		private void DeleteEntry(string catEntry)
		{
            SasSubmitter s = new SasSubmitter(currentServer);
            if (s.IsServerBusy())
            {
                MessageBox.Show(string.Format(Translate.MessageServerBusy, currentServer), Translate.TitleError);
                return;
            }

			if (catEntry.Length>0)
			{
				if (DialogResult.Yes==MessageBox.Show(string.Format(Translate.MessageDeleteConfirm,catEntry), 
					Translate.TitleDeleteConfirm, 
					MessageBoxButtons.YesNo))
				{
					Cursor c = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;

					string[] parts=catEntry.Split('.');

					if (parts.Length==4)
					{
						string code = string.Format("PROC CATALOG CATALOG={0}.{1}; DELETE {2}.{3}; RUN; ;*';*\";*/;quit;run;", parts[0], parts[1], parts[2], parts[3]);                
                        string log="";

                        if (s.SubmitSasProgramAndWait(code, out log))
                        {
                            // refresh view
                            PopulateMembers(parts[0], parts[1]);
                            statusBar.Text = string.Format(Translate.MessageCatalogDeleted, catEntry);
                        }
                        else 
                            MessageBox.Show(string.Format(Translate.MessageCatalogNotDeleted, catEntry), Translate.TitleError);
					}
					Cursor.Current = c;
				}
			}
		}

        private string GetTextFromFileref(string fileref)
        {
            SAS.FileService fs = null;
            SAS.IFileref fr = null;
            SAS.Workspace ws = null;
            SAS.TextStream ts = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                // use SAS workspace and fileservice to download the file
                ws = consumer.Workspace(currentServer) as SAS.Workspace;
                fs = ws.FileService;
                fr = fs.UseFileref(fileref);
                ts = fr.OpenTextStream(SAS.StreamOpenMode.StreamOpenModeForReading, 16500);
                ts.Separator = Environment.NewLine;

                // downloading catalog entry contents to local temp file
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

                ts.Close();
                fs.DeassignFileref(fileref);

            }
            catch (Exception)
            { 

            }
            finally
            {
                ts = null;
                fr = null;
                fs = null;
                ws = null;
            }

            return sb.ToString();
        }

		// download the catalog entry contents and view in an editor
		private void ViewInEditor(string catEntry)
		{

            SAS.FileService fs = null;
            SAS.IFileref fr = null;
            SAS.Workspace ws = null;
            SAS.TextStream ts = null;
            string fileref;
            StringBuilder sb = new StringBuilder();
            try
            {
                // use SAS workspace and fileservice to download the file
                ws = consumer.Workspace(currentServer) as SAS.Workspace;
                fs = ws.FileService;
                // using the FILENAME CATALOG access method
                fr = fs.AssignFileref("", "CATALOG", catEntry, "", out fileref);
                ts = fr.OpenTextStream(SAS.StreamOpenMode.StreamOpenModeForReading, 16500);
                ts.Separator = Environment.NewLine;

                // downloading catalog entry contents to local temp file
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
                ts.Close();
                fs.DeassignFileref(fileref);

                // show the text in a modal dialog
                SAS.Tasks.Toolkit.Controls.SASCodeViewDialog dlg = new SAS.Tasks.Toolkit.Controls.SASCodeViewDialog(
                    Control.FromHandle(this.Handle),
                    catEntry, sb.ToString());
                dlg.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Translate.MessageCatalogReadError, catEntry, ex.Message), Translate.TitleError);
            }
            finally
            {
                ts = null;
                fr = null;
                fs = null;
                ws = null;
            }
		}
	
		#endregion



	}
}

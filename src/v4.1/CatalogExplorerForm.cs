// ---------------------------------------------------------------
// Copyright 2004, SAS Institute Inc.
// ---------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using SAS.Shared.AddIns;
// for access to the Add-in interfaces

namespace CatalogExplorer
{
	/// <summary>
	/// SPresent a UI for viewing SAS catalog entries
	/// </summary>
	public class CatalogExplorerForm : System.Windows.Forms.Form
	{
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

		// the ISASTaskConsumer from the host application
		public ISASTaskConsumer Consumer
		{
			set
			{
				consumer = value;
			}
		}

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
			Delete = 12
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// add the images in the same sequence as defined in the CatImages enumeration
			imgList.Images.Add(Global.GetImageFromResources("icons.catalog.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.library.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.open.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.server.ico", new Size(16,16)));

			imgList.Images.Add(Global.GetImageFromResources("icons.entry.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.frame.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.scl.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.class.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.menu.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.sascode.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.slist.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.wsave.ico", new Size(16,16)));
			imgList.Images.Add(Global.GetImageFromResources("icons.delete.ico", new Size(16,16)));

			// set up the treeview imagelists
			tvLibsCats.ImageList = imgList;
			lvMembers.SmallImageList = imgList;
			lvMembers.LargeImageList = imgList;

			// set up the toolbar imagelist
			tbTools.ImageList = imgList;
			btnServers.ImageIndex = (int)CatImages.Server;
			btnView.ImageIndex = (int)CatImages.Open;
			btnDelete.ImageIndex = (int)CatImages.Delete;

			this.Icon = new Icon(typeof(Global), "icons.catalog.ico");
		
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
				
				// get the initial catalog list based on default servers
				NavigateToServer();


			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbTools = new System.Windows.Forms.ToolBar();
			this.btnServers = new System.Windows.Forms.ToolBarButton();
			this.mnuServers = new System.Windows.Forms.ContextMenu();
			this.btnView = new System.Windows.Forms.ToolBarButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lvMembers = new System.Windows.Forms.ListView();
			this.objName = new System.Windows.Forms.ColumnHeader();
			this.objType = new System.Windows.Forms.ColumnHeader();
			this.objDesc = new System.Windows.Forms.ColumnHeader();
			this.objCreated = new System.Windows.Forms.ColumnHeader();
			this.objModified = new System.Windows.Forms.ColumnHeader();
			this.ctlSplitter = new System.Windows.Forms.Splitter();
			this.tvLibsCats = new System.Windows.Forms.TreeView();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.btnDelete = new System.Windows.Forms.ToolBarButton();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbTools
			// 
			this.tbTools.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.tbTools.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.btnServers,
																					   this.btnView,
																					   this.btnDelete});
			this.tbTools.DropDownArrows = true;
			this.tbTools.Location = new System.Drawing.Point(0, 0);
			this.tbTools.Name = "tbTools";
			this.tbTools.ShowToolTips = true;
			this.tbTools.Size = new System.Drawing.Size(558, 42);
			this.tbTools.TabIndex = 0;
			// 
			// btnServers
			// 
			this.btnServers.DropDownMenu = this.mnuServers;
			this.btnServers.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			this.btnServers.Text = "Servers";
			this.btnServers.ToolTipText = "Select active server";
			// 
			// btnView
			// 
			this.btnView.Text = "View";
			this.btnView.ToolTipText = "View contents of entry";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.lvMembers);
			this.panel1.Controls.Add(this.ctlSplitter);
			this.panel1.Controls.Add(this.tvLibsCats);
			this.panel1.Controls.Add(this.statusBar);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 42);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(558, 370);
			this.panel1.TabIndex = 1;
			// 
			// lvMembers
			// 
			this.lvMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.objName,
																						this.objType,
																						this.objDesc,
																						this.objCreated,
																						this.objModified});
			this.lvMembers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvMembers.FullRowSelect = true;
			this.lvMembers.Location = new System.Drawing.Point(170, 0);
			this.lvMembers.MultiSelect = false;
			this.lvMembers.Name = "lvMembers";
			this.lvMembers.Size = new System.Drawing.Size(388, 348);
			this.lvMembers.TabIndex = 2;
			this.lvMembers.View = System.Windows.Forms.View.Details;
			this.lvMembers.DoubleClick += new System.EventHandler(this.OnItemDoubleClicked);
			// 
			// objName
			// 
			this.objName.Text = "Name";
			this.objName.Width = 100;
			// 
			// objType
			// 
			this.objType.Text = "Type";
			this.objType.Width = 100;
			// 
			// objDesc
			// 
			this.objDesc.Text = "Description";
			this.objDesc.Width = 150;
			// 
			// objCreated
			// 
			this.objCreated.Text = "Created";
			this.objCreated.Width = 100;
			// 
			// objModified
			// 
			this.objModified.Text = "Modified";
			this.objModified.Width = 100;
			// 
			// ctlSplitter
			// 
			this.ctlSplitter.Location = new System.Drawing.Point(167, 0);
			this.ctlSplitter.Name = "ctlSplitter";
			this.ctlSplitter.Size = new System.Drawing.Size(3, 348);
			this.ctlSplitter.TabIndex = 1;
			this.ctlSplitter.TabStop = false;
			// 
			// tvLibsCats
			// 
			this.tvLibsCats.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvLibsCats.ImageIndex = -1;
			this.tvLibsCats.Location = new System.Drawing.Point(0, 0);
			this.tvLibsCats.Name = "tvLibsCats";
			this.tvLibsCats.SelectedImageIndex = -1;
			this.tvLibsCats.Size = new System.Drawing.Size(167, 348);
			this.tvLibsCats.TabIndex = 0;
			this.tvLibsCats.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeNodeSelected);
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 348);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(558, 22);
			this.statusBar.TabIndex = 3;
			// 
			// btnDelete
			// 
			this.btnDelete.Text = "Delete";
			this.btnDelete.ToolTipText = "Delete the selected entry";
			// 
			// CatalogExplorerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(558, 412);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tbTools);
			this.Name = "CatalogExplorerForm";
			this.Text = "SAS Catalog Explorer";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

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
				string[] libs;
				consumer.Libraries(currentServer,out libs);
				foreach(string lib in libs)
				{
					TreeNode tn = new TreeNode(lib);
					tn.ImageIndex = (int)CatImages.Library;
					tn.SelectedImageIndex = (int)CatImages.Library;
					tn.Tag = "LIBRARY";
					sn.Nodes.Add(tn);

					// add catalogs for this library
					PopulateCatalogs(tn);
				}
			}
			catch
			{
			}

			Cursor.Current = c;
			UpdateToolbar();
		}

		// read list of catalogs for the given library (in the tree node)
		private void PopulateCatalogs(TreeNode tn)
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			tvLibsCats.BeginUpdate();

			if (tn!=null)
			{
				SAS.Workspace ws = null;
				try
				{
					ws = consumer.Workspace(currentServer) as SAS.Workspace;		
				}
				catch (Exception ex)
				{
					throw new System.Exception("ISASTaskConsumer.Workspace is not usable!",ex);
				}

				if (currentServer.Length>0 && ws!=null)
				{

					// use the SAS IOM OLEDB provider to read data from the SAS workspace
					ADODB.Recordset adorecordset = new ADODB.RecordsetClass();
					ADODB.Connection adoconnect = new ADODB.ConnectionClass();

					try
					{
						adoconnect.Open("Provider=sas.iomprovider.1; SAS Workspace ID=" + ws.UniqueIdentifier, "", "", 0);
						// use the SASHELP.VMEMBER view to get names of all of the catalogs in the specified library
						string selectclause = "select memname from sashelp.vmember where libname='" + tn.Text + "' and memtype in ('CATALOG')";
						adorecordset.Open( selectclause, adoconnect, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, (int) ADODB.CommandTypeEnum.adCmdText);
					
						while (!adorecordset.EOF)
						{				
							TreeNode cat = tn.Nodes.Add(adorecordset.Fields["memname"].Value.ToString());
							cat.ImageIndex = (int)CatImages.Catalog;
							cat.SelectedImageIndex = (int)CatImages.Catalog;
							cat.Tag = "CATALOG";
							adorecordset.MoveNext();
						}

					}
					catch {}
					finally
					{
						adoconnect.Close();
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

			SAS.Workspace ws = null;
			try
			{
				ws = consumer.Workspace(currentServer) as SAS.Workspace;
			}
			catch (Exception ex)
			{
				throw new System.Exception("ISASTaskConsumer.Workspace is not usable!",ex);
			}

			if (currentServer.Length>0 && ws!=null)
			{

				// use the SAS IOM OLEDB provider to read data from the SAS workspace
				ADODB.Recordset adorecordset = new ADODB.RecordsetClass();
				ADODB.Connection adoconnect = new ADODB.ConnectionClass();

				try
				{
					adoconnect.Open("Provider=sas.iomprovider.1; SAS Workspace ID=" + ws.UniqueIdentifier, "", "", 0);
					// use the SASHELP.VCATALG view to get all of the catalog entries in the specified library/catalog
					string selectclause = "select * from sashelp.vcatalg where libname='" + lib + "' and memname = '" + cat + "'";
					adorecordset.Open( selectclause, adoconnect, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, (int) ADODB.CommandTypeEnum.adCmdText);
					
					while (!adorecordset.EOF)
					{				
						ListViewItem lvi = new ListViewItem();
						lvi.Text = adorecordset.Fields["objname"].Value.ToString();
						lvi.SubItems.Add(adorecordset.Fields["objtype"].Value.ToString());
						lvi.SubItems.Add(adorecordset.Fields["objdesc"].Value.ToString());
						lvi.SubItems.Add(ConvertSASDate(adorecordset.Fields["created"].Value.ToString()));
						lvi.SubItems.Add(ConvertSASDate(adorecordset.Fields["modified"].Value.ToString()));
						lvi.ImageIndex = GetImageIndexForEntry(adorecordset.Fields["objtype"].Value.ToString());
						lvi.Tag = string.Format("{0}.{1}.{2}.{3}", lib, cat, lvi.Text, adorecordset.Fields["objtype"].Value.ToString());
						lvMembers.Items.Add(lvi);
						adorecordset.MoveNext();
					}

				}
				catch {}
				finally
				{
					adoconnect.Close();
				}

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
				default: break;
			}

			return index;
			
		}

		/// <summary>
		/// Convert a SAS datetime (represented as a double within a string) to a Windows datetime
		/// </summary>
		/// <param name="sasdt">string version of the double value for the SAS date time</param>
		/// <returns>a string with the Windows-formatted date</returns>
		private string ConvertSASDate(string sasdt)
		{
			string rc = string.Empty;
			try
			{
				double dDate = Convert.ToDouble(sasdt);

				// SAS datetime is number of seconds since Jan 1, 1960
				DateTime dt = new DateTime(1960,1,1,0,0,0,0);

				System.Globalization.DateTimeFormatInfo dfi = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
				dt = dt.AddSeconds(dDate);                                                                 
				rc = dt.ToString();
			}
			catch {}

			return rc;

		}
		#endregion

		#region Event handlers

		private bool CanOpen(string entryType)
		{
			return (entryType=="source" || entryType=="slist" || entryType=="scl" || entryType=="list");
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
			currentServer = mi.Text;

			NavigateToServer();
		}

		private void OnTreeNodeSelected(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			switch (e.Node.Tag.ToString())
			{
				case "SERVER":
				{
					lvMembers.Items.Clear();
					statusBar.Text = string.Format("Server has {0} libraries defined", e.Node.Nodes.Count);
					break;
				}
				case "LIBRARY":
				{
					lvMembers.Items.Clear();
					statusBar.Text = string.Format("Library contains {0} catalogs", e.Node.Nodes!=null ? e.Node.Nodes.Count : 0);
					break;
				}
				case "CATALOG":
				{
					if (e.Node.Parent!=null)
						PopulateMembers(e.Node.Parent.Text, e.Node.Text);
					statusBar.Text = string.Format("Catalog contains {0} entries", lvMembers.Items.Count);
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

		private void tbTools_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button==btnView)
			{
				string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].SubItems[1].Text.ToLower() : "";
				if (CanOpen(selectedEntry))
				{
					ViewInEditor(lvMembers.SelectedItems[0].Tag.ToString());
				}
			}
			else if (e.Button == btnDelete)
			{
				string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].Tag.ToString() : "";
				DeleteEntry(selectedEntry);
			}
		}
		
		private void OnItemDoubleClicked(object sender, System.EventArgs e)
		{
			string selectedEntry = (lvMembers.SelectedItems!=null && lvMembers.SelectedItems.Count>0) ? lvMembers.SelectedItems[0].SubItems[1].Text.ToLower() : "";
			if (CanOpen(selectedEntry))
			{
				ViewInEditor(lvMembers.SelectedItems[0].Tag.ToString());
			}	
		}

		private void DeleteEntry(string catEntry)
		{
			if (catEntry.Length>0)
			{
				if (DialogResult.Yes==MessageBox.Show(string.Format("Are you sure that you want to delete {0}?",catEntry), 
					"Confirm: Delete entry?", 
					MessageBoxButtons.YesNo))
				{
					Cursor c = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;

					string[] parts=catEntry.Split('.');

					if (parts.Length==4)
					{
						string code = string.Format("PROC CATALOG CATALOG={0}.{1}; DELETE {2}.{3}; RUN;", parts[0], parts[1], parts[2], parts[3]);
						consumer.Submit.SubmitCode(code, currentServer, false);

						// refresh view
						PopulateMembers(parts[0], parts[1]);

						statusBar.Text = string.Format("{0} was deleted.",catEntry);
					}
					Cursor.Current = c;
				}
			}

		}

		// download the catalog entry contents and view in an editor
		private void ViewInEditor(string catEntry)
		{
			SAS.FileService fs = null;
			SAS.Fileref fr = null;
			SAS.Workspace ws = null;
			SAS.TextStream ts = null;
			string fileref;

			try
			{
				// use SAS workspace and fileservice to download the file
				ws = consumer.Workspace(currentServer) as SAS.Workspace;		
				fs = ws.FileService;
				// using the FILENAME CATALOG access method
				fr = fs.AssignFileref("", "CATALOG", catEntry, "", out fileref);
				ts = fr.OpenTextStream(SAS.StreamOpenMode.StreamOpenModeForReading, 16500);
				ts.Separator = Environment.NewLine;
				string tempFn = System.IO.Path.GetTempFileName();

				// downloading catalog entry contents to local temp file
				using(System.IO.StreamWriter sw = System.IO.File.CreateText(tempFn))
				{
					int lines = 1;
					Array truncLines, readLines;
					// iterate through until all lines are read in
					while (lines>0)
					{
						ts.ReadLines(100, out truncLines, out readLines);
						lines = readLines.GetLength(0);
						for (int i=0; i<lines; i++)
							sw.WriteLine(readLines.GetValue(i));
					}
					sw.Close();
				}

				EEFileViewer dlg = new EEFileViewer(tempFn);
				dlg.Text = catEntry.ToUpper();
				dlg.Icon = new Icon(typeof(Global), "icons.entry.ico");

				// show modal dialog with editor
				dlg.ShowDialog();

				// clean up
				System.IO.File.Delete(tempFn);

			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Cannot open the catalog entry {0}.  Reason: {1}", catEntry, ex.Message));
			}

		}
	
		#endregion



	}
}

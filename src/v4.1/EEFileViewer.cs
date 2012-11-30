// ---------------------------------------------------------------
// Copyright 2004, SAS Institute Inc.
// ---------------------------------------------------------------

using System;

namespace CatalogExplorer
{
	/// <summary>
	/// Simple form that contains the enhanced editor control
	/// </summary>
	public class EEFileViewer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panelPlaceholder;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// interop class for the SAS enhanced editor control
		// requires reference to SAS.AxInterop.EDITORCONTROLLib.dll
		// and SAS.Interop.EDITORCONTROLLib.dll
		// both are C:\Program Files\SAS\Shared Files\BIClientTasks\4 by default 	
		private AxEDITORCONTROLLib.AxEditorControl ctlEditor = null;
		
		// file property
		private string file = "";
		public string File
		{
			set
			{
				file = value;
			}
		}

		// Special ctor that takes a filename argument
		public EEFileViewer(string file)
		{
			InitializeComponent();
			this.file = file;
		}

		public EEFileViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// creat the editor control
			ctlEditor = new AxEDITORCONTROLLib.AxEditorControl();
			ctlEditor.Location = panelPlaceholder.Location;
			ctlEditor.Visible = true;
			ctlEditor.Dock = panelPlaceholder.Dock;

			// add handler for when the control is finished being created
			ctlEditor.HandleCreated += new EventHandler(ctlEditor_HandleCreated);

			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.ctlEditor});
		}

		// control is created - now open file
		private void ctlEditor_HandleCreated(object sender, EventArgs e)
		{
			if (file.Length>0)
				ctlEditor.OpenFile(file);
			ctlEditor.SetLanguage(4); // set language to SAS for syntax coloring
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
			this.panelPlaceholder = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panelPlaceholder
			// 
			this.panelPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelPlaceholder.Location = new System.Drawing.Point(0, 0);
			this.panelPlaceholder.Name = "panelPlaceholder";
			this.panelPlaceholder.Size = new System.Drawing.Size(421, 231);
			this.panelPlaceholder.TabIndex = 0;
			this.panelPlaceholder.Visible = false;
			// 
			// EEFileViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(421, 231);
			this.Controls.Add(this.panelPlaceholder);
			this.Name = "EEFileViewer";
			this.Text = "File viewer";
			this.ResumeLayout(false);

		}
		#endregion


	}
}

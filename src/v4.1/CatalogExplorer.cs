// ---------------------------------------------------------------
// Copyright 2004, SAS Institute Inc.
// ---------------------------------------------------------------

// for XMLTextWriter and XMLTextReader
	// for StreamReader and StreamWriter
	// for StringBuilder
using System.Windows.Forms;
using SAS.Shared.AddIns;
// for access to the Add-in interfaces

namespace CatalogExplorer
{
	/// <summary>
	/// Present a UI for viewing SAS catalog entries
	/// </summary>
	public class CatalogExplorer : ISASTask, ISASTaskDescription, ISASTaskAddIn
	{
		public CatalogExplorer()
		{
		}
	
		private ISASTaskConsumer consumer = null;

		#region ISASTaskAddIn Members

		public bool VisibleInManager
		{
			get
			{
				return true;
			}
		}

		public string AddInName
		{
			get
			{
				return "SAS Catalog Explorer";
			}
		}

		public void Disconnect()
		{
			consumer = null;
		}

		public int Languages(out string[] Items)
		{
			Items = new string[] { "en-US" };
			return 1;
		}

		public string AddInDescription
		{
			get
			{				
				return "Provides a window to explore the contents of SAS catalogs on a server";
			}
		}

		public bool Connect(ISASTaskConsumer Consumer)
		{
			consumer = Consumer;
			return true;
		}

		public string Language
		{
			set
			{
			}
		}

		#endregion

		#region ISASTaskDescription Members

		public string ProductsRequired
		{
			get
			{
				return "BASE";
			}
		}

		public bool StandardCategory
		{
			get
			{
				return false;
			}
		}

		public bool GeneratesListOutput
		{
			get
			{
				return false;
			}
		}

		public string TaskName
		{
			get
			{
				return "Catalog Explorer";
			}
		}

		public string Validation
		{
			get
			{
				return "SAS-supplied example";
			}
		}

		public string TaskCategory
		{
			get
			{
				return "SAS Press Examples";
			}
		}

		public string IconAssembly
		{
			get
			{
				// return the full path/name of this assembly
				return System.Reflection.Assembly.GetExecutingAssembly().Location;
			}
		}

		public string Clsid
		{
			get
			{
				return "26891660-f177-4093-975c-987fbe3cc9e8";
			}
		}

		public ShowType TaskType
		{
			get
			{
				return ShowType.Wizard;
			}
		}

		public bool RequiresData
		{
			get
			{
				return false;
			}
		}

		public string IconName
		{
			get
			{
				return "CatalogExplorer.icons.catalog.ico";
			}
		}

		public int MinorVersion
		{
			get
			{
				return 0;
			}
		}

		public int MajorVersion
		{
			get
			{
				return 1;
			}
		}

		public int NumericColsRequired
		{
			get
			{
				return 0;
			}
		}

		public string TaskDescription
		{
			get
			{
				return "Provide a window to let you explorer the contents of SAS catalogs.";
			}
		}

		public bool GeneratesSasCode
		{
			get
			{
				return true;
			}
		}

		public string FriendlyName
		{
			get
			{
				return "Catalog Explorer";
			}
		}

		public string ProcsUsed
		{
			get
			{
				return "CATALOG";
			}
		}

		public string WhatIsDescription
		{
			get
			{
				return "Provide a window to let you explorer the contents of SAS catalogs.";
			}
		}

		public string ProductsOptional
		{
			get
			{
				return "";
			}
		}

		#endregion
	
		#region ISASTask Members

		public string RunLog
		{
			get
			{
				return null;
			}
		}

		public string XmlState
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public void Terminate()
		{
		}

		public OutputData OutputDataInfo(int Index, out string Source, out string Label)
		{
			Source = null;
			Label = null;
			return new OutputData ();
		}

		public string SasCode
		{
			get
			{
				return null;
			}
		}

		public bool Initialize()
		{
			return true;
		}

		public ShowResult Show(IWin32Window Owner)
		{	
			CatalogExplorerForm dlg = new CatalogExplorerForm();
			dlg.Consumer = consumer;
			dlg.ShowDialog(Owner);
			return ShowResult.Canceled;
		}

		public string Label
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int OutputDataCount
		{
			get
			{
				return 0;
			}
		}

		#endregion
	}
}

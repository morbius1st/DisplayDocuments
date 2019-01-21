#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Private.Windows;
using Autodesk.Private.Windows.ToolBars;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using ComboBox = Autodesk.Revit.UI.ComboBox;

using UIFramework;


using static UtilityLibrary.MessageUtilities2;
using Orientation = System.Windows.Controls.Orientation;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using TaskDialogIcon = Autodesk.Revit.UI.TaskDialogIcon;
using TextBox = Autodesk.Revit.UI.TextBox;

#endregion

namespace DisplayDocuments
{
	class Ribbon : IExternalApplication
	{
		// application: launch with revit - setup interface elements
		// display information

		private const string TAB_NAME = "AO Tools";
		private const string PANEL_INFO_NAME = "Display Office Documents";

		private static string AddInPath = typeof(Ribbon).Assembly.Location;

		private const string SMALLICON_DOCS = "information16.png";
//		private const string LARGEICON_DOCS = "information32.png";
//
//		private const string PANEL_INFO_ID = "id_Info";
//		private const string CLASSPATH_DOCS = "DisplayDocuments.ButtonOp_";
//
//		private const string DISPLAY_DOCUMENT_NAME = "Display Documents";
//		private const string NEW_FEATURES_NAME = "New Features";

		public static FileMgr fm;

		public static PushButton pb2;

		private string exceptMessage = null;

//		private bool ribbonTabInit = false;
//		private Autodesk.Windows.RibbonControl rc;
//		private Autodesk.Windows.RibbonTab rt;


//		public static SplitButton sb;
//		public static Button btn;

		public Result OnStartup(UIControlledApplication app)
		{
			try
			{
				
				// create the ribbon tab first - this is the top level
				// ui item,  below this will be the panel that is "on" the tab
				//  and below this will be a split button that is "on" the panel

				// first try to create the tab
				try
				{
					app.CreateRibbonTab(TAB_NAME);
				}
				catch (Exception)
				{
					// might already exist - do nothing
				}

//				RibbonControl rc = new RibbonControl(app);
//
//				Autodesk.Windows.RibbonTab rt = rc.FindTab(TAB_NAME);

				// tab created or exists

				// create the ribbon panel if needed
				RibbonPanel ribbonPanel = null;

				// check to see if the panel already exists
				// get the Panel within the tab name
				List<RibbonPanel> rp = new List<RibbonPanel>();

				rp = app.GetRibbonPanels(TAB_NAME);

//				MakeRibbonPanel();

				foreach (RibbonPanel rpx in rp)
				{
					if (rpx.Name.ToUpper().Equals(PANEL_INFO_NAME.ToUpper()))
					{
						ribbonPanel = rpx;
					}
				}

				// if panel not found
				// add the panel if it does not exist
				if (ribbonPanel == null)
				{
					// create the ribbon panel on the tab given the tab's name
					// FYI - leave off the ribbon panel's name to put onto the "add-in" tab
					ribbonPanel = app.CreateRibbonPanel(TAB_NAME, PANEL_INFO_NAME);
					ribbonPanel.Title = PANEL_INFO_NAME;
				}

				//create a button for the command
				if (!AddStack(ribbonPanel))
				{
					TaskDialog td = new TaskDialog("Ribbon Control Failed");
					td.MainInstruction = "Ribbon Control Setup Failed";

					if (exceptMessage != null)
					{
						td.MainContent = exceptMessage;
					}

					td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
					td.Show();

					// create the split button failed
//					MessageBox.Show("Failed to Add the Ribbon Control!", "Information",
//						MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return Result.Failed;
				}

			}
			catch
			{
				return Result.Failed;
			}

			return Result.Succeeded;
		}

		public Result OnShutdown(UIControlledApplication a)
		{

			try
			{
				return Result.Succeeded;
			}
			catch
			{
				
				return Result.Failed;
			}
		}

		public static string SetButtonText
		{
			set
			{
				if (value != null)
					pb2.ItemText = value;
			}
		}

		private bool AddStack(RibbonPanel ribbonPanel)
		{
			try
			{
//				TextBoxData tbd1 = CreateTextBox("tbx_displayDox1");
//
//				if (tbd1 == null) return false;
//				
//				TextBoxData tbd2 = CreateTextBox("tbx_displayDox2");
//
//				if (tbd2 == null) return false;

				PushButtonData pbd1 = createButton("pb1", "Select Document to Display");
				if (pbd1 == null) return false;

				PushButtonData pbd2 = createButton("pb2", "Show");
				if (pbd2 == null) return false;

			
				ComboBoxData cbd = new ComboBoxData("DisplayDoxCbx");

				IList<RibbonItem> rbi = ribbonPanel.AddStackedItems(pbd1, cbd, pbd2);

				if (!AddComboBox(rbi[1] as ComboBox))
				{
					return false;
				}

				PushButton pb1 = rbi[0] as PushButton;

				pb1.Enabled = false;

				pb2 = rbi[2] as PushButton;

				pb2.ItemText = "Display " + 
					fm.DocxList[0].Fields[(int) FileMgr.DocxFields.Title];
				ShowDocument.pathAndFile = 
					fm.DocxList[0].Fields[(int) FileMgr.DocxFields.PathAndFile];

				pb2.Enabled = true;
			}
			catch (Exception e)
			{
				exceptMessage = e.Message;

				return false;
			}
			return true;
		}

		private TextBoxData CreateTextBox(string name)
		{
			TextBoxData tbd = new TextBoxData(name);

			tbd.Image = null;

			return tbd;
		}


		private bool AddComboBox(ComboBox cbx)
		{
			fm = new FileMgr();

			if (!fm.Status) return false;

//			ComboBoxData cbxData = new ComboBoxData("DisplayDoxCbx");
//			ComboBox cbx = ribbonPanel.AddItem(cbxData) as ComboBox;
			ComboBoxMemberData cbxMD;
			BitmapImage bm = Util.GetBitmapImage(SMALLICON_DOCS);

			cbx.ItemText = "Display Information";
			cbx.ToolTip = "Select an Item to Display";
			cbx.LongDescription = "Select an item from the list" 
				+ nl + "press the button below to" 
				+ nl + "display the Information";
//			cbx.CurrentChanged += ShowDocument.Cbx_CurrentChanged;
			cbx.DropDownClosed += ShowDocument.Cbx_DropDownClosed;

			int idx = 0;

			foreach (DocxItem di in fm.DocxList)
			{
				cbxMD = new ComboBoxMemberData(idx++.ToString(),
					di.Fields[(int) FileMgr.DocxFields.Title]);

				if (di.Key.Equals(DocxItem.NONE))
				{
					cbxMD.GroupName = "";
				}
				else
				{
					cbxMD.GroupName = di.Key;
				}

				cbxMD.Image = bm;
				cbxMD.ToolTip = di.Fields[(int) FileMgr.DocxFields.Tip];
				cbx.AddItem(cbxMD);
			}

			return true;
		}


		private PushButtonData createButton(string ButtonName,
			string ButtonText)
		{
			PushButtonData pdData;

			try
			{
				pdData = new PushButtonData(ButtonName, ButtonText, AddInPath, "DisplayDocuments.ShowDocument");
				pdData.Image = null;
				pdData.LargeImage = null;
				pdData.ToolTip = null;

			}
			catch
			{
				return null;
			}

			return pdData;
		}


//
//		// code to add a split button
//		private bool AddSplitButton(RibbonPanel ribbonPanel)
//		{
//			FileMgr fm = new FileMgr();
//			string[,] DocList = fm.DocList;
//
////			VerifyIfShowNewFeature();
//
//			SplitButtonData sbData = new SplitButtonData("splitButton1", "Split");
//			sb = ribbonPanel.AddItem(sbData) as SplitButton;
//
//			PushButtonData pbd;
//
//			int count = 0;
//
//			// for each pdf file, setup a push button and add to the split button
//			for (int i = 0; i < fm.Size; i++)
//			{
//				pbd = createButton(DocList[i, (int) FileMgr.DocData.Name],
//					DocList[i, (int) FileMgr.DocData.Description],
//					DocList[i, (int) FileMgr.DocData.Tip], i);
//
//				if (pbd == null) 
//					continue;
//
//				count++;
//				sb.AddPushButton(pbd);
//			}
//
//			if (count == 0)
//				return false;
//
//			return true;
//		}
//
//
//		private PushButtonData createButton(string ButtonName, string ButtonText, string ToolTip, int count)
//		{
//			PushButtonData pdData;
//
//			ButtonText = formatButtonText(ButtonText);
//
//			string FullClassPath = string.Format("{0}{1:00}", CLASSPATH_DOCS, count);
//
//			try
//			{
//				pdData = new PushButtonData(ButtonName, ButtonText, AddInPath, FullClassPath);
//				pdData.Image = Util.GetBitmapImage(SMALLICON_DOCS);
//				pdData.LargeImage = Util.GetBitmapImage(LARGEICON_DOCS);
//				pdData.ToolTip = ToolTip;
//
//			}
//			catch (Exception ex)
//			{
//				return null;
//			}
//
//			return pdData;
//		}
//
//
//		private string formatButtonText(string s)
//		{
//			// cap the string to a maximum of 32 characters
//			if (s.Length > 36)
//			{
//				// find first space starting from the end of the text
//				int i = s.LastIndexOf(' ');
//
//				if (i > 0)
//				{
//					// truncate string 
//					s = s.Substring(0, i - 1);
//				}
//				else
//				{
//					// return the first 36 characters of the string
//					// broken into 3 parts
//					return s.Substring(0, 11) + "\n" + s.Substring(12, 23) + "\n" + s.Substring(24, 35);
//				}
//			}
//		
//			// break string into small parts of about 12 characters
//
//			string[] t = s.Split(' ');
//
//			string start = t[0];
//			string final = "";
//
//			for (int i = 1; i < t.Length; i++)
//			{
//				if (start.Length + t[i].Length > 12)
//				{
//					final += start;
//
//					if (i != t.Length)
//					{
//						final += "\n";
//					}
//
//					start = t[i];
//				}
//				else
//				{
//					start += " " + t[i];
//				}
//			}
//
//			return final + start;
//
//		}

//		private void VerifyIfShowNewFeature()
//		{
//
//			long lastFileAccessTime = Util.GetNewFeatureFileLastAccTime();
//
//			if (lastFileAccessTime == -1)
//			{
//				TaskDialog.Show(NEW_FEATURES_NAME, "Cannot access the New Features document");
//
//				return;
//			}
//
//			long lastRegAccessTime = Util.GetNewFeatureRegLastAccTime(lastFileAccessTime);
//
//			if (lastRegAccessTime == -1)
//			{
//				TaskDialog.Show(NEW_FEATURES_NAME, "Cannot access the New Features key");
//
//				return;
//			}
//
//			if (lastFileAccessTime != lastRegAccessTime)
//			{
//				Util.DisplayFile(FileMgr.NEW_FEATURE_FILEPATH);
//
//				Util.SetNewFeatureRegLastAccTime(lastFileAccessTime);
//			}
//		}

//		private void Cbx_CurrentChanged(object sender,
//			Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs e)
//		{
//			// cast sender as TextBox to retrieve text value
//			ComboBox cbx = sender as ComboBox;
//			ComboBoxMember cbxMember = cbx.Current as ComboBoxMember;
//
//			logMsgLn2("CurrentChanged", cbxMember.ItemText);
//		}

//		private void Cbx_DropDownClosed(object sender,
//			Autodesk.Revit.UI.Events.ComboBoxDropDownClosedEventArgs e) 
//		{
//			// cast sender as TextBox to retrieve text value
//			ComboBox cbx = sender as ComboBox;
//			ComboBoxMember cbxMember = cbx.Current as ComboBoxMember;
//
//			logMsgLn2("DropDownClosed", cbxMember.ItemText);
//		}
//
//		private bool MakeRibbonPanel()
//		{
//			if (!GetRibbonTab()) return false;
//
//
//			RibbonPanelSource rs = getRPSource(PANEL_INFO_NAME, PANEL_INFO_ID);
//
//			Autodesk.Windows.RibbonPanel rp = getRP(rs);
//
//
//			RibbonSubPanelSource pRow1Source = new Autodesk.Windows.RibbonSubPanelSource()
//			{
//				
//				Description = null, Id = "row1_shr", Name = null, Tag = null, UID = null
//			};
//
//			RibbonRowPanel row1 = new RibbonRowPanel()
//			{
//				
//				Height = 32, Id = "pRow1", IsEnabled = true, Source = pRow1Source
//			};
//
//
//
//			RibbonSubPanelSource pRow2Source = new Autodesk.Windows.RibbonSubPanelSource()
//			{
//				Description = null, Id = "row2_shr", Name = null, Tag = null, UID = null
//			};
//
//			RibbonRowPanel row2 = new RibbonRowPanel()
//			{
//				Height = 32, Id = "pRow2", IsEnabled = true, Source = pRow2Source
//			};
//			
//
//			RibbonSubPanelSource pRow3Source = new Autodesk.Windows.RibbonSubPanelSource()
//			{
//				Description = null, Id = "row3_shr", Name = null, Tag = null, UID = null
//			};
//
//			RibbonRowPanel row3 = new RibbonRowPanel()
//			{
//				Height = 32, Id = "pRow3", IsEnabled = true, Source = pRow3Source
//			};
//
//
//
////			Autodesk.Windows.RibbonSeparator pBreak = new Autodesk.Windows.RibbonSeparator();
////			pBreak.SeparatorStyle = Autodesk.Windows.RibbonSeparatorStyle.Line;
//
//
//			Autodesk.Windows.RibbonLabel rl1 = new RibbonLabel()
//			{
//				Height = 30, Width = 100, Text = "ribbon text"
//
//			};
//			
//			Autodesk.Windows.RibbonLabel rl3 = new RibbonLabel()
//			{
//				Height = 30, Width = 100, Text = ""
//
//			};
//
//			RibbonCommandItem[] rci = new []
//			{
//				new RibbonCommandItem() {Text = "item 1"},
//				new RibbonCommandItem() {Text = "item 2"},
//				new RibbonCommandItem() {Text = "item 3"},
//				new RibbonCommandItem() {Text = "item 4"}
//			};
//
//			Autodesk.Windows.RibbonCombo rcbo = new RibbonCombo()
//			{
//				Height = 30, Width = 100, Name = "rcbo", Id = "id_rcbo",
//				Tag = null, ShowText = false, Text = null, UID = null, 
//			};
//
//			rci[0].Description = "an item 1";
//			rci[0].Id = "id_item1";
//			rci[0].Image = null;
//			rci[0].LargeImage = null;
//			rci[0].ShowImage = false;
//			rci[0].ShowText = true;
//			rci[0].IsToolTipEnabled = false;
//			rci[0].Name = "name_item1";
//
//			rci[1].Description = "an item 2";
//			rci[1].Id = "id_item2";
//			rci[1].Image = null;
//			rci[1].LargeImage = null;
//			rci[1].ShowImage = false;
//			rci[1].ShowText = true;
//			rci[1].IsToolTipEnabled = false;
//			rci[1].Name = "name_item2";
//
//			rci[2].Description = "an item 3";
//			rci[2].Id = "id_item3";
//			rci[2].Image = null;
//			rci[2].LargeImage = null;
//			rci[2].ShowImage = false;
//			rci[2].ShowText = true;
//			rci[2].IsToolTipEnabled = false;
//			rci[2].Name = "name_item3";
//
//			rci[3].Description = "an item 4";
//			rci[3].Id = "id_item4";
//			rci[3].Image = null;
//			rci[3].LargeImage = null;
//			rci[3].ShowImage = false;
//			rci[3].ShowText = true;
//			rci[3].IsToolTipEnabled = false;
//			rci[3].Name = "name_item4";
//
//
////			rcbo.MenuItems.Add(rci[0]);
////			rcbo.MenuItems.Add(rci[1]);
//
//			rcbo.Items.Add(rci[0]);
//			rcbo.Items.Add(rci[1]);
//			rcbo.Items.Add(rci[2]);
//			rcbo.Items.Add(rci[3]);
//
//			rcbo.Current = rci[0];
//// nope
////			rcbo.ItemsView.AddNewItem("item a");
////			rcbo.ItemsView.AddNewItem("item b");
////			rcbo.Current = "item b";
//
//
//			rcbo.CurrentChanged += ShowDocument.Rcbo_CurrentChanged;
//
////			Autodesk.Windows.ComponentManager.UIElementActivated
//
//
//			row1.Source.Items.Add(rl1);
////			row1.Source.Items.Add(pBreak);
//			rp.Source.Items.Add(row1);
//
//			rp.Source.Items.Add(new Autodesk.Windows.RibbonRowBreak());
//
//			row2.Source.Items.Add(rcbo);
//			rp.Source.Items.Add(row2);
//
//			rp.Source.Items.Add(new Autodesk.Windows.RibbonRowBreak());
//
//			row3.Source.Items.Add(rl3);
//			rp.Source.Items.Add(row3);
//
//			rt.Panels.Add(rp);
//
//			return true;
//		}
//
//		
//
//		private bool GetRibbonTab()
//		{
//			if (ribbonTabInit) return (rc != null);
//
//			ribbonTabInit = true;
//
//			rc = Autodesk.Windows.ComponentManager.Ribbon;
//
//			if (rc == null) return false;
//
//			rt = rc.FindTab(TAB_NAME);
//
//			if (rt == null) return false;
//
//			return true;
//		}
//
//		private RibbonPanelSource getRPSource(string title, string id)
//		{
//			RibbonPanelSource rs = new RibbonPanelSource();
//
//			rs.Title = title;
//			rs.Id = id;
//			rs.IsSlideOutPanelVisible = true;
//			rs.Description = null;
//			rs.DialogLauncher = null;
//			rs.UID = null;
//			rs.Tag = null;
//			rs.Name = null;
//
//			return rs;
//		}
//
//		private Autodesk.Windows.RibbonPanel getRP(RibbonPanelSource rps)
//		{
//			Autodesk.Windows.RibbonPanel rp = new Autodesk.Windows.RibbonPanel();
//			rp.Source = rps;
//			rp.IsVisible = true;
//			rp.IsEnabled = true;
//
//			return rp;
//		}
//
//
//
//		private bool AddStack(RibbonPanel ribbonPanel)
//		{
//			Autodesk.Windows.RibbonControl rc = Autodesk.Windows.ComponentManager.Ribbon;
//
//			if (rc == null) return false;
//
//			Autodesk.Windows.RibbonTab rt = rc.FindTab(TAB_NAME);
//
//			if (rt == null) return false;
//
//			Autodesk.Windows.RibbonPanel rp = rt.FindPanel(PANEL_INFO_NAME);
//
//			if (rp == null) return false;
//
//			RibbonLabel rl = new RibbonLabel();
//			rl.Height = 30.0;
//			rl.Width = 100.0;
//			rl.Text = "label text";
//
//			rp.Source.Items.Add(rl);
//
//			return false;
//		}

	}
}

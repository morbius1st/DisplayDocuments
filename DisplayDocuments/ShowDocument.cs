#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Windows;
using DisplayDocuments;
using ComboBox = Autodesk.Revit.UI.ComboBox;

//using static UtilityLibrary.CsExtensions;
using static UtilityLibrary.MessageUtilities2;

#endregion

namespace DisplayDocuments
{
	[Transaction(TransactionMode.Manual)]
	public class ShowDocument : IExternalCommand
	{
		public static string pathAndFile;

		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message, ElementSet elements)
		{
			Util.DisplayFile(pathAndFile);

			return Result.Succeeded;
		}


		public static void Cbx_DropDownClosed(object sender,
			Autodesk.Revit.UI.Events.ComboBoxDropDownClosedEventArgs e)
		{
			// cast sender as TextBox to retrieve text value
			ComboBox cbx = sender as ComboBox;
			ComboBoxMember cbxMember = cbx.Current as ComboBoxMember;

			logMsgLn2("CurrentClosed", cbxMember.ItemText 
				+ " :: name| " + cbxMember.Name);

			int idx;
				
			bool result = Int32.TryParse(cbxMember.Name, out idx);

			if (!result) return;

			pathAndFile = 
				Ribbon.fm.DocxList[idx].Fields[(int) FileMgr.DocxFields.PathAndFile];

			Ribbon.SetButtonText = "Display " + 
				Ribbon.fm.DocxList[idx].Fields[(int) FileMgr.DocxFields.Title];

			logMsgLn2("selected| category" + Ribbon.fm.DocxList[idx].Key,
				" :: path| " + pathAndFile);
		}
	}

//

//
//
//	// "bogus" classes that are automatically pointed to 
//	// in the setup of the splitbutton.  each class is
//	// a sub class of the primary class.  the actual 
//	// procedure is in the primary class but the name of
//	// the class (specifically the last two characters)
//	// determine the actual index of the arrayy
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_00 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_01 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_02 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_03 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_04 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_05 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_06 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_07 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_08 : ShowDocument
//	{
//	}
//
//	[Transaction(TransactionMode.Manual)]
//	public class ButtonOp_09 : ShowDocument
//	{
//	}

//		public static void Rcbo_CurrentChanged(object sender,
//			RibbonPropertyChangedEventArgs e)
//		{
//			// cast sender as TextBox to retrieve text value
//			Autodesk.Windows.RibbonCombo cbx = sender as Autodesk.Windows.RibbonCombo;
//			RibbonCommandItem cbxMember = cbx.Current as RibbonCommandItem;
//
//			logMsgLn2("CurrentChanged", cbxMember.Id + " :: name| " + cbxMember.Name);
//		}

//
//		public static void Cbx_CurrentChanged(object sender,
//			Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs e)
//		{
//			// cast sender as TextBox to retrieve text value
//			ComboBox cbx = sender as ComboBox;
//			ComboBoxMember cbxMember = cbx.Current as ComboBoxMember;
//
//			logMsgLn2("CurrentChanged", cbxMember.ItemText + " :: name| " + cbxMember.Name);
//		}

}

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
//
//			logMsgLn2("CurrentClosed", cbxMember.ItemText 
//				+ " :: name| " + cbxMember.Name);

			int idx;
				
			bool result = Int32.TryParse(cbxMember.Name, out idx);

			if (!result) return;

			pathAndFile = 
				Ribbon.fm.DocxList[idx].Fields[(int) FileMgr.DocxFields.PathAndFile];

			Ribbon.SetButtonText = "Display " + 
				Ribbon.fm.DocxList[idx].Fields[(int) FileMgr.DocxFields.Title];
//
//			logMsgLn2("selected| category| " + Ribbon.fm.DocxList[idx].Key,
//				" :: path| " + pathAndFile);
		}
	}
}

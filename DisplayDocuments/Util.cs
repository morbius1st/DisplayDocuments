using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace DisplayDocuments
{
	internal static class Util
	{
//		private const string REG_REVIT_KEY = @"Software\Autodesk\Revit\";
//		private const string REG_AO_SUB_KEY = @"AO\";
//		private const string REG_DISP_DOCS_SUB_KEY = @"DisplayDocs\";
//		private const string REG_NEW_FEATURE_ITEM_KEY = @"NewFeatureLastDateTime";

		private const string NAMESPACE_PREFIX = "DisplayDocuments.Resources";

		internal static string nl = Environment.NewLine;

		// load an image from embeded resource
		internal static BitmapImage GetBitmapImage(string imageName)
		{
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(NAMESPACE_PREFIX + "." + imageName);

			BitmapImage img = new BitmapImage();

			img.BeginInit();
			img.StreamSource = s;
			img.EndInit();

			return img;
		}

//		// get the registry stored last access date for the new features file
//		// return -1 if this fails- most likely due to premission problem
//		internal static long GetNewFeatureRegLastAccTime(long fileLastAccessTime)
//		{
//			try
//			{
//				RegistryKey rk =
//					Registry.CurrentUser.CreateSubKey(REG_REVIT_KEY 
//						+ REG_AO_SUB_KEY + REG_DISP_DOCS_SUB_KEY, 
//						RegistryKeyPermissionCheck.ReadWriteSubTree);
//
//				if (rk == null)
//				{
//					return 0L;
//				}
//
//				long dtValue = (long) rk.GetValue(REG_NEW_FEATURE_ITEM_KEY, 0L);
//
//				if (dtValue == 0L)
//				{
//					rk.SetValue(REG_NEW_FEATURE_ITEM_KEY,
//						fileLastAccessTime, RegistryValueKind.QWord);
//				}
//
//				rk.Dispose();
//
//				return dtValue;
//			}
//			catch
//			{
//				return -1L;
//			}
//		}

//		internal static void SetNewFeatureRegLastAccTime(long fileLastAccessTime)
//		{
//			try
//			{
//				RegistryKey rk =
//					Registry.CurrentUser.OpenSubKey(REG_REVIT_KEY
//						+ REG_AO_SUB_KEY + REG_DISP_DOCS_SUB_KEY,
//						RegistryKeyPermissionCheck.ReadWriteSubTree);
//
//				if (rk == null)
//				{
//					return;
//				}
//
//				rk.SetValue(REG_NEW_FEATURE_ITEM_KEY,
//						fileLastAccessTime, RegistryValueKind.QWord);
//
//				rk.Dispose();
//			}
//			catch { }
//		}

//		// get a number that represents the last access date for the file
//		// return -1 if this fails - most likely due to permission problem or
//		// the file does not exist for some reason
//		internal static long GetNewFeatureFileLastAccTime()
//		{
//			try
//			{
//				return File.GetLastWriteTime(
//					FileMgr.NEW_FEATURE_FILEPATH).ToFileTime();
//
//
////				return File.GetLastAccessTime(
////					FileMgr.NEW_FEATURE_FILEPATH).ToFileTime();
//			}
//			catch
//			{
//				return -1L;
//			}
//		}

		// display a pdf file based on its location in the file system
		//  this is an absolute path name
		internal static void DisplayFile(string location)
		{
			System.Diagnostics.Process process = new System.Diagnostics.Process();

			try
			{
				process.StartInfo.FileName = "explorer";
				process.StartInfo.Arguments = location;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.Start();

			}
			catch (Exception e)
			{
				Debug.Print(e.Message);
			}
		}
	}

}

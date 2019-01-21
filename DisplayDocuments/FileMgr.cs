
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using static UtilityLibrary.MessageUtilities2;
using static UtilityLibrary.CsExtensions;


// class to process the file which determines which pdf files can be displayed
// also holds constants about the files: path and pdf file list

namespace DisplayDocuments
{

	public class DocxItem : IComparable<DocxItem>
	{
		public const string NONE = "-NONE-";
		public string Key;
		public string[] Fields;

		public int CompareTo(DocxItem other)
		{
			if (this.Key.Equals(NONE)) return -1;

			return String.Compare(this.Key, other.Key, 
				StringComparison.Ordinal);
		}
	}

	public class FileMgr
	{
		private const string FILE_NAME = "Docx to Display in Revit.txt";
		private const string FILE_PATH = @"Y:\AO Commercial\Revit\Information\";

		internal const string NEW_FEATURE_PATH = @"Y:\AO Commercial\Revit\Information\New Features\";
		internal const string NEW_FEATURE_NAME = "New Features.pdf";
		internal const string NEW_FEATURE_FILEPATH = NEW_FEATURE_PATH + NEW_FEATURE_NAME;

		private const int ITEMS = 3;
		private List<DocxItem> _docxList = new List<DocxItem>();
		private static bool _initialized = false;

		public enum DocxFields
		{
			Title = 0,
			PathAndFile = 1,
			Tip = 2
		}

		public List<DocxItem> DocxList => _docxList;
		public int Count => _docxList.Count;
		public bool Status { get ; } = false;

		public FileMgr()
		{
			if (_initialized) { return; }

			try
			{
				Status = ReadDocx();
			}
			catch (Exception e)
			{
				Debug.Print("Document List could not be read");
				Debug.Print(e.Message);
			}
		}

		private bool ReadDocx()
		{
			if (_initialized) { return true; }

			try
			{
				// open the list of revit docx to display
				using (StreamReader sr =
					new StreamReader(FILE_PATH + FILE_NAME))
				{
					while (sr.Peek() >= 0)
					{
						string t = sr.ReadLine().Trim();

						if (!t.StartsWith("<", StringComparison.Ordinal)) continue;

						DocxItem di = parseDocx(t);

						if (di.Key == null) continue;

						_docxList.Add(di);
					}
				}
			}
			catch (Exception e)
			{
				Debug.Print("Document List could not be read");
				Debug.Print(e.Message);

				_initialized = true;

				return false;
			}

			_initialized = true;

			_docxList.Sort();

			return _docxList.Count > 0;
		}

		// parse a single data line
		private DocxItem parseDocx(string line)
		{
			int idx = 0;

			const string pattern = @"\s*\<(.*?)\>";

			DocxItem di = new DocxItem {Key = null, Fields = new string[ITEMS]};

			Match match = Regex.Match(line, pattern);

			int g = match.Groups.Count;

			if (match.Success)
			{
				di.Key = match.Groups[1].Value;

				if (di.Key.Equals("") || 
					di.Key.ToUpper().Equals(DocxItem.NONE))
				{
					di.Key = DocxItem.NONE;
				}

				match = match.NextMatch();

				while (match.Success)
				{
					if (idx == ITEMS) break;

					if (match.Groups[1].Success)
					{
						di.Fields[idx] = match.Groups[1].Value;
					}

					idx++;

					match = match.NextMatch();
				}
			}

			return di;
		}






//		public FileMgr()
//		{
//			int end_pos;

//			if (initialized) { return; }

//			initialized_old = true;
//
//			try
//			{
//				ReadDocx();


//				// open the list of revit applicable PDF files
//				using (StreamReader sr =
//					new StreamReader(FILE_PATH + FILE_NAME_OLD))
//				{
//					while (sr.Peek() >= 0 && count < (end - 1))
//					{
//						string t = sr.ReadLine().Trim();
//
//						end_pos = 0;
//
//						string s = getDocInfo(t, end_pos, out end_pos);
//
//						if (s == null)
//						{
//							continue;
//						}
//
//						_docList[count, (int) DocData.Description] = s;
//
//						s = getDocInfo(t, end_pos, out end_pos);
//
//						if (s == null)
//						{
//							continue;
//						}
//
//						_docList[count, (int) DocData.File] = s;
//
//						s = getDocInfo(t, end_pos, out end_pos);
//
//						if (s == null)
//						{
//							continue;
//						}
//
//						_docList[count, (int) DocData.Tip] = s;
//
//
//						_docList[count, (int) DocData.Name] = string.Format("{1:00}_{0}", BUTTON_NAME, count);
//
//						count++;
//					}
//				}
//			}
//			catch (Exception e)
//			{
//				Debug.Print("Document List could not be read");
//				Debug.Print(e.Message);
//			}
//
//		}
//
//		private string getDocInfo(string s,
//			int start_pos,
//			out int end_pos)
//		{
//			int beg_pos;
//			int comment_pos;
//
//
//			beg_pos = s.IndexOf(SEPERATOR_BEG, start_pos);
//
//			if (beg_pos < 0)
//			{
//				end_pos = -1;
//				return null;
//			}
//
//			comment_pos = s.IndexOf(COMMENT, start_pos);
//
//
//			if (comment_pos >= 0 && comment_pos < beg_pos)
//			{
//				end_pos = -1;
//				return null;
//			}
//
//			end_pos = s.IndexOf(SEPERATOR_END, beg_pos) - 1;
//
//			if (end_pos < 0)
//			{
//				return null;
//			}
//
//			return s.Substring(beg_pos + 1, end_pos - beg_pos);
//
//		}
//
//		public string[,] DocList
//		{
//			get { return _docList; }
//		}
//
//		public int Size
//		{
//			get { return count; }
//		}



	}
}
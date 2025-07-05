using System.Drawing;

namespace Odotocodot.OneNote.Linq.Parsers
{
	internal static class Constants
	{
		internal static class Elements
		{
			internal const string Notebook = "Notebook";
			internal const string SectionGroup = "SectionGroup";
			internal const string Section = "Section";
			internal const string Page = "Page";
			internal const string NotebookList = "Notebooks";
		}

		internal static class Attributes
		{
			internal const string ID = "ID";
			internal const string Name = "name";
			internal const string NickName = "nickname";
			internal const string Path = "path";
			internal const string Color = "color";
			internal const string IsUnread = "isUnread";
			internal const string LastModifiedTime = "lastModifiedTime";
			internal const string DateTime = "dateTime";
			internal const string PageLevel = "pageLevel";
			internal const string IsInRecycleBin = "isInRecycleBin";
			internal const string Encrypted = "encrypted";
			internal const string Locked = "locked";
			internal const string IsDeletedPages = "isDeletedPages";
			internal const string IsRecycleBin = "isRecycleBin";
		}

		internal const string NamespaceUri = "http://schemas.microsoft.com/office/onenote/2013/onenote";
		internal const char RelativePathSeparator = '\\';
		internal const string RelativePathSeparatorString = "\\";

		internal static Color? GetColor(in string color)
		{
			if (color == "none")
				return null;

			return ColorTranslator.FromHtml(color);
		}
	}
}
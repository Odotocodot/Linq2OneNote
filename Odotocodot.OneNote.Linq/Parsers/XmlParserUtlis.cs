using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Odotocodot.OneNote.Linq.Parsers
{
    internal static class XmlParserUtlis
    {
        internal const char RelativePathSeparator = '\\';
        internal const string NamespaceUri = "http://schemas.microsoft.com/office/onenote/2013/onenote";
        internal static class Names
        {
            internal const string Notebook = "Notebook";
            internal const string SectionGroup = "SectionGroup";
            internal const string Section = "Section";
            internal const string Page = "Page";
        }
        
        internal static readonly Dictionary<string, Action<OneNoteNotebook, string>> notebookSetters =
            new Dictionary<string, Action<OneNoteNotebook, string>>
        {
            { "ID", (item, value) => item.ID = value },
            { "name", (item, value) => item.Name = value },
            { "lastModifiedTime", (item, value) => item.LastModified = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) },
            { "isUnread", (item, value) => item.IsUnread = bool.Parse(value)},

            { "nickname", (notebook, value) => notebook.NickName = value },
            { "path", (notebook, value) => notebook.Path = value },
            { "color", (notebook, value) => notebook.Color = ColorTranslator.FromHtml(value) }
        };

        internal static readonly Dictionary<string, Action<OneNoteSectionGroup, string>> sectionGroupSetters =
            new Dictionary<string, Action<OneNoteSectionGroup, string>>
        {
            { "ID", (item, value) => item.ID = value },
            { "name", (item, value) => item.Name = value },
            { "lastModifiedTime", (item, value) => item.LastModified = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) },
            { "isUnread", (item, value) => item.IsUnread = bool.Parse(value)},

            { "path", (sectionGroup, value) => sectionGroup.Path = value },
            { "isRecycleBin", (sectionGroup, value) => sectionGroup.IsRecycleBin =bool.Parse(value) },
        };

        internal static readonly Dictionary<string, Action<OneNoteSection, string>> sectionSetters =
            new Dictionary<string, Action<OneNoteSection, string>>
        {
            { "ID", (item, value) => item.ID = value },
            { "name", (item, value) => item.Name = value },
            { "lastModifiedTime", (item, value) => item.LastModified = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) },
            { "isUnread", (item, value) => item.IsUnread = bool.Parse(value)},

            { "path", (section, value) => section.Path = value },
            { "color", (section, value) => section.Color = ColorTranslator.FromHtml(value) },
            { "encrypted", (section, value) => section.Encrypted = bool.Parse(value)},
            { "locked", (section, value) => section.Locked = bool.Parse(value) },
            { "isInRecycleBin", (section, value) => section.IsInRecycleBin = bool.Parse(value) },
            { "isDeletedPages", (section, value) => section.IsDeletedPages = bool.Parse(value) }
        };

        internal static readonly Dictionary<string, Action<OneNotePage, string>> pageSetters =
            new Dictionary<string, Action<OneNotePage, string>>
        {
            { "ID", (item, value) => item.ID = value },
            { "name", (item, value) => item.Name = value },
            { "lastModifiedTime", (item, value) => item.LastModified = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) },
            { "isUnread", (item, value) => item.IsUnread = bool.Parse(value)},

            { "dateTime", (page, value) => page.Created = DateTime.Parse(value) },
            { "pageLevel", (page, value) => page.Level = int.Parse(value) },
            { "isInRecycleBin", (page, value) => page.IsInRecycleBin = bool.Parse(value) }
        };
        
        
        private static Color? GetColor(string value)
        {
            if (value == "none")
                return null;

            return ColorTranslator.FromHtml(value);
        }

    }
}
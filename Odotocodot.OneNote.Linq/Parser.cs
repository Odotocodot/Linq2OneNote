using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Odotocodot.OneNote.Linq
{
    internal static class Parser
    {
        internal const char RelativePathSeparator = '\\';
        private const string NamespaceUri = "http://schemas.microsoft.com/office/onenote/2013/onenote";
        private static readonly XName NotebookXName = XName.Get("Notebook", NamespaceUri);
        private static readonly XName SectionGroupXName = XName.Get("SectionGroup", NamespaceUri);
        private static readonly XName SectionXName = XName.Get("Section", NamespaceUri);
        private static readonly XName PageXName = XName.Get("Page", NamespaceUri);

        private static readonly Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>> runtimeParser = new()
        {
            {NotebookXName, ParseNotebook},
            {SectionGroupXName, ParseSectionGroup},
            {SectionXName, ParseSection},
            {PageXName, ParsePage}
        };
        
        private static readonly Dictionary<string, Action<OneNotePage, XAttribute>> pageSetters = new()
        {
            { "ID", (item, attribute) => item.ID = attribute.Value },
            { "name", (item, attribute) => item.Name = attribute.Value },
            { "lastModifiedTime", (item, attribute) => item.LastModified = (DateTime)attribute },
            { "isUnread", (item, attribute) => item.IsUnread = (bool)attribute },
            
            { "dateTime", (page, attribute) => page.Created = (DateTime)attribute },
            { "pageLevel", (page, attribute) => page.Level = (int)attribute },
            { "isInRecycleBin", (page, attribute) => page.IsInRecycleBin = (bool)attribute }
        };
        
        private static readonly Dictionary<string, Action<OneNoteSection, XAttribute>> sectionSetters = new()
        {
            { "ID", (item, attribute) => item.ID = attribute.Value },
            { "name", (item, attribute) => item.Name = attribute.Value },
            { "lastModifiedTime", (item, attribute) => item.LastModified = (DateTime)attribute },
            { "isUnread", (item, attribute) => item.IsUnread = (bool)attribute },
            
            { "path", (section, attribute) => section.Path = attribute.Value },
            { "color", (section, attribute) => section.Color = GetColor(attribute) },
            { "encrypted", (section, attribute) => section.Encrypted = (bool)attribute },
            { "locked", (section, attribute) => section.Locked = (bool)attribute },
            { "isInRecycleBin", (section, attribute) => section.IsInRecycleBin = (bool)attribute },
            { "isDeletedPages", (section, attribute) => section.IsDeletedPages = (bool)attribute }
        };

        private static readonly Dictionary<string, Action<OneNoteSectionGroup, XAttribute>> sectionGroupSetters = new()
        {
            { "ID", (item, attribute) => item.ID = attribute.Value },
            { "name", (item, attribute) => item.Name = attribute.Value },
            { "lastModifiedTime", (item, attribute) => item.LastModified = (DateTime)attribute },
            { "isUnread", (item, attribute) => item.IsUnread = (bool)attribute },

            { "path", (sectionGroup, attribute) => sectionGroup.Path = attribute.Value },
            { "isRecycleBin", (sectionGroup, attribute) => sectionGroup.IsRecycleBin = (bool)attribute }
        };
        
        private static readonly Dictionary<string, Action<OneNoteNotebook, XAttribute>> notebookSetters = new()
        {
            { "ID", (item, attribute) => item.ID = attribute.Value },
            { "name", (item, attribute) => item.Name = attribute.Value },
            { "lastModifiedTime", (item, attribute) => item.LastModified = (DateTime)attribute },
            { "isUnread", (item, attribute) => item.IsUnread = (bool)attribute },

            { "nickname", (notebook, attribute) => notebook.NickName = attribute.Value },
            { "path", (notebook, attribute) => notebook.Path = attribute.Value },
            { "color", (notebook, attribute) => notebook.Color = GetColor(attribute) }
        };
        
        //Unknown at runtime
        internal static IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            var root = XElement.Parse(xml);
            return runtimeParser[root.Name](root, parent);   
        }
        
        private static OneNotePage ParsePage(XElement element, IOneNoteItem parent)
        {
            var page = new OneNotePage();
            
            foreach (var attribute in element.Attributes())
            {
                if (pageSetters.TryGetValue(attribute.Name.LocalName, out var setter))
                {
                    setter(page, attribute);
                }
            }
            
            page.Section = (OneNoteSection)parent;
            page.Notebook = parent.Notebook;
            page.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{page.Name}";
            return page;
        }

        private static OneNoteSection ParseSection(XElement element, IOneNoteItem parent)
        {
            var section = new OneNoteSection();
            foreach (var attribute in element.Attributes())
            {
                if (sectionSetters.TryGetValue(attribute.Name.LocalName, out var setter))
                {
                    setter(section, attribute);
                }
            }
            
            section.Parent = parent;
            section.Notebook = parent.Notebook;
            section.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{section.Name}";
            section.Pages = element.Elements(PageXName)
                                   .Select(e => ParsePage(e, section));
            return section;
        }

        private static Color? GetColor(XAttribute attribute) 
            => attribute.Value == "none" ? null : ColorTranslator.FromHtml(attribute.Value);

        private static OneNoteSectionGroup ParseSectionGroup(XElement element, IOneNoteItem parent)
        {
            var sectionGroup = new OneNoteSectionGroup();
            foreach (var attribute in element.Attributes())
            {
                if (sectionGroupSetters.TryGetValue(attribute.Name.LocalName, out var setter))
                {
                    setter(sectionGroup, attribute);
                }
            }
            sectionGroup.Notebook = parent.Notebook;
            sectionGroup.Parent = parent;
            sectionGroup.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{sectionGroup.Name}";
            sectionGroup.Sections = element.Elements(SectionXName)
                                           .Select(e => ParseSection(e, sectionGroup));
            sectionGroup.SectionGroups = element.Elements(SectionGroupXName)
                                                .Select(e => ParseSectionGroup(e, sectionGroup));
            return sectionGroup;

        }

        private static OneNoteNotebook ParseNotebook(XElement element, IOneNoteItem _)
        {
            var notebook = new OneNoteNotebook();
            foreach (var attribute in element.Attributes())
            {
                if (notebookSetters.TryGetValue(attribute.Name.LocalName, out var setter))
                {
                    setter(notebook, attribute);
                }
            }
            notebook.Notebook = notebook;
            notebook.Sections = element.Elements(SectionXName)
                                       .Select(e => ParseSection(e, notebook));
            notebook.SectionGroups = element.Elements(SectionGroupXName)
                                            .Select(e => ParseSectionGroup(e, notebook));
            return notebook;
        }
        internal static IEnumerable<OneNoteNotebook> ParseNotebooks(string xml) => XElement.Parse(xml)
                                                                                           .Elements(NotebookXName)
                                                                                           .Select(e => ParseNotebook(e, null));
    }
}

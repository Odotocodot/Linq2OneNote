using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Odotocodot.OneNote.Linq.Parsers
{
    using static Constants;
    internal class XmlParserXElement : IXmlParser
    {
        private static readonly XName NotebookXName = XName.Get(Elements.Notebook, NamespaceUri);
        private static readonly XName SectionGroupXName = XName.Get(Elements.SectionGroup, NamespaceUri);
        private static readonly XName SectionXName = XName.Get(Elements.Section, NamespaceUri);
        private static readonly XName PageXName = XName.Get(Elements.Page, NamespaceUri);

        private static readonly Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>> runtimeParser =
            new Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>>
        {
            { NotebookXName, ParseNotebook},
            { SectionGroupXName, ParseSectionGroup},
            { SectionXName, ParseSection},
            { PageXName, ParsePage}
        };

        private static readonly Dictionary<string, Action<OneNotePage, XAttribute>> pageSetters =
            new Dictionary<string, Action<OneNotePage, XAttribute>>
        {
            { Attributes.ID, (item, attribute) => item.ID = attribute.Value },
            { Attributes.Name, (item, attribute) => item.Name = attribute.Value },
            { Attributes.LastModifiedTime, (item, attribute) => item.LastModified = (DateTime)attribute },
            { Attributes.IsUnread, (item, attribute) => item.IsUnread = (bool)attribute },

            { Attributes.DateTime, (page, attribute) => page.Created = (DateTime)attribute },
            { Attributes.PageLevel, (page, attribute) => page.Level = (int)attribute },
            { Attributes.IsInRecycleBin, (page, attribute) => page.IsInRecycleBin = (bool)attribute }
        };

        private static readonly Dictionary<string, Action<OneNoteSection, XAttribute>> sectionSetters =
            new Dictionary<string, Action<OneNoteSection, XAttribute>>
        {
            { Attributes.ID, (item, attribute) => item.ID = attribute.Value },
            { Attributes.Name, (item, attribute) => item.Name = attribute.Value },
            { Attributes.LastModifiedTime, (item, attribute) => item.LastModified = (DateTime)attribute },
            { Attributes.IsUnread, (item, attribute) => item.IsUnread = (bool)attribute },

            { Attributes.Path, (section, attribute) => section.Path = attribute.Value },
            { Attributes.Color, (section, attribute) => section.Color = GetColor(attribute) },
            { Attributes.Encrypted, (section, attribute) => section.Encrypted = (bool)attribute },
            { Attributes.Locked, (section, attribute) => section.Locked = (bool)attribute },
            { Attributes.IsInRecycleBin, (section, attribute) => section.IsInRecycleBin = (bool)attribute },
            { Attributes.IsDeletedPages, (section, attribute) => section.IsDeletedPages = (bool)attribute }
        };

        private static readonly Dictionary<string, Action<OneNoteSectionGroup, XAttribute>> sectionGroupSetters =
            new Dictionary<string, Action<OneNoteSectionGroup, XAttribute>>
        {
            { Attributes.ID, (item, attribute) => item.ID = attribute.Value },
            { Attributes.Name, (item, attribute) => item.Name = attribute.Value },
            { Attributes.LastModifiedTime, (item, attribute) => item.LastModified = (DateTime)attribute },
            { Attributes.IsUnread, (item, attribute) => item.IsUnread = (bool)attribute },

            { Attributes.Path, (sectionGroup, attribute) => sectionGroup.Path = attribute.Value },
            { Attributes.IsRecycleBin, (sectionGroup, attribute) => sectionGroup.IsRecycleBin = (bool)attribute }
        };

        private static readonly Dictionary<string, Action<OneNoteNotebook, XAttribute>> notebookSetters =
            new Dictionary<string, Action<OneNoteNotebook, XAttribute>>
        {
            { Attributes.ID, (item, attribute) => item.ID = attribute.Value },
            { Attributes.Name, (item, attribute) => item.Name = attribute.Value },
            { Attributes.LastModifiedTime, (item, attribute) => item.LastModified = (DateTime)attribute },
            { Attributes.IsUnread, (item, attribute) => item.IsUnread = (bool)attribute },

            { Attributes.NickName, (notebook, attribute) => notebook.NickName = attribute.Value },
            { Attributes.Path, (notebook, attribute) => notebook.Path = attribute.Value },
            { Attributes.Color, (notebook, attribute) => notebook.Color = GetColor(attribute) }
        };

        //Unknown at runtime
        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
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
        {
            if (attribute.Value == "none")
                return null;

            return ColorTranslator.FromHtml(attribute.Value);
        }

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
            notebook.Sections = element.Elements(SectionXName)
                                       .Select(e => ParseSection(e, notebook));
            notebook.SectionGroups = element.Elements(SectionGroupXName)
                                            .Select(e => ParseSectionGroup(e, notebook));
            return notebook;
        }
        public IEnumerable<OneNoteNotebook> ParseNotebooks(string xml) => XElement.Parse(xml)
                                                                                  .Elements(NotebookXName)
                                                                                  .Select(e => ParseNotebook(e, null));
    }
}

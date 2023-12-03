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

        private static readonly Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>> runtimeParser = new Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>>()
        {
            {NotebookXName, ParseNotebook},
            {SectionGroupXName, ParseSectionGroup},
            {SectionXName, ParseSection},
            {PageXName, ParsePage}
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
            //Technically 'faster' than the XElement.GetAttribute method
            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "ID":
                        page.ID = attribute.Value;
                        break;
                    case "name":
                        page.Name = attribute.Value;
                        break;
                    case "dateTime":
                        page.Created = (DateTime)attribute;
                        break;
                    case "lastModifiedTime":
                        page.LastModified = (DateTime)attribute;
                        break;
                    case "pageLevel":
                        page.Level = (int)attribute;
                        break;
                    case "isUnread":
                        page.IsUnread = (bool)attribute;
                        break;
                    case "isInRecycleBin":
                        page.IsInRecycleBin = (bool)attribute;
                        break;
                }
            }
            // if (parent == null)
            // {
            //     page.notebook = new Lazy<OneNoteNotebook>(OneNoteApplication.GetNotebook());
            // }
            // else
            // {
            //     page.Section = (OneNoteSection)parent;
            //     page.Notebook = parent.Notebook;
            //     page.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{page.Name}";
            // }
            page.Section = (OneNoteSection)parent;
            page.Notebook = parent.Notebook;
            page.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{page.Name}";
            return page;
        }

        private static OneNoteSection ParseSection(XElement element, IOneNoteItem parent)
        {
            var section = new OneNoteSection();
            //Technically 'faster' than the XElement.GetAttribute method
            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name":
                        section.Name = attribute.Value;
                        break;
                    case "ID":
                        section.ID = attribute.Value;
                        break;
                    case "path":
                        section.Path = attribute.Value;
                        break;
                    case "isUnread":
                        section.IsUnread = (bool)attribute;
                        break;
                    case "color":
                        if (attribute.Value != "none")
                            section.Color = ColorTranslator.FromHtml(attribute.Value);
                        else
                            section.Color = null;
                        break;
                    case "lastModifiedTime":
                        section.LastModified = (DateTime)attribute;
                        break;
                    case "encrypted":
                        section.Encrypted = (bool)attribute;
                        break;
                    case "locked":
                        section.Locked = (bool)attribute;
                        break;
                    case "isInRecycleBin":
                        section.IsInRecycleBin = (bool)attribute;
                        break;
                    case "isDeletedPages":
                        section.IsDeletedPages = (bool)attribute;
                        break;
                }
            }
            
            section.Parent = parent;
            section.Notebook = parent.Notebook;
            section.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{section.Name}";
            section.Pages = element.Elements(PageXName)
                                   .Select(e => ParsePage(e, section));
            return section;
        }

        private static OneNoteSectionGroup ParseSectionGroup(XElement element, IOneNoteItem parent)
        {
            var sectionGroup = new OneNoteSectionGroup();
            //Technically 'faster' than the XElement.GetAttribute method
            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name":
                        sectionGroup.Name = attribute.Value;
                        break;
                    case "ID":
                        sectionGroup.ID = attribute.Value;
                        break;
                    case "path":
                        sectionGroup.Path = attribute.Value;
                        break;
                    case "lastModifiedTime":
                        sectionGroup.LastModified = (DateTime)attribute;
                        break;
                    case "isUnread":
                        sectionGroup.IsUnread = (bool)attribute;
                        break;
                    case "isRecycleBin":
                        sectionGroup.IsRecycleBin = (bool)attribute;
                        break;
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
            //Technically 'faster' than the XElement.GetAttribute method
            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name":
                        notebook.Name = attribute.Value;
                        break;
                    case "nickname":
                        notebook.NickName = attribute.Value;
                        break;
                    case "ID":
                        notebook.ID = attribute.Value;
                        break;
                    case "path":
                        notebook.Path = attribute.Value;
                        break;
                    case "lastModifiedTime":
                        notebook.LastModified = (DateTime)attribute;
                        break;
                    case "color":
                        if (attribute.Value != "none")
                            notebook.Color = ColorTranslator.FromHtml(attribute.Value);
                        else
                            notebook.Color = null;
                        break;
                    case "isUnread":
                        notebook.IsUnread = (bool)attribute;
                        break;
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

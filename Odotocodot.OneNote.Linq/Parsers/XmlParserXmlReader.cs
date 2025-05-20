using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Odotocodot.OneNote.Linq.Parsers
{
    using static Constants;

    internal class XmlParserXmlReader : IXmlParser
    {
        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    reader.MoveToContent();
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case Elements.Notebook:
                                return ParseNotebook(reader);
                            case Elements.Section:
                                return ParseSection(reader, parent);
                            case Elements.SectionGroup:
                                return ParseSectionGroup(reader, parent);
                            case Elements.Page:
                                return ParsePage(reader, (OneNoteSection)parent);
                            default:
                                return null;
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<OneNoteNotebook> ParseNotebooks(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    reader.MoveToContent();
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.NotebookList)
                    {
                        return ParseNotebooks(reader);
                    }

                    return Array.Empty<OneNoteNotebook>();
                }
            }
        }

        private List<OneNoteNotebook> ParseNotebooks(XmlReader reader)
        {
            var notebooks = new List<OneNoteNotebook>();

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return notebooks;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.Notebook)
                {
                    notebooks.Add(ParseNotebook(reader));
                }
                else
                {
                    reader.Read();
                }
            }

            reader.ReadEndElement();
            return notebooks;
        }
        private OneNoteNotebook ParseNotebook(XmlReader reader)
        {
            var notebook = new OneNoteNotebook();
            // reader.MoveToContent();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        notebook.ID = reader.Value;
                        break;
                    case Attributes.Name:
                        notebook.Name = reader.Value;
                        break;
                    case Attributes.NickName:
                        notebook.NickName = reader.Value;
                        break;
                    case Attributes.Path:
                        notebook.Path = reader.Value;
                        break;
                    case Attributes.Color:
                        notebook.Color = GetColor(reader.Value);
                        break;
                    case Attributes.IsUnread:
                        notebook.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.LastModifiedTime:
                        notebook.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                }
            }

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                notebook.Sections = Array.Empty<OneNoteSection>();
                notebook.SectionGroups = Array.Empty<OneNoteSectionGroup>();
                return notebook;
            }

            var sections = new List<OneNoteSection>();
            var sectionGroups = new List<OneNoteSectionGroup>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == Elements.Section)
                    {
                        sections.Add(ParseSection(reader, notebook));
                    }
                    else if (reader.LocalName == Elements.SectionGroup)
                    {
                        sectionGroups.Add(ParseSectionGroup(reader, notebook));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
            notebook.Sections = sections;
            notebook.SectionGroups = sectionGroups;
            reader.ReadEndElement();
            return notebook;
        }

        private OneNoteSectionGroup ParseSectionGroup(XmlReader reader, IOneNoteItem parent)
        {
            var sectionGroup = new OneNoteSectionGroup();
            sectionGroup.Parent = parent;
            sectionGroup.Notebook = parent.Notebook;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        sectionGroup.ID = reader.Value;
                        break;
                    case Attributes.Name:
                        sectionGroup.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        sectionGroup.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        sectionGroup.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.Path:
                        sectionGroup.Path = reader.Value;
                        break;
                    case Attributes.IsRecycleBin:
                        sectionGroup.IsRecycleBin = bool.Parse(reader.Value);
                        break;
                }
            }

            sectionGroup.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{sectionGroup.Name}";

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                sectionGroup.Sections = Array.Empty<OneNoteSection>();
                sectionGroup.SectionGroups = Array.Empty<OneNoteSectionGroup>();
                return sectionGroup;
            }

            var sections = new List<OneNoteSection>();
            var sectionGroups = new List<OneNoteSectionGroup>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == Elements.Section)
                    {
                        sections.Add(ParseSection(reader, sectionGroup));
                    }
                    else if (reader.LocalName == Elements.SectionGroup)
                    {
                        sectionGroups.Add(ParseSectionGroup(reader, sectionGroup));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
            sectionGroup.Sections = sections;
            sectionGroup.SectionGroups = sectionGroups;
            reader.ReadEndElement();
            return sectionGroup;
        }


        private OneNoteSection ParseSection(XmlReader reader, IOneNoteItem parent)
        {
            var section = new OneNoteSection();
            section.Parent = parent;
            section.Notebook = parent.Notebook;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        section.ID = reader.Value;
                        break;
                    case Attributes.Name:
                        section.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        section.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        section.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.Path:
                        section.Path = reader.Value;
                        break;
                    case Attributes.Color:
                        section.Color = ColorTranslator.FromHtml(reader.Value);
                        break;
                    case Attributes.Encrypted:
                        section.Encrypted = bool.Parse(reader.Value);
                        break;
                    case Attributes.Locked:
                        section.Locked = bool.Parse(reader.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        section.IsInRecycleBin = bool.Parse(reader.Value);
                        break;
                    case Attributes.IsDeletedPages:
                        section.IsDeletedPages = bool.Parse(reader.Value);
                        break;
                }
            }

            section.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{section.Name}";

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                section.Pages = Array.Empty<OneNotePage>();
                return section;
            }

            var pages = new List<OneNotePage>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == Elements.Page)
                    {
                        pages.Add(ParsePage(reader, section));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
            section.Pages = pages;
            reader.ReadEndElement();
            return section;
        }

        private OneNotePage ParsePage(XmlReader reader, OneNoteSection parent)
        {
            var page = new OneNotePage();
            page.Parent = parent;
            page.Notebook = parent.Notebook;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        page.ID = reader.Value;
                        break;
                    case Attributes.Name:
                        page.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        page.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        page.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.DateTime:
                        page.Created = DateTime.Parse(reader.Value);
                        break;
                    case Attributes.PageLevel:
                        page.Level = int.Parse(reader.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        page.IsInRecycleBin = bool.Parse(reader.Value);
                        break;
                }
            }

            page.RelativePath = $"{parent.RelativePath}{RelativePathSeparator}{page.Name}";

            reader.Skip();
            return page;
        }
    }
}
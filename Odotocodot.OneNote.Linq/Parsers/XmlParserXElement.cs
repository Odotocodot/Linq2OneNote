using Odotocodot.OneNote.Linq.Abstractions;
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
        internal static readonly XName NotebookXName = XName.Get(Elements.Notebook, NamespaceUri);
        internal static readonly XName SectionGroupXName = XName.Get(Elements.SectionGroup, NamespaceUri);
        internal static readonly XName SectionXName = XName.Get(Elements.Section, NamespaceUri);
        internal static readonly XName PageXName = XName.Get(Elements.Page, NamespaceUri);

        internal static readonly Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>> runtimeParser =
            new Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>>
        {
            { NotebookXName, (element, _) => ParseNotebook(element)},
            { SectionGroupXName, ParseSectionGroup},
            { SectionXName, ParseSection},
            { PageXName, ParsePage}
        };

        private static void SetAttributes(OneNoteItem item, IEnumerable<XAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name.LocalName)
                {
                    case Attributes.ID:
                        item.ID = attribute.Value;
                        break;
                    case Attributes.Name:
                        item.Name = attribute.Value;
                        break;
                    case Attributes.IsUnread:
                        item.IsUnread = (bool)attribute;
                        break;
                    case Attributes.LastModifiedTime:
                        item.LastModified = (DateTime)attribute;
                        break;
                    case Attributes.Path:
                        ((IWritePath)item).Path = attribute.Value;
                        break;
                    case Attributes.Color:
                        ((IWriteColor)item).Color = GetColor(attribute.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        ((IWriteIsInRecycleBin)item).IsInRecycleBin = bool.Parse(attribute.Value);
                        break;
                    case Attributes.NickName:
                        ((OneNoteNotebook)item).NickName = attribute.Value;
                        break;
                    case Attributes.IsRecycleBin:
                        ((OneNoteSectionGroup)item).IsRecycleBin = (bool)attribute;
                        break;
                    case Attributes.Encrypted:
                        ((OneNoteSection)item).Encrypted = bool.Parse(attribute.Value);
                        break;
                    case Attributes.Locked:
                        ((OneNoteSection)item).Locked = bool.Parse(attribute.Value);
                        break;
                    case Attributes.IsDeletedPages:
                        ((OneNoteSection)item).IsDeletedPages = bool.Parse(attribute.Value);
                        break;
                    case Attributes.PageLevel:
                        ((OneNotePage)item).Level = int.Parse(attribute.Value);
                        break;
                    case Attributes.DateTime:
                        ((OneNotePage)item).Created = DateTime.Parse(attribute.Value);
                        break;
                }
            }
        }
        private static Color? GetColor(in string color)
        {
            if (color == "none")
                return null;

            return ColorTranslator.FromHtml(color);
        }

        public IEnumerable<OneNoteNotebook> ParseNotebooks(string xml) => XElement.Parse(xml)
                                                                                  .Elements(NotebookXName)
                                                                                  .Select(e => ParseNotebook(e));

        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            var root = XElement.Parse(xml);
            return runtimeParser[root.Name](root, parent);
        }


        private static T Parse<T>(T item, XElement element, IOneNoteItem parent) where T : OneNoteItem
        {
            SetAttributes(item, element.Attributes());
            item.Parent = parent;
            item.Notebook = parent?.Notebook;
            item.RelativePath = $"{parent?.RelativePath}{RelativePathSeparator}{item.Name}";
            return item;
        }

        private static T ParseChildren<T>(T item, XElement element) where T : OneNoteItem, IWriteSectionsAndSectionGroups
        {
            item.Sections = element.Elements(SectionXName)
                                   .Select(e => ParseSection(e, item));
            item.SectionGroups = element.Elements(SectionGroupXName)
                                        .Select(e => ParseSectionGroup(e, item));
            return item;
        }

        private static OneNotePage ParsePage(XElement element, IOneNoteItem parent)
            => Parse(new OneNotePage(), element, parent);

        private static OneNoteSection ParseSection(XElement element, IOneNoteItem parent)
        {
            var section = Parse(new OneNoteSection(), element, parent);
            section.Pages = element.Elements(PageXName)
                                   .Select(e => ParsePage(e, section));
            return section;
        }

        private static OneNoteSectionGroup ParseSectionGroup(XElement element, IOneNoteItem parent)
            => ParseChildren(Parse(new OneNoteSectionGroup(), element, parent), element);

        private static OneNoteNotebook ParseNotebook(XElement element)
            => ParseChildren(Parse(new OneNoteNotebook(), element, null), element);
    }
}

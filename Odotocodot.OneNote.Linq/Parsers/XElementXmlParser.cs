using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Odotocodot.OneNote.Linq.Parsers
{
    internal class XElementXmlParser : IXmlParser
    {
        private static readonly XName NotebookXName = XName.Get(XmlParserUtlis.Names.Notebook, XmlParserUtlis.NamespaceUri);
        private static readonly XName SectionGroupXName = XName.Get(XmlParserUtlis.Names.SectionGroup, XmlParserUtlis.NamespaceUri);
        private static readonly XName SectionXName = XName.Get(XmlParserUtlis.Names.Section, XmlParserUtlis.NamespaceUri);
        private static readonly XName PageXName = XName.Get(XmlParserUtlis.Names.Page, XmlParserUtlis.NamespaceUri);

        private static readonly Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>> runtimeParser =
            new Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>>
        {
            { NotebookXName, ParseNotebook},
            { SectionGroupXName, ParseSectionGroup},
            { SectionXName, ParseSection},
            { PageXName, ParsePage}
        };
        
        //Unknown at runtime
        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            var root = XElement.Parse(xml);
            return runtimeParser[root.Name](root, parent);
        }
        
        private static void ParseAttributes<T>(XElement element, T item, Dictionary<string, Action<T, string>> setters)
        {
            foreach (var attribute in element.Attributes())
            {
                if (setters.TryGetValue(attribute.Name.LocalName, out var setter))
                {
                    setter(item, attribute.Value);
                }
            }
        }

        private static OneNotePage ParsePage(XElement element, IOneNoteItem parent)
        {
            var page = new OneNotePage();
            ParseAttributes(element, page, XmlParserUtlis.pageSetters);

            page.Section = (OneNoteSection)parent;
            page.Notebook = parent.Notebook;
            page.RelativePath = $"{parent.RelativePath}{XmlParserUtlis.RelativePathSeparator}{page.Name}";
            return page;
        }

        private static OneNoteSection ParseSection(XElement element, IOneNoteItem parent)
        {
            var section = new OneNoteSection();
            ParseAttributes(element, section, XmlParserUtlis.sectionSetters);

            section.Parent = parent;
            section.Notebook = parent.Notebook;
            section.RelativePath = $"{parent.RelativePath}{XmlParserUtlis.RelativePathSeparator}{section.Name}";
            section.Pages = element.Elements(PageXName)
                                   .Select(e => ParsePage(e, section));
            return section;
        }

        private static OneNoteSectionGroup ParseSectionGroup(XElement element, IOneNoteItem parent)
        {
            var sectionGroup = new OneNoteSectionGroup();
            ParseAttributes(element, sectionGroup, XmlParserUtlis.sectionGroupSetters);
            
            sectionGroup.Notebook = parent.Notebook;
            sectionGroup.Parent = parent;
            sectionGroup.RelativePath = $"{parent.RelativePath}{XmlParserUtlis.RelativePathSeparator}{sectionGroup.Name}";
            sectionGroup.Sections = element.Elements(SectionXName)
                                           .Select(e => ParseSection(e, sectionGroup));
            sectionGroup.SectionGroups = element.Elements(SectionGroupXName)
                                                .Select(e => ParseSectionGroup(e, sectionGroup));
            return sectionGroup;

        }

        private static OneNoteNotebook ParseNotebook(XElement element, IOneNoteItem _)
        {
            var notebook = new OneNoteNotebook();
            ParseAttributes(element, notebook, XmlParserUtlis.notebookSetters);
            
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

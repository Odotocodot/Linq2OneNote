using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

namespace Odotocodot.OneNote.Linq.Parsers
{
    internal class XmlReaderSubTreeXmlParser : IXmlParser
    {
        public IEnumerable<OneNoteNotebook> ParseNotebooks(string xml)
        {
            var notebooks = new List<OneNoteNotebook>();

            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.LocalName == XmlParserUtlis.Names.Notebook)
                        {
                            var notebook = new OneNoteNotebook();
                            XmlReaderXmlParser.ParseAttributes(reader, notebook, XmlParserUtlis.notebookSetters);
                            // {
                            //     Sections = new List<OneNoteSection>(),
                            //     SectionGroups = new List<OneNoteSectionGroup>()
                            // };

                            using (var notebookReader = reader.ReadSubtree())
                            {
                                ParseSectionsAndGroups(notebookReader, notebook);
                            }

                            notebooks.Add(notebook);
                        }
                    }
                }
            }

            return notebooks;
        }

        private void ParseSectionsAndGroups(XmlReader reader, OneNoteNotebook notebook)
        {
            var stack = new Stack<(XmlReader, OneNoteSectionGroup)>();
            stack.Push((reader, null));

            while (stack.Count > 0)
            {
                var (currentReader, parentGroup) = stack.Pop();

                while (currentReader.Read())
                {
                    if (currentReader.NodeType == XmlNodeType.Element)
                    {
                        if (currentReader.Name == "one:Section")
                        {
                            var section = new OneNoteSection();
                            XmlReaderXmlParser.ParseAttributes(currentReader, section, XmlParserUtlis.sectionSetters);
                            //Pages = new List<OneNotePage>()
                            
                            using (var sectionReader = currentReader.ReadSubtree())
                            {
                                while (sectionReader.ReadToFollowing("one:Page"))
                                {
                                    var page = new OneNotePage();
                                    XmlReaderXmlParser.ParseAttributes(sectionReader, page, XmlParserUtlis.pageSetters);
                                    page.Section = section;
                                    section.Pages = section.Pages.Append(page);
                                }
                            }

                            if (parentGroup != null)
                            {
                                parentGroup.Sections = parentGroup.Sections.Append(section);
                            }
                            else
                            {
                                notebook.Sections = notebook.Sections.Append(section);
                            }
                        }
                        else if (currentReader.Name == "one:SectionGroup")
                        {
                            var sectionGroup = new OneNoteSectionGroup();
                            XmlReaderXmlParser.ParseAttributes(currentReader, sectionGroup, XmlParserUtlis.sectionGroupSetters);
                            // Sections = new List<OneNoteSection>(),
                            // SectionGroups = new List<OneNoteSectionGroup>()

                            if (parentGroup != null)
                            {
                                parentGroup.SectionGroups = parentGroup.SectionGroups.Append(sectionGroup);
                            }
                            else
                            {
                                notebook.SectionGroups = notebook.SectionGroups.Append(sectionGroup);
                            }

                            stack.Push((currentReader.ReadSubtree(), sectionGroup));
                        }
                    }
                }
            }
        }

        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            // Implementation for parsing unknown elements
            throw new NotImplementedException();
        }

        private Color? ParseColor(string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                return null;
            }

            return ColorTranslator.FromHtml(color);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

namespace Odotocodot.OneNote.Linq.Parsers
{
    internal class XmlReaderXmlParser : IXmlParser
    {
        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            //TODO: refactor, this is essentially repeated code. Also test.
            IOneNoteItem item = null;
            // using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            // {
            //     using (XmlReader reader = XmlReader.Create(stream))
            //     {
            //         OneNoteNotebook notebook = null;
            //         OneNoteSection section = null;
            //         Stack<OneNoteSectionGroup> stack = new Stack<OneNoteSectionGroup>();
            //         reader.MoveToContent();
            //         while (!reader.EOF)
            //         {
            //             if (reader.NodeType == XmlNodeType.Element)
            //             {
            //                 switch (reader.LocalName)
            //                 {
            //                     case "Notebook":
            //                         notebook = new OneNoteNotebook();
            //                         SetValues(reader, notebook, notebookSetters);
            //                         item = notebook;
            //                         break;
            //                     case "SectionGroup":
            //                         var sectionGroup = new OneNoteSectionGroup();
            //                         if (item == null)
            //                         {
            //                             item = sectionGroup;
            //                             notebook = parent.Notebook;
            //
            //                         }
            //                         SetValues(reader, sectionGroup, XmlParserHelpers.sectionGroupSetters);
            //                         AddToParent(notebook, sectionGroup, stack);
            //
            //                         IOneNoteItem parentSg = stack.TryPeek(out var sg) ? sg : (IOneNoteItem)notebook;
            //
            //
            //                         sectionGroup.Parent = parentSg;
            //                         sectionGroup.Notebook = parentSg.Notebook;
            //                         sectionGroup.RelativePath = $"{parentSg.RelativePath}{XmlParserHelpers.RelativePathSeparator}{sectionGroup.Name}";
            //                         stack.Push(sectionGroup);
            //                         break;
            //                     case "Section":
            //                         section = new OneNoteSection();
            //                         if (item == null)
            //                         {
            //                             item = section;
            //
            //                             notebook = parent.Notebook;
            //                         }
            //                         SetValues(reader, section, sectionSetters);
            //                         AddToParent(notebook, section, stack);
            //
            //                         IOneNoteItem parentS = stack.TryPeek(out var sg1) ? sg1 : (IOneNoteItem)notebook;
            //
            //
            //                         section.Parent = parentS;
            //                         section.Notebook = notebook;
            //                         section.RelativePath = $"{parentS.RelativePath}{XmlParserHelpers.RelativePathSeparator}{section.Name}";
            //                         break;
            //                     case "Page":
            //                         var page = new OneNotePage();
            //                         if (item == null)
            //                         {
            //                             item = page;
            //                             section = (OneNoteSection)parent;
            //                             notebook = parent.Notebook;
            //                         }
            //                         SetValues(reader, page, pageSetters);
            //                         if (section.children == null)
            //                         {
            //                             section.children = new List<IOneNoteItem>();
            //                         }
            //                         section.children.Add(page);
            //
            //
            //                         page.Section = section;
            //                         page.Notebook = notebook;
            //                         page.RelativePath = $"{section.RelativePath}{XmlParserHelpers.RelativePathSeparator}{page.Name}";
            //                         break;
            //                 }
            //             }
            //             else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "SectionGroup")
            //             {
            //                 if (stack.Count > 0)
            //                 {
            //                     stack.Pop();
            //                 }
            //             }
            //             else
            //                 reader.Read();
            //         }
            //     }
            // }
            return item;
        }
        public IEnumerable<OneNoteNotebook> ParseNotebooks(string xml)
        {
            var notebooks = new List<OneNoteNotebook>();
            using (var stream = new StringReader(xml))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    OneNoteNotebook notebook = null;
                    OneNoteSection section = null;
                    Stack<OneNoteSectionGroup> stack = new Stack<OneNoteSectionGroup>(); //TODO: replace with a Stack<List<IOneNoteItem>>?

                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.LocalName)
                            {
                                case XmlParserUtlis.Names.Notebook:
                                    notebook = new OneNoteNotebook();
                                    ParseAttributes(reader, notebook, XmlParserUtlis.notebookSetters);
                                    notebooks.Add(notebook);
                                    stack.Clear();
                                    break;
                                case XmlParserUtlis.Names.SectionGroup:
                                    var sectionGroup = new OneNoteSectionGroup();
                                    ParseAttributes(reader, sectionGroup, XmlParserUtlis.sectionGroupSetters);
                                    AddToParent(notebook, sectionGroup, stack);

                                    IOneNoteItem parentSg = stack.TryPeek(out var sg) ? sg : (IOneNoteItem)notebook;

                                    sectionGroup.Parent = parentSg;
                                    sectionGroup.Notebook = parentSg.Notebook;
                                    sectionGroup.RelativePath = $"{parentSg.RelativePath}{XmlParserUtlis.RelativePathSeparator}{sectionGroup.Name}";
                                    stack.Push(sectionGroup);
                                    break;
                                case XmlParserUtlis.Names.Section:
                                    section = new OneNoteSection();
                                    ParseAttributes(reader, section, XmlParserUtlis.sectionSetters);
                                    AddToParent(notebook, section, stack);

                                    IOneNoteItem parentS = stack.TryPeek(out var sg1) ? sg1 : (IOneNoteItem)notebook;

                                    section.Parent = parentS;
                                    section.Notebook = notebook;
                                    section.RelativePath = $"{parentS.RelativePath}{XmlParserUtlis.RelativePathSeparator}{section.Name}";
                                    break;
                                case XmlParserUtlis.Names.Page:
                                    var page = new OneNotePage();
                                    ParseAttributes(reader, page, XmlParserUtlis.pageSetters);
                                    if (section.children == null)
                                    {
                                        section.children = new List<IOneNoteItem>();
                                    }
                                    section.children.Add(page);

                                    page.Section = section;
                                    page.Notebook = notebook;
                                    page.RelativePath = $"{section.RelativePath}{XmlParserUtlis.RelativePathSeparator}{page.Name}";
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlParserUtlis.Names.SectionGroup)
                        {
                            if (stack.Count > 0)
                            {
                                stack.Pop();
                            }
                        }
                    }
                }
            }
            return notebooks;
        }

        internal static void ParseAttributes<T>(XmlReader reader, T item, Dictionary<string, Action<T, string>> itemSetters)
        {
            while (reader.MoveToNextAttribute()) // Could change to MoveToAttribute and AttributeCount
            {
                if (itemSetters.TryGetValue(reader.Name, out var setter))
                {
                    setter(item, reader.Value);
                }
            }
        }

        private static void AddToParent(OneNoteNotebook notebook, IOneNoteItem item, Stack<OneNoteSectionGroup> stack)
        {
            // TODO: refactor
            if (stack.TryPeek(out var sg))
            {
                if (sg.children == null)
                {
                    sg.children = new List<IOneNoteItem>();
                }
                sg.children.Add(item);
                //item.Parent = sg;
            }
            else
            {
                if (notebook.children == null)
                {
                    notebook.children = new List<IOneNoteItem>();
                }
                notebook.children.Add(item);
            }
        }
    }

    internal static class StackExtensions
    {
        internal static bool TryPeek<T>(this Stack<T> stack, out T item)
        {
            if (stack.Count > 0)
            {
                item = stack.Peek();
                return true;
            }
            item = default;
            return false;
        }
    }
}

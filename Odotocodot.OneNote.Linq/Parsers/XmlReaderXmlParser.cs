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
        private static readonly Dictionary<string, Action<OneNoteNotebook, XmlReader>> notebookSetters =
            new Dictionary<string, Action<OneNoteNotebook, XmlReader>>
        {
            { "ID", (item, reader) => item.ID = reader.Value },
            { "name", (item, reader) => item.Name = reader.Value },
            { "lastModifiedTime", (item, reader) => item.LastModified = DateTime.Parse(reader.Value) },
            { "isUnread", (item, reader) => item.IsUnread = bool.Parse(reader.Value)},

            { "nickname", (notebook, reader) => notebook.NickName = reader.Value },
            { "path", (notebook, reader) => notebook.Path = reader.Value },
            { "color", (notebook, reader) => notebook.Color = ColorTranslator.FromHtml(reader.Value) }
        };

        private static readonly Dictionary<string, Action<OneNoteSectionGroup, XmlReader>> sectionGroupSetters =
            new Dictionary<string, Action<OneNoteSectionGroup, XmlReader>>
        {
            { "ID", (item, reader) => item.ID = reader.Value },
            { "name", (item, reader) => item.Name = reader.Value },
            { "lastModifiedTime", (item, reader) => item.LastModified = DateTime.Parse(reader.Value) },
            { "isUnread", (item, reader) => item.IsUnread = bool.Parse(reader.Value)},

            { "path", (sectionGroup, reader) => sectionGroup.Path = reader.Value },
            { "isRecycleBin", (sectionGroup, reader) => sectionGroup.IsRecycleBin =bool.Parse(reader.Value) }
        };

        private static readonly Dictionary<string, Action<OneNoteSection, XmlReader>> sectionSetters =
            new Dictionary<string, Action<OneNoteSection, XmlReader>>
        {
            { "ID", (item, reader) => item.ID = reader.Value },
            { "name", (item, reader) => item.Name = reader.Value },
            { "lastModifiedTime", (item, reader) => item.LastModified = DateTime.Parse(reader.Value) },
            { "isUnread", (item, reader) => item.IsUnread = bool.Parse(reader.Value)},

            { "path", (section, reader) => section.Path = reader.Value },
            { "color", (section, reader) => section.Color = ColorTranslator.FromHtml(reader.Value) },
            { "encrypted", (section, reader) => section.Encrypted = bool.Parse(reader.Value)},
            { "locked", (section, reader) => section.Locked = bool.Parse(reader.Value) },
            { "isInRecycleBin", (section, reader) => section.IsInRecycleBin = bool.Parse(reader.Value) },
            { "isDeletedPages", (section, reader) => section.IsDeletedPages = bool.Parse(reader.Value) }
        };

        private static readonly Dictionary<string, Action<OneNotePage, XmlReader>> pageSetters =
            new Dictionary<string, Action<OneNotePage, XmlReader>>
        {
            { "ID", (item, reader) => item.ID = reader.Value },
            { "name", (item, reader) => item.Name = reader.Value },
            { "lastModifiedTime", (item, reader) => item.LastModified = DateTime.Parse(reader.Value) },
            { "isUnread", (item, reader) => item.IsUnread = bool.Parse(reader.Value)},

            { "dateTime", (page, reader) => page.Created = DateTime.Parse(reader.Value) },
            { "pageLevel", (page, reader) => page.Level = int.Parse(reader.Value) },
            { "isInRecycleBin", (page, reader) => page.IsInRecycleBin = bool.Parse(reader.Value) }
        };

        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            //TODO: refactor, this is essentially repeated code. Also test.
            IOneNoteItem item = null;
            using (MemoryStream streamReader = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                using (XmlReader reader = XmlReader.Create(streamReader))
                {
                    OneNoteNotebook notebook = null;
                    OneNoteSectionGroup sectionGroup = null;
                    OneNoteSection section = null;
                    Stack<OneNoteSectionGroup> stack = new Stack<OneNoteSectionGroup>();
                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.LocalName)
                            {
                                case "Notebook":
                                    notebook = new OneNoteNotebook();
                                    SetValues(reader, notebook, notebookSetters);
                                    item = notebook;
                                    stack.Clear();
                                    break;
                                case "SectionGroup":
                                    sectionGroup = new OneNoteSectionGroup();
                                    SetValues(reader, sectionGroup, sectionGroupSetters);
                                    AddToParent(notebook, sectionGroup, stack);

                                    IOneNoteItem parentSg = stack.TryPeek(out var sg) ? sg : (IOneNoteItem)notebook;

                                    if (item == null)
                                    {
                                        item = sectionGroup;
                                    }

                                    sectionGroup.Parent = parentSg;
                                    sectionGroup.Notebook = parentSg.Notebook;
                                    sectionGroup.RelativePath = $"{parentSg.RelativePath}{XmlParserHelpers.RelativePathSeparator}{sectionGroup.Name}";
                                    stack.Push(sectionGroup);
                                    break;
                                case "Section":
                                    section = new OneNoteSection();
                                    SetValues(reader, section, sectionSetters);
                                    AddToParent(notebook, section, stack);

                                    IOneNoteItem parentS = stack.TryPeek(out var sg1) ? sg1 : (IOneNoteItem)notebook;

                                    if (item == null)
                                    {
                                        item = section;
                                    }

                                    section.Parent = parentS;
                                    section.Notebook = notebook;
                                    section.RelativePath = $"{parentS.RelativePath}{XmlParserHelpers.RelativePathSeparator}{section.Name}";
                                    break;
                                case "Page":
                                    var page = new OneNotePage();
                                    SetValues(reader, page, pageSetters);
                                    if (section.children == null)
                                    {
                                        section.children = new List<IOneNoteItem>();
                                    }
                                    section.children.Add(page);

                                    if (item == null)
                                    {
                                        item = page;
                                    }

                                    page.Section = section;
                                    page.Notebook = notebook;
                                    page.RelativePath = $"{section.RelativePath}{XmlParserHelpers.RelativePathSeparator}{page.Name}";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "SectionGroup")
                        {
                            if (stack.Count > 0)
                            {
                                stack.Pop();
                            }
                        }
                    }
                }
            }
            return item;
        }
        public IEnumerable<OneNoteNotebook> ParseNotebooks(string xml)
        {
            var notebooks = new List<OneNoteNotebook>();
            using (MemoryStream streamReader = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                using (XmlReader reader = XmlReader.Create(streamReader))
                {
                    OneNoteNotebook notebook = null;
                    OneNoteSectionGroup sectionGroup = null;
                    OneNoteSection section = null;
                    Stack<OneNoteSectionGroup> stack = new Stack<OneNoteSectionGroup>();

                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.LocalName)
                            {
                                case "Notebook":
                                    notebook = new OneNoteNotebook();
                                    SetValues(reader, notebook, notebookSetters);
                                    notebooks.Add(notebook);
                                    stack.Clear();
                                    break;
                                case "SectionGroup":
                                    sectionGroup = new OneNoteSectionGroup();
                                    SetValues(reader, sectionGroup, sectionGroupSetters);
                                    AddToParent(notebook, sectionGroup, stack);

                                    IOneNoteItem parentSg = stack.TryPeek(out var sg) ? sg : (IOneNoteItem)notebook;

                                    sectionGroup.Parent = parentSg;
                                    sectionGroup.Notebook = parentSg.Notebook;
                                    sectionGroup.RelativePath = $"{parentSg.RelativePath}{XmlParserHelpers.RelativePathSeparator}{sectionGroup.Name}";
                                    stack.Push(sectionGroup);
                                    break;
                                case "Section":
                                    section = new OneNoteSection();
                                    SetValues(reader, section, sectionSetters);
                                    AddToParent(notebook, section, stack);

                                    IOneNoteItem parentS = stack.TryPeek(out var sg1) ? sg1 : (IOneNoteItem)notebook;

                                    section.Parent = parentS;
                                    section.Notebook = notebook;
                                    section.RelativePath = $"{parentS.RelativePath}{XmlParserHelpers.RelativePathSeparator}{section.Name}";
                                    break;
                                case "Page":
                                    var page = new OneNotePage();
                                    SetValues(reader, page, pageSetters);
                                    if (section.children == null)
                                    {
                                        section.children = new List<IOneNoteItem>();
                                    }
                                    section.children.Add(page);

                                    page.Section = section;
                                    page.Notebook = notebook;
                                    page.RelativePath = $"{section.RelativePath}{XmlParserHelpers.RelativePathSeparator}{page.Name}";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "SectionGroup")
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

        private static void SetValues<T>(XmlReader reader, T item, Dictionary<string, Action<T, XmlReader>> itemSetters)
        {
            while (reader.MoveToNextAttribute()) // Could change to MoveToAttribute and AttributeCount
            {
                if (itemSetters.TryGetValue(reader.Name, out var setter))
                {
                    setter(item, reader);
                }
            }
        }

        private static void AddToParent(OneNoteNotebook notebook, IOneNoteItem item, Stack<OneNoteSectionGroup> stack)
        {
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

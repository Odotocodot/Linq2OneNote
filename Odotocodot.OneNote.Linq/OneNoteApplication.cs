using Microsoft.Office.Interop.OneNote;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using Odotocodot.OneNote.Linq.Parsers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// A static wrapper class around the <see cref="Application"/> class, allowing for <see cref="Lazy{T}">lazy</see> acquirement and
    /// release of a OneNote COM object. In addition to exposing the
    /// <a href="https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote"> OneNote's API</a>
    /// </summary>
    /// <remarks>A <see cref="Application">OneNote COM object</see> is required to access any of the OneNote API.</remarks>
    public static class OneNoteApplication
    {
        #region COM Object Members

        private static Lazy<Application> lazyOneNote = GetLazyOneNote();
        private static Application OneNote => lazyOneNote.Value;

        /// <summary>
        /// Use this only if you know what you are doing.
        /// The COM Object instance of the OneNote application.
        /// </summary>
        /// <seealso cref="HasComObject"/>
        /// <seealso cref="InitComObject"/>
        /// <seealso cref="ReleaseComObject"/>
        public static Application ComObject => OneNote;

        /// <summary>
        /// Indicates whether the class has a usable <see cref="Application">COM Object instance</see>.
        /// </summary>
        /// <remarks>When <see langword="true"/> a "Microsoft OneNote" process should be visible in the Task Manager.</remarks>
        /// <seealso cref="InitComObject"/>
        /// <seealso cref="ReleaseComObject"/>
        public static bool HasComObject => lazyOneNote.IsValueCreated;
        #endregion

        #region Name Validator Members

        /// <summary>
        /// An array containing the characters that are not allowed in a <see cref="OneNoteNotebook">notebook</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # .</b>
        /// </summary>
        /// <seealso cref="IsNotebookNameValid(string)"/>
        /// <seealso cref="InvalidSectionChars"/>
        /// <seealso cref="InvalidSectionGroupChars"/>
        public static readonly ImmutableArray<char> InvalidNotebookChars = @"\/*?""|<>:%#.".ToImmutableArray();

        /// <summary>
        /// An array containing the characters that are not allowed in a <see cref="OneNoteSection">section</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # &amp;</b>
        /// </summary>
        /// <seealso cref="IsSectionNameValid(string)"/>
        /// <seealso cref="InvalidNotebookChars"/>
        /// <seealso cref="InvalidSectionGroupChars"/>
        public static readonly ImmutableArray<char> InvalidSectionChars = @"\/*?""|<>:%#&".ToImmutableArray();

        /// <summary>
        /// An array containing the characters that are not allowed in a <see cref="OneNoteSectionGroup">section group</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # &amp;</b>
        /// </summary>
        /// <seealso cref="IsSectionGroupNameValid(string)"/>
        /// <seealso cref="InvalidNotebookChars"/>
        /// <seealso cref="InvalidSectionChars"/>
        public static readonly ImmutableArray<char> InvalidSectionGroupChars = InvalidSectionChars;
        #endregion

        /// <summary>
        /// The directory separator used in <see cref="IOneNoteItem.RelativePath"/>.
        /// </summary>
        public const char RelativePathSeparator = Constants.RelativePathSeparator;

        // You never know they might add a new one... (Press X to doubt)
        // As the docs say:
        // NOTE: We recommend specifying a version of OneNote (such as xs2013) instead of using xsCurrent or leaving it blank, because this will allow your add-in to work with future versions of OneNote.
        private const XMLSchema xmlSchema = XMLSchema.xs2013;

        private static readonly XmlParserXElement xmlParser = new XmlParserXElement();

        #region COM Object Methods

        private static Lazy<Application> GetLazyOneNote() => new Lazy<Application>(() => new Application(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Forcible initialises the static class by acquiring a <see cref="Application">OneNote COM object</see>. Does nothing if a COM object is already accessible.
        /// </summary>
        /// <exception cref="COMException">Thrown if an error occurred when trying to get the 
        /// <see cref="Application">OneNote COM object</see>.</exception>
        /// <seealso cref="HasComObject"/>
        /// <seealso cref="ReleaseComObject"/>
        public static void InitComObject()
        {
            if (!lazyOneNote.IsValueCreated)
            {
                _ = OneNote;
            }
        }


        private static readonly object lockObj = new object();
        /// <summary>
        /// Releases the <see cref="Application">OneNote COM object</see> freeing memory.
        /// </summary>
        /// <seealso cref="InitComObject"/>
        /// <seealso cref="HasComObject"/>
        public static void ReleaseComObject()
        {
            lock (lockObj)
            {
                if (HasComObject)
                {
                    Marshal.ReleaseComObject(OneNote);
                    lazyOneNote = GetLazyOneNote();
                }
            }
        }

        #endregion

        #region OneNote API Methods

        /// <summary>
        /// Get all notebooks down to all children.
        /// </summary>
        /// <returns>The full hierarchy node structure with <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="OneNoteNotebook"/>&gt; as the root.</returns>
        public static IEnumerable<OneNoteNotebook> GetNotebooks()
        {
            OneNote.GetHierarchy(null, HierarchyScope.hsPages, out string xml, xmlSchema);
            return xmlParser.ParseNotebooks(xml);
        }

        /// <summary>
        /// Get a flattened collection of <see cref="OneNotePage">pages</see> that match the <paramref name="search"/> parameter.
        /// </summary>
        /// <param name="search">The search query. This should be exactly the same string that you would type into the search box in the OneNote UI. You can use bitwise operators, such as AND and OR, which must be all uppercase.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="OneNotePage"/>&gt; that contains <see cref="OneNotePage">pages</see> that match the <paramref name="search"/> parameter.</returns>
        /// <inheritdoc cref="ValidateSearch(string)" path="/exception"/>
        /// <seealso cref="FindPages(string, IOneNoteItem)"/>
        public static IEnumerable<OneNotePage> FindPages(string search)
        {
            ValidateSearch(search);

            OneNote.FindPages(null, search, out string xml, xsSchema: xmlSchema);
            return xmlParser.ParseNotebooks(xml).GetPages();
        }

        /// <summary>
        /// <inheritdoc cref="FindPages(string)" path="/summary"/> Within the specified <paramref name="scope"/>.
        /// </summary>
        /// <param name="search"><inheritdoc cref="FindPages(string)" path="/param[@name='search']"/></param>
        /// <param name="scope">The hierarchy item to search within.</param>
        /// <returns><inheritdoc cref="FindPages(string)" path="/returns"/></returns>
        /// <seealso cref="FindPages(string)"/>
        /// <exception cref="ArgumentException"><inheritdoc cref="ValidateSearch(string)" path="/exception[@cref='ArgumentException']"/></exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="search"/> or <paramref name="scope"/> is <see langword="null"/>.</exception>
        public static IEnumerable<OneNotePage> FindPages(string search, IOneNoteItem scope)
        {
            if (scope is null)
                throw new ArgumentNullException(nameof(scope));

            ValidateSearch(search);

            OneNote.FindPages(scope.ID, search, out string xml, xsSchema: xmlSchema);

            return xmlParser.ParseUnknown(xml, scope).GetPages();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="search"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="search"/> is empty or only whitespace, or if the first character of <paramref name="search"/> is NOT a letter or a digit.</exception>
        private static void ValidateSearch(string search)
        {
            if (search is null)
                throw new ArgumentNullException(nameof(search));

            if (string.IsNullOrWhiteSpace(search))
                throw new ArgumentException("Search string cannot be empty or only whitespace", nameof(search));

            if (!char.IsLetterOrDigit(search[0]))
                throw new ArgumentException("The first character of the search must be a letter or a digit", nameof(search));
        }

        /// <summary>
        /// Opens the <paramref name="item"/> in OneNote (creates a new OneNote window if one is not currently open).
        /// </summary>       
        /// <param name="item">The OneNote hierarchy item.</param>
        public static void OpenInOneNote(IOneNoteItem item) => OneNote.NavigateTo(item.ID);

        /// <summary>
        /// Forces OneNote to sync the <paramref name="item"/>.
        /// </summary>       
        /// <param name="item"><inheritdoc cref="OpenInOneNote(IOneNoteItem)" path="/param[@name='item']"/></param>
        public static void SyncItem(IOneNoteItem item) => OneNote.SyncHierarchy(item.ID);

        /// <summary>
        /// Gets the content of the specified <paramref name="page"/>.
        /// </summary>       
        /// <param name="page">The page to retrieve content from.</param>
        /// <returns>An <see langword="string"/> in the OneNote XML format.</returns>
        public static string GetPageContent(OneNotePage page)
        {
            OneNote.GetPageContent(page.ID, out string xml, xsSchema: xmlSchema);
            return xml;
        }

        /// <summary>
        /// Updates the content of a OneNote page with the provided <paramref name="xml"/>. 
        /// The chosen page depends on the ID provided in the <paramref name="xml"/>. 
        /// An example can be seen <a href="https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote#updatepagecontent-method">here</a> at the Microsoft OneNote API documentation.
        /// </summary>
        /// <remarks>The <paramref name="xml"/> must match the OneNote XML format, the schema can be
        /// found <a href="https://github.com/idvorkin/onom/blob/eb9ce52764e9ad639b2c9b4bca0622ee6221106f/OneNoteObjectModel/onenote.xsd">here</a>.</remarks>
        /// <param name="xml">An <see langword="string"/> in the OneNote XML format. </param>
        public static void UpdatePageContent(string xml) => OneNote.UpdatePageContent(xml, xsSchema: xmlSchema);

        #region Experimental API Methods

        /// <summary>
        /// Deletes the hierarchy <paramref name="item"/> from the OneNote notebook hierarchy.
        /// </summary>
        /// <param name="item"><inheritdoc cref="OpenInOneNote(IOneNoteItem)" path="/param[@name='item']"/></param>
        internal static void DeleteItem(IOneNoteItem item) => OneNote.DeleteHierarchy(item.ID);

        /// <summary>
        /// Closes the <paramref name="notebook"/>.
        /// </summary>
        /// <param name="notebook">The specified OneNote notebook.</param>
        internal static void CloseNotebook(OneNoteNotebook notebook) => OneNote.CloseNotebook(notebook.ID);

        //TODO: Works but UpdateHierarchy takes A LONG TIME!
        internal static void RenameItem(IOneNoteItem item, string newName)
        {
            if (item.IsInRecycleBin())
            {
                throw new ArgumentException("Cannot rename unique items, such as recycle bin.");
            }
            OneNote.GetHierarchy(null, HierarchyScope.hsPages, out string xml);
            var doc = XDocument.Parse(xml);
            var element = doc.Descendants()
                             .FirstOrDefault(e => (string)e.Attribute("ID") == item.ID);

            if (element == null)
                return;

            element.Attribute("name").SetValue(newName);
            OneNote.UpdateHierarchy(doc.ToString());
            switch (item)
            {
                case OneNoteNotebook nb:
                    nb.Name = newName;
                    break;
                case OneNoteSectionGroup sg:
                    sg.Name = newName;
                    break;
                case OneNoteSection s:
                    s.Name = newName;
                    break;
                case OneNotePage p:
                    p.Name = newName;
                    break;
            }
        }
        #endregion

        #region Creating New OneNote Items Methods

        /// <summary>
        /// Creates a <see cref="OneNotePage">page</see> with a title equal to <paramref name="name"/> located in the specified <paramref name="section"/>.<br/>
        /// If <paramref name="section"/> is <see langword="null"/>, this method creates a page in the default quick notes location.
        /// </summary>        
        /// <param name="section">The section to create the page in.</param>
        /// <param name="name">The title of the page.</param>
        /// <param name="open">Whether to open the newly created page in OneNote immediately.</param>
        /// <returns>The <see cref="OneNoteItem.ID"/> of the newly created page.</returns>
        public static string CreatePage(OneNoteSection section, string name, bool open)
        {
            string sectionID;
            if (section != null)
            {
                sectionID = section.ID;
            }
            else
            {
                var path = GetUnfiledNotesSection();
                OneNote.OpenHierarchy(path, null, out sectionID, CreateFileType.cftNone);
            }

            OneNote.SyncHierarchy(sectionID);
            OneNote.CreateNewPage(sectionID, out string pageID, NewPageStyle.npsBlankPageWithTitle);
            OneNote.GetPageContent(pageID, out string xml, PageInfo.piBasic, xmlSchema);
            XDocument doc = XDocument.Parse(xml);

            XNamespace one = XNamespace.Get(Constants.NamespaceUri);

            XElement xTitle = doc.Descendants(one + "T").First();
            xTitle.Value = name;

            OneNote.UpdatePageContent(doc.ToString());

            if (open)
                OneNote.NavigateTo(pageID);

            return pageID;
        }

        /// <summary>
        /// Creates a quick note page located at the users quick notes location.
        /// </summary>       
        /// <param name="open"><inheritdoc cref="CreatePage(OneNoteSection, string, bool)" path="/param[@name='open']"/></param>
        /// <returns>The <see cref="OneNoteItem.ID"/> of the newly created quick note page.</returns>
        public static string CreateQuickNote(bool open)
        {
            var path = GetUnfiledNotesSection();
            OneNote.OpenHierarchy(path, null, out string sectionID, CreateFileType.cftNone);
            OneNote.SyncHierarchy(sectionID);
            OneNote.CreateNewPage(sectionID, out string pageID, NewPageStyle.npsDefault);

            if (open)
                OneNote.NavigateTo(pageID);

            return pageID;
        }

        /// <summary>
        /// Creates a quick note page with the title specified by <paramref name="name"/>, located at the users quick notes location.
        /// </summary>
        /// <remarks>This is identical to calling <see cref="CreatePage(OneNoteSection, string, bool)"/> with the
        /// section parameter set to null</remarks>
        /// <param name="name"><inheritdoc cref="CreatePage(OneNoteSection, string, bool)" path="/param[@name='name']"/></param>
        /// <param name="open"><inheritdoc cref="CreatePage(OneNoteSection, string, bool)" path="/param[@name='open']"/></param>
        /// <returns>The <see cref="OneNoteItem.ID"/> of the newly created quick note page.</returns>
        public static string CreateQuickNote(string name, bool open) => CreatePage(null, name, open);

        private static string CreateItem(
            IOneNoteItem parent,
            string name,
            bool open,
            string path,
            CreateFileType createFileType,
            Func<string, bool> nameValidator,
            string type,
            in ImmutableArray<char> invalidChars)
        {
            if (!nameValidator(name))
                throw new ArgumentException($"Invalid {type.ToLower()} name provided: \"{name}\". {type} names cannot empty, only whitespace or contain the symbols: \t {string.Join(" ", invalidChars)}");

            OneNote.OpenHierarchy(path, parent?.ID, out string newItemID, createFileType);
            if (open)
                OneNote.NavigateTo(newItemID);

            return newItemID;
        }

        /// <summary>
        /// Creates a <see cref="OneNoteSection">section</see> with a title equal to <paramref name="name"/> located in the specified <paramref name="parent"/>.
        /// </summary>        
        /// <param name="parent">The hierarchy item to create the section in.</param>
        /// <param name="name">The name of the new section.</param>
        /// <param name="open">Whether to open the newly created section in OneNote immediately.</param>
        /// <typeparam name="TNotebookOrSectionGroup">Represents a <see cref="OneNoteNotebook">notebook</see> or a <see cref="OneNoteSectionGroup">section group</see>.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not a valid section name.</exception>
        /// <seealso cref="IsSectionNameValid(string)"/>
        /// <returns>The <see cref="OneNoteItem.ID"/> of the newly created section.</returns>
        public static string CreateSection<TNotebookOrSectionGroup>(TNotebookOrSectionGroup parent, string name, bool open) where TNotebookOrSectionGroup : INotebookOrSectionGroup
            => CreateItem(parent, name, open, $"{name}.one", CreateFileType.cftSection, IsSectionNameValid, Constants.Elements.Section, in InvalidSectionChars);

        /// <summary>
        /// Creates a <see cref="OneNoteSectionGroup">section group</see> with a title equal to <paramref name="name"/> located in the specified <paramref name="parent"/>.
        /// </summary>        
        /// <param name="parent">The hierarchy item to create the section group in.</param>
        /// <param name="name">The name of the new section group.</param>
        /// <param name="open">Whether to open the newly created section group in OneNote immediately.</param>
        /// <typeparam name="TNotebookOrSectionGroup">Represents a <see cref="OneNoteNotebook">notebook</see> or a <see cref="OneNoteSectionGroup">section group</see>.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not a valid section group name.</exception>
        /// <seealso cref="IsSectionGroupNameValid(string)"/>
        /// <returns>The <see cref="OneNoteItem.ID"/> of the newly created section group.</returns>
        public static string CreateSectionGroup<TNotebookOrSectionGroup>(TNotebookOrSectionGroup parent, string name, bool open) where TNotebookOrSectionGroup : INotebookOrSectionGroup
            => CreateItem(parent, name, open, name, CreateFileType.cftFolder, IsSectionGroupNameValid, Constants.Elements.SectionGroup, in InvalidSectionGroupChars);

        /// <summary>
        /// Creates a <see cref="OneNoteNotebook">notebook</see> with a title equal to <paramref name="name"/> located in the <see cref="GetDefaultNotebookLocation()">default notebook location</see>.
        /// </summary>        
        /// <param name="name">The name of the new notebook.</param>
        /// <param name="open">Whether to open the newly created notebook in OneNote immediately.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not a valid notebook name.</exception>
        /// <seealso cref="IsNotebookNameValid(string)"/>
        /// <returns>The <see cref="OneNoteItem.ID"/> of the newly created notebook.</returns>
        public static string CreateNotebook(string name, bool open)
            => CreateItem(null, name, open, System.IO.Path.Combine(GetDefaultNotebookLocation(), name), CreateFileType.cftNotebook, IsNotebookNameValid, Constants.Elements.Notebook, in InvalidNotebookChars);

        #endregion

        #region Special Folder Locations

        /// <summary>
        /// Retrieves the path on disk to the default notebook folder location, this is where new notebooks are created and saved to.
        /// </summary>        
        /// <returns>The path to the default notebook folder location.</returns>
        public static string GetDefaultNotebookLocation()
        {
            OneNote.GetSpecialLocation(SpecialLocation.slDefaultNotebookFolder, out string path);
            return path;
        }
        /// <summary>
        /// Retrieves the path on disk to the backup folder location.
        /// </summary>        
        /// <returns>The path on disk to the backup folder location.</returns>
        public static string GetBackUpLocation()
        {
            OneNote.GetSpecialLocation(SpecialLocation.slBackUpFolder, out string path);
            return path;
        }
        /// <summary>
        /// Retrieves the folder path on disk to the unfiled notes section, this is also where quick notes are created and saved to.
        /// </summary>   
        /// <returns>The folder path on disk to the unfiled notes section.</returns>
        public static string GetUnfiledNotesSection()
        {
            OneNote.GetSpecialLocation(SpecialLocation.slUnfiledNotesSection, out string path);
            return path;
        }

        #endregion

        #endregion

        #region Name Validator Methods 

        /// <summary>
        /// Returns a value that indicates whether the supplied <paramref name="name"/> is a valid for a notebook.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see langword="true"/> if the specified <paramref name="name"/> is not null, empty, whitespace or contains any characters from <see cref="InvalidNotebookChars"/>; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="InvalidNotebookChars"/>
        public static bool IsNotebookNameValid(string name)
            => !string.IsNullOrWhiteSpace(name) && !InvalidNotebookChars.Any(name.Contains);

        /// <summary>
        /// Returns a value that indicates whether the supplied <paramref name="name"/> is a valid for a section.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see langword="true"/> if the specified <paramref name="name"/> is not null, empty, whitespace or contains any characters from <see cref="InvalidSectionChars"/>; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="InvalidSectionChars"/>
        public static bool IsSectionNameValid(string name)
            => !string.IsNullOrWhiteSpace(name) && !InvalidSectionChars.Any(name.Contains);

        /// <summary>
        /// Returns a value that indicates whether the supplied <paramref name="name"/> is a valid for a section group.
        /// </summary>
        /// <returns><see langword="true"/> if the specified <paramref name="name"/> is not null, empty, whitespace or contains any characters from <see cref="InvalidSectionGroupChars"/>; otherwise, <see langword="false"/>.</returns>
        /// <param name="name"></param>
        /// <seealso cref="InvalidSectionGroupChars"/>
        public static bool IsSectionGroupNameValid(string name)
            => !string.IsNullOrWhiteSpace(name) && !InvalidSectionGroupChars.Any(name.Contains);

        #endregion
    }
}

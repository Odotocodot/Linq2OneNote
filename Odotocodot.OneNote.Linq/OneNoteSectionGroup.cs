using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a section group in OneNote.
    /// </summary>
    public class OneNoteSectionGroup : OneNoteItem, IOneNoteItem, INotebookOrSectionGroup, IWritableHasPath
    {
        internal OneNoteSectionGroup() { }

        /// <summary>
        /// The full path to the section group.
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Indicates whether this is a special section group which contains all the recently deleted sections as well as the "Deleted Pages" section (see <see cref="OneNoteSection.IsDeletedPages"/>).
        /// </summary>
        /// <seealso cref="OneNoteSection.IsInRecycleBin"/>
        /// <seealso cref="OneNoteSection.IsDeletedPages"/>
        /// <seealso cref="OneNotePage.IsInRecycleBin"/>
        public bool IsRecycleBin { get; internal set; }

        /// <summary>
        /// The sections that this section group contains (direct children only). 
        /// </summary>
        public IEnumerable<OneNoteSection> Sections => Children.OfType<OneNoteSection>();
        /// <summary>
        /// The section groups that this section group contains (direct children only).
        /// </summary>
        public IEnumerable<OneNoteSectionGroup> SectionGroups => Children.OfType<OneNoteSectionGroup>();

        string IWritableHasPath.Path { set => Path = value; }
    }
}

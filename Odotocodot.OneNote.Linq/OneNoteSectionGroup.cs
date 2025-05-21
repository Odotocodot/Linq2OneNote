using Odotocodot.OneNote.Linq.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a section group in OneNote.
    /// </summary>
    public class OneNoteSectionGroup : OneNoteItem, IWritableNotebookOrSectionGroup, IWritableHasPath
    {
        internal OneNoteSectionGroup() { }

        /// <summary>
        /// The direct children of this section group. <br/>
        /// Equivalent to concatenating <see cref="SectionGroups"/> and <see cref="Sections"/>.
        /// </summary>
        public override IEnumerable<IOneNoteItem> Children => ((IEnumerable<IOneNoteItem>)Sections).Concat(SectionGroups);
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
        public IEnumerable<OneNoteSection> Sections { get; internal set; }
        /// <summary>
        /// The section groups that this section group contains (direct children only).
        /// </summary>
        public IEnumerable<OneNoteSectionGroup> SectionGroups { get; internal set; }

        string IWritableHasPath.Path { set => Path = value; }
        IEnumerable<OneNoteSection> IWritableNotebookOrSectionGroup.Sections { set => Sections = value; }
        IEnumerable<OneNoteSectionGroup> IWritableNotebookOrSectionGroup.SectionGroups { set => SectionGroups = value; }
    }
}

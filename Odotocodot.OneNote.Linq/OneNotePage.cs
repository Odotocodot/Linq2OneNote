using Odotocodot.OneNote.Linq.Internal;
using System;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a page in OneNote.
    /// </summary>
    public class OneNotePage : OneNoteItem, IOneNoteItem, IWritableHasIsInRecycleBin
    {
        internal OneNotePage() { }

        /// <summary>
        /// The section that owns this page.
        /// </summary>
        public OneNoteSection Section => (OneNoteSection)Parent;
        /// <summary>
        /// The page level.
        /// </summary>
        public int Level { get; internal set; }
        /// <summary>
        /// The time when the page was created.
        /// </summary>
        public DateTime Created { get; internal set; }
        /// <summary>
        /// Indicates whether the page is in the recycle bin.
        /// </summary>
        /// <seealso cref="OneNoteSectionGroup.IsRecycleBin"/>
        /// <seealso cref="OneNoteSection.IsInRecycleBin"/>
        /// <seealso cref="OneNoteSection.IsDeletedPages"/>
        public bool IsInRecycleBin { get; internal set; }

        bool IWritableHasIsInRecycleBin.IsInRecycleBin { set => IsInRecycleBin = value; }
    }
}
using Odotocodot.OneNote.Linq.Internal;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a section in OneNote.
    /// </summary>
    public class OneNoteSection : OneNoteItem, IOneNoteItem, IWritableHasPath, IWritableHasIsInRecycleBin, IWritableHasColor
    {
        internal OneNoteSection() { }

        /// <summary>
        /// The full path to the section.
        /// </summary>
        public string Path { get; internal set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public bool IsReadOnly { get; internal set; }
        /// <summary>
        /// Indicates whether an encrypted section has been unlocked allowing access, otherwise <see langword="false"/>. <br/>
        /// </summary>
        /// <seealso cref="Encrypted"/>
        public bool Locked { get; internal set; }
        /// <summary>
        /// Indicates whether the section is encrypted.
        /// </summary>
        /// <seealso cref="Locked"/>
        public bool Encrypted { get; internal set; }
        /// <summary>
        /// Indicates whether the section is in recycle bin.
        /// </summary>
        /// <seealso cref="IsDeletedPages"/>
        /// <seealso cref="OneNoteSectionGroup.IsRecycleBin"/>
        /// <seealso cref="OneNotePage.IsInRecycleBin"/>
        public bool IsInRecycleBin { get; internal set; }
        /// <summary>
        /// Indicates whether this section is a special section that contains all the recently deleted pages in this section's notebook.
        /// </summary>
        /// <seealso cref="IsInRecycleBin"/>
        /// <seealso cref="OneNoteSectionGroup.IsRecycleBin"/>
        /// <seealso cref="OneNotePage.IsInRecycleBin"/>
        public bool IsDeletedPages { get; internal set; }
        /// <summary>
        /// The color of the section.
        /// </summary>
        public Color? Color { get; internal set; }
        /// <summary>
        /// The collection of pages within this section, equal to <see cref="IOneNoteItem.Children"/> for a section.
        /// </summary>
        public IEnumerable<OneNotePage> Pages => Children.Cast<OneNotePage>();

        Color? IWritableHasColor.Color { set => Color = value; }
        string IWritableHasPath.Path { set => Path = value; }
        bool IWritableHasIsInRecycleBin.IsInRecycleBin { set => IsInRecycleBin = value; }
    }
}
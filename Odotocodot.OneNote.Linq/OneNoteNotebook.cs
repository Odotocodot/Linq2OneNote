using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a notebook in OneNote.
    /// </summary>
    public class OneNoteNotebook : OneNoteItem, IOneNoteItem, IWritableHasPath, INotebookOrSectionGroup, IWritableHasColor
    {
        internal OneNoteNotebook() { }

        /// <inheritdoc/>
        public override IOneNoteItem Parent { get => null; internal set { } }
        /// <inheritdoc/>
        public override string RelativePath { get => Name; internal set { } }
        /// <inheritdoc/>
        public override OneNoteNotebook Notebook { get => this; internal set { } }

        /// <summary>
        /// The nickname of the notebook.
        /// </summary>
        public string NickName { get; internal set; }
        /// <summary>
        /// The full path to the notebook.
        /// </summary>
        public string Path { get; internal set; }
        /// <summary>
        /// The color of the notebook.
        /// </summary>
        public Color? Color { get; internal set; }
        /// <summary>
        /// The sections that this notebook contains (direct children only). 
        /// </summary>
        public IEnumerable<OneNoteSection> Sections => Children.OfType<OneNoteSection>();
        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IEnumerable<OneNoteSectionGroup> SectionGroups => Children.OfType<OneNoteSectionGroup>();

        Color? IWritableHasColor.Color { set => Color = value; }
        string IWritableHasPath.Path { set => Path = value; }
    }
}
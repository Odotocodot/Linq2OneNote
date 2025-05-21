using Odotocodot.OneNote.Linq.Abstractions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a notebook in OneNote.
    /// </summary>
    public class OneNoteNotebook : OneNoteItem, IWritableHasPath, IWritableNotebookOrSectionGroup, IWritableHasColor
    {
        internal OneNoteNotebook() { }


        /// <inheritdoc/>
        public override IOneNoteItem Parent { get => null; internal set { } }
        /// <inheritdoc/>
        public override string RelativePath { get => Name; internal set { } }
        /// <inheritdoc/>
        public override OneNoteNotebook Notebook { get => this; internal set { } }
        /// <summary>
        /// The direct children of this notebook. <br/>
        /// Equivalent to concatenating <see cref="SectionGroups"/> and <see cref="Sections"/>.
        /// </summary>
        public override IEnumerable<IOneNoteItem> Children => ((IEnumerable<IOneNoteItem>)Sections).Concat(SectionGroups);

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
        public IEnumerable<OneNoteSection> Sections { get; internal set; }
        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IEnumerable<OneNoteSectionGroup> SectionGroups { get; internal set; }

        Color? IWritableHasColor.Color { set => Color = value; }
        string IWritableHasPath.Path { set => Path = value; }
        IEnumerable<OneNoteSection> IWritableNotebookOrSectionGroup.Sections { set => Sections = value; }
        IEnumerable<OneNoteSectionGroup> IWritableNotebookOrSectionGroup.SectionGroups { set => SectionGroups = value; }
    }
}
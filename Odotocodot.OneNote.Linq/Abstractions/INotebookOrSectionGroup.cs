using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents a OneNote hierarchy item that can have <see cref="OneNoteSection">sections</see> and/or <see cref="OneNoteSectionGroup">section groups</see> as children, i.e. a <see cref="OneNoteNotebook">notebook</see> or a <see cref="OneNoteSectionGroup">section group</see>.
	/// </summary>
	/// <seealso cref="OneNoteNotebook"/>
	/// <seealso cref="OneNoteSectionGroup"/>
	public interface INotebookOrSectionGroup : IOneNoteItem
	{
		/// <summary>
		/// The sections that this item contains (direct children only). 
		/// </summary>
		IEnumerable<OneNoteSection> Sections { get; }

		/// <summary>
		/// The section groups that this item contains (direct children only).
		/// </summary>
		IEnumerable<OneNoteSectionGroup> SectionGroups { get; }
	}
}
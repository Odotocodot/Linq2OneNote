using System.Collections.Generic;
using System.Drawing;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents a OneNote hierarchy item that can have <see cref="OneNoteSection">sections</see> and/or <see cref="OneNoteSectionGroup">section groups</see> as children.
	/// </summary>
	/// <seealso cref="OneNoteNotebook"/>
	/// <seealso cref="OneNoteSectionGroup"/>
	public interface IParentOfSectionsAndSectionGroups : IOneNoteItem
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

	internal interface IWriteParentOfSectionsAndSectionGroups : IParentOfSectionsAndSectionGroups
	{
		IEnumerable<OneNoteSection> Sections { set; }
		IEnumerable<OneNoteSectionGroup> SectionGroups { set; }
	}

	public interface IHasPath : IOneNoteItem
	{
		string Path { get; }
	}

	internal interface IWritePath : IHasPath
	{
		string Path { set; }
	}

	public interface IHasColor : IOneNoteItem
	{
		Color? Color { get; }
	}

	internal interface IWriteColor : IHasColor
	{
		Color? Color { set; }
	}

	public interface IHasIsInRecycleBin : IOneNoteItem
	{
		bool IsInRecycleBin { get; }
	}

	internal interface IWriteIsInRecycleBin : IHasIsInRecycleBin
	{
		bool IsInRecycleBin { set; }
	}
}
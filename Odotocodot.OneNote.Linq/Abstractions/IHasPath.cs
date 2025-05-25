namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents an OneNote hierarchy item that has a file path.
	/// </summary>
	/// <seealso cref="OneNoteNotebook"/>
	/// <seealso cref="OneNoteSectionGroup"/>
	/// <seealso cref="OneNoteSection"/>
	public interface IHasPath : IOneNoteItem
	{
		/// <summary>
		/// The full path to the item.
		/// </summary>
		string Path { get; }
	}
}
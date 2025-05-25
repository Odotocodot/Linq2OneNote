namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents an OneNote hierarchy item that can be <b>in</b> a OneNote recycle bin. <br/>
	/// For ease, use <see cref="OneNoteItemExtensions.IsInRecycleBin"/> instead.
	/// </summary>
	/// <seealso cref="OneNoteSection"/>
	/// <seealso cref="OneNotePage"/>
	/// <seealso cref="OneNoteItemExtensions.IsInRecycleBin"/>
	public interface IHasIsInRecycleBin : IOneNoteItem
	{
		/// <summary>
		/// Indicates whether the item is in recycle bin.
		/// </summary>
		bool IsInRecycleBin { get; }
	}
}
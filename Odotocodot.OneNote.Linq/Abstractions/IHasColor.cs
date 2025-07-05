using System.Drawing;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents an OneNote hierarchy item that has a color.
	/// </summary>
	/// <seealso cref="OneNoteNotebook"/>
	/// <seealso cref="OneNoteSection"/>
	public interface IHasColor : IOneNoteItem
	{
		/// <summary>
		/// The color of the notebook.
		/// </summary>
		Color? Color { get; }
	}
}
using System.Drawing;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	public interface IHasColor : IOneNoteItem
	{
		Color? Color { get; }
	}
}
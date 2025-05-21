using System.Drawing;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	internal interface IWritableHasColor : IHasColor
	{
		new Color? Color { set; }
	}
}
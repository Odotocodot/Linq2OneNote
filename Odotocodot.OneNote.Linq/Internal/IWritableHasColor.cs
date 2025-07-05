using Odotocodot.OneNote.Linq.Abstractions;
using System.Drawing;

namespace Odotocodot.OneNote.Linq.Internal
{
	internal interface IWritableHasColor : IHasColor
	{
		new Color? Color { set; }
	}
}
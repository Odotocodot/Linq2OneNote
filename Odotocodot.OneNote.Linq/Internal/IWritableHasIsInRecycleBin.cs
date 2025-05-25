using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Internal
{
	internal interface IWritableHasIsInRecycleBin : IHasIsInRecycleBin
	{
		new bool IsInRecycleBin { set; }
	}
}
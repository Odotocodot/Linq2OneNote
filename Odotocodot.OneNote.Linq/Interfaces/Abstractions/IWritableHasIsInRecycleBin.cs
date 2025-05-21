namespace Odotocodot.OneNote.Linq.Abstractions
{
	internal interface IWritableHasIsInRecycleBin : IHasIsInRecycleBin
	{
		new bool IsInRecycleBin { set; }
	}
}
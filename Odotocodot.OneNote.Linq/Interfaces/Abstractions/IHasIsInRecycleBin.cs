namespace Odotocodot.OneNote.Linq.Abstractions
{
	public interface IHasIsInRecycleBin : IOneNoteItem
	{
		bool IsInRecycleBin { get; }
	}
}
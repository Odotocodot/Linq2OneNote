namespace Odotocodot.OneNote.Linq.Abstractions
{
	public interface IHasPath : IOneNoteItem
	{
		string Path { get; }
	}
}
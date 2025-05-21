namespace Odotocodot.OneNote.Linq.Abstractions
{
	internal interface IWritableHasPath : IHasPath
	{
		new string Path { set; }
	}
}
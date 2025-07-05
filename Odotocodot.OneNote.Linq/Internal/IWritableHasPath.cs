using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal interface IWritableHasPath : IHasPath
    {
        new string Path { set; }
    }
}
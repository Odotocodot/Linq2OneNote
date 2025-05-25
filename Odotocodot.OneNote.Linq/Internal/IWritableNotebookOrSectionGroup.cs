using Odotocodot.OneNote.Linq.Abstractions;
using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal interface IWritableNotebookOrSectionGroup : INotebookOrSectionGroup
    {
        new IEnumerable<OneNoteSection> Sections { set; }
        new IEnumerable<OneNoteSectionGroup> SectionGroups { set; }
    }
}
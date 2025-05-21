using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Abstractions
{
    internal interface IWritableNotebookOrSectionGroup : INotebookOrSectionGroup
    {
        new IEnumerable<OneNoteSection> Sections { set; }
        new IEnumerable<OneNoteSectionGroup> SectionGroups { set; }
    }
    
    
}
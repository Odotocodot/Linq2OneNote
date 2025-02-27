using System;
using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq
{
    internal class OneNoteItem : IOneNoteItem
    {
        public string ID { get; }
        public string Name { get; }
        public bool IsUnread { get; }
        public DateTime LastModified { get; }
        public IEnumerable<IOneNoteItem> Children { get; }
        public IOneNoteItem Parent { get; }
        public string RelativePath { get; }
        public OneNoteNotebook Notebook { get; }
    }
}

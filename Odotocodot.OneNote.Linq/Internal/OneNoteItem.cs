using System;
using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Internal
{
    /// <summary>
    /// Use <see cref="IOneNoteItem"/> instead.
    /// </summary>
    /// <seealso cref="IOneNoteItem"/>
    public abstract class OneNoteItem : IOneNoteItem
    {
        internal OneNoteItem() { }

        /// <inheritdoc/>
        public string ID { get; internal set; }
        /// <inheritdoc/>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public bool IsUnread { get; internal set; }
        /// <inheritdoc/>
        public DateTime LastModified { get; internal set; }
        /// <inheritdoc/>
        public abstract IEnumerable<IOneNoteItem> Children { get; }
        /// <inheritdoc/>
        public virtual IOneNoteItem Parent { get; internal set; }
        /// <inheritdoc/>
        public virtual string RelativePath { get; internal set; }
        /// <inheritdoc/>
        public virtual OneNoteNotebook Notebook { get; internal set; }
    }
}

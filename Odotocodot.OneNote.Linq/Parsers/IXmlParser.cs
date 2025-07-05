using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Parsers
{
    internal interface IXmlParser
    {
        IEnumerable<OneNoteNotebook> ParseNotebooks(string xml);
        IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent);
    }
}
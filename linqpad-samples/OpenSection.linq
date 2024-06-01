using Odotocodot.OneNote.Linq;

var page = OneNoteApplication.FindPages("This specific search").MaxBy(p => p.LastModified);
OneNoteApplication.OpenInOneNote(page.Section);
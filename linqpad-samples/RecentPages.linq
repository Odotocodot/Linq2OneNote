using Odotocodot.OneNote.Linq;

var pages = OneNoteApplication.GetNotebooks()
    .GetPages()
    .Where(p => !p.IsInRecycleBin)
    .OrderByDescending(p => p.LastModified)
    .Take(5);

foreach (var page in pages)
{
    Console.WriteLine(page.Name);
}
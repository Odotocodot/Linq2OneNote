using Odotocodot.OneNote.Linq;

var items = OneNoteApplication.GetNotebooks()
    .Traverse(i => i.IsInRecycleBin()) // use an extension method to check if the item is in the recycle bin
    .Where(i => i switch
    {
		// skip the special recycle bin section group
        OneNoteSectionGroup sectionGroup when sectionGroup.IsRecycleBin => false,
		// skip the special deleted pages section in a recycle bin
        OneNoteSection section when section.IsDeletedPages => false, 
        _ => true
    })
    .ToList();


Console.WriteLine(items.Count);
foreach (var item in items)
{
    Console.WriteLine(item.Name);
}
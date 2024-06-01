# Samples and Examples

The library's original purpose was for the OneNote [Flow Launcher](https://www.flowlauncher.com/) plugin available [here](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote). This plugin itself has several good examples of how to use the library.

The examples below are not exactly best practices, but they should give you a good starting point!

[//]: # (TODO Fix the LinqPad Link)
They can also be found in [LinqPad]() for easy viewing! (Though be weary of the [Create Page](#create-pages-in-sections-with-less-than-2-pages) example as it will create a pages in your OneNote!)
### Get Recent Pages

```csharp
var pages = OneNoteApplication.GetNotebooks()
    .GetPages()
    .Where(p => !p.IsInRecycleBin)
    .OrderByDescending(p => p.LastModified)
    .Take(5);

foreach (var page in pages)
{
    Console.WriteLine(page.Name);
}
```

### Get All Items in Recycle Bins

```csharp
var items = OneNoteApplication.GetNotebooks()
    .Traverse(i => i.IsInRecycleBin()) // use an extension method to check if the item is in the recycle bin
    .Where(i => i switch
    {
        OneNoteSectionGroup sectionGroup when sectionGroup.IsRecycleBin => false, // skip the special recycle bin section group
        OneNoteSection section when section.IsDeletedPages => false, // skip the special deleted pages section in a recycle bin
        _ => true
    })
    .ToList();


Console.WriteLine(items.Count);
foreach (var item in items)
{
    Console.WriteLine(item.Name);
}
```

### Create Pages in Sections With Less Than 2 Pages

> [!WARNING]
> If you decide to run this code it will create pages (potentially hundreds :dizzy_face:) in your OneNote!

```csharp
//IF YOU RUN THIS IT WILL CREATE A PAGES IN YOUR ONENOTE!
var newPageName = "Hopefully a very unique and specific title!";
var sections = OneNoteApplication.GetNotebooks()
    .Traverse(i => i is OneNoteSection)
    .Cast<OneNoteSection>()
    .Where(s => s.Pages.Count() <= 1);
foreach (var section in sections)
{
    OneNoteApplication.CreatePage(section, newPageName, false);
}
```

### Search for a Page and Open Its Section

```csharp
var page = OneNoteApplication.FindPages("This specific search").MaxBy(p => p.LastModified);
OneNoteApplication.OpenInOneNote(page.Section);
```

### TEST
[!code-csharp[](../../linqpad-samples/RecentPages.linq#L5-L16)]

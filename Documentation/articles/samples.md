# Samples and Examples

The library's original purpose was for the OneNote [Flow Launcher](https://www.flowlauncher.com/) plugin available [here](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote). This plugin itself has several good examples of how to use the library.

The examples below are not exactly best practices, but they should give you a good starting point!

They can also be found in the free and paid version of [LinqPad](https://www.linqpad.net/) for easy viewing! (Though be weary of the [Create Page](#create-pages-in-sections-with-less-than-2-pages) example as it will create a pages in your OneNote!)
### Get Recent Pages

[!code-csharp[](../../linqpad-samples/RecentPages.linq#L7-L18)]

### Get All Items in Recycle Bins

[!code-csharp[](../../linqpad-samples/RecycleBinItems.linq#L7-L26)]

### Search for a Page and Open Its Section

[!code-csharp[](../../linqpad-samples/OpenSection.linq#L7-L17)]

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





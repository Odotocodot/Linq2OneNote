![logo](https://github.com/Odotocodot/Linq2OneNote/assets/48138990/bf168880-0bb7-47ac-80dd-e0825a1e021a)
# LINQ to OneNote

A helper library for dealing with the [OneNote Interop API](https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote).
Originally made for [Flow.Launcher.Plugin.OneNote](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote).

## Usage

Visit the [API Reference](https://odotocodot.github.io/Linq2OneNote/api/Odotocodot.OneNote.Linq.html) to see the full API, or visit the Flow Launcher [plugin](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote/blob/master/Flow.Launcher.Plugin.OneNote/SearchManager.cs) to see it in action.\
To see an outline of the library view the [class diagram](https://github.com/Odotocodot/Linq2OneNote/blob/main/Documentation/images/class_diagram.png).

See the [documentation](https://odotocodot.github.io/Linq2OneNote/) for more information and examples!

### Quick Start

The main entry point of the library is the static class ``OneNoteApplication`` which has a collection of [methods](https://odotocodot.github.io/Linq2OneNote/api/Odotocodot.OneNote.Linq.OneNoteApplication.html#methods) that interact with your OneNote installation.

Below is quick example on using the library to search your OneNote pages.

```csharp
//Search pages that have "hello there" in the title or content.
IEnumerable<OneNotePage> pages = OneNoteApplication.FindPages("hello there");

OneNotePage page = pages.FirstOrDefault();

Console.WriteLine(page.Name);

page.OpenInOneNote();
```

Most functions return an IEnumerable allowing for easy use with LINQ.


## Inspired By

- [ScipeBe Common Office](https://github.com/scipbe/ScipBe-Common-Office)
- [OneNote Object Model](https://github.com/idvorkin/onom)
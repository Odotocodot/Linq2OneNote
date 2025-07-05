<h1 align="center">
    <img src="https://github.com/Odotocodot/Linq2OneNote/assets/48138990/4b6025ab-6aa7-4d5e-aac6-2328961daeb5" alt="logo" width=40 height=40>
LINQ to OneNote
    <img src="https://github.com/Odotocodot/Linq2OneNote/assets/48138990/9f6b5f41-ed6a-4840-8766-fd5890c6bb7c" alt="logo mini" width=40 height=40>
</h1>

A helper library for dealing with the [OneNote Interop API](https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote).
Originally made for [Flow.Launcher.Plugin.OneNote](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote).

## Installation

Get the library from NuGet [here](https://www.nuget.org/packages/Odotocodot.OneNote.Linq/):
```
dotnet add package Odotocodot.OneNote.Linq
```

## Usage

Visit the [API Reference](https://odotocodot.github.io/Linq2OneNote/api/Odotocodot.OneNote.Linq.html) to see the full API, or visit the Flow Launcher [plugin](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote/blob/master/Flow.Launcher.Plugin.OneNote/SearchManager.cs) to see it in action.\
To see an outline of the library view the [class diagram](https://github.com/Odotocodot/Linq2OneNote/blob/main/Documentation/images/class_diagram.png).

:star: See the [documentation](https://odotocodot.github.io/Linq2OneNote/) for more information and examples! :star:

### Quick Start

The main entry point of the library is the static class ``OneNoteApplication`` which has a collection of [methods](https://odotocodot.github.io/Linq2OneNote/api/Odotocodot.OneNote.Linq.OneNoteApplication.html#methods) that interact with your OneNote installation.

Below is quick example on using the library to search your OneNote pages.

```csharp
using System.Linq;
using Odotocodot.OneNote.Linq;

namespace Linq2OneNoteExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Search pages that have "hello there" in the title or content.
            IEnumerable<OneNotePage> pages = OneNoteApplication.FindPages("hello there");
            
            OneNotePage page = pages.FirstOrDefault();

            Console.WriteLine(page.Name);
            
            page.OpenInOneNote();
        }
    }
}
```

Most functions return an IEnumerable allowing for easy use with LINQ.

### Features

- Search your OneNote pages, and optionally specify a notebook, section group or section to restrict the search to.
- Traverse your whole OneNote hierarchy.
- Create a new notebook, section group, section, or page in OneNote.
- Open a notebook, section group, section, or page in OneNote.


## Inspired By

- [ScipeBe Common Office](https://github.com/scipbe/ScipBe-Common-Office)
- [OneNote Object Model](https://github.com/idvorkin/onom)
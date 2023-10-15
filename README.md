<h1 align="center"> LINQ to OneNote </h1>

A helper library for dealing with the [OneNote Interop API](https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote)
([package](https://www.nuget.org/packages/Interop.Microsoft.Office.Interop.OneNote#readme-body-tab)).<br/>
Originally made for [Flow.Launcher.Plugin.OneNote](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote).

## Installation

1. Install either "OneNote" or "OneNote for Windows 10"
1. TODO nuget

> [!IMPORTANT]
> This library only works for local versions of OneNote, and does not make use of the Microsoft Graph API.

## Usage

Visit to the [API Reference](https://odotocodot.github.io/Linq2OneNote/api/Odotocodot.OneNote.Linq.html) to see the full API, or visit the Flow Launcher [plugin](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote/blob/master/Flow.Launcher.Plugin.OneNote/SearchManager.cs) to see it in action.<br/>
To see an outline of the library view the [class diagram](https://github.com/Odotocodot/Linq2OneNote/blob/main/Documentation/images/class_diagram.png).


### Getting Started

The main entry point of the library is the static class ``OneNoteApplication`` which has a collection of [methods](https://odotocodot.github.io/Linq2OneNote/Odotocodot.OneNote.Linq.OneNoteApplication) that interact with your OneNote installaiton.

Below is quick example on using the library to search your OneNote pages. 

```csharp
//Search pages that have "hello there" in the title or content.
IEnumerable<OneNotePage> pages = OneNoteApplication.FindPages("hello there");

OneNotePage page = pages.FirstOrDefault();

Console.WriteLine(page.Name);

page.OpenInOneNote();
```

Most functions return an IEnuermable allowing for easy use with LINQ.

### Memory Management

A COM object is required to use the OneNote Interop API, by default Linq2OneNote acquires an object lazily, i.e. the first time you call a method that requires a COM object, the library gets one.

However, acquiring a COM object is _slow_ and once retrieved it is visible in the Task Manager ([screenshot](https://github.com/Odotocodot/Linq2OneNote/blob/main/Documentation/images/task_manager.png)).

If you want to choose when the this operation occurs, you can call ``OneNoteApplication.InitComObject()`` to forcible acquire the COM object (it does nothing if one has already been attained).<br/>

To free up the memory that the COM object takes up, rather they wait for your application to exit you can call  ``OneNoteApplication.ReleaseComObject()``.

See below for an example.

```csharp
//Get the COM object
OneNoteApplication.InitComObject();

//Do stuff e.g.
OneNoteNotebook notebooks = OneNoteApplication.GetNotebooks();

foreach (var notebook in notebooks)
{
    Console.WriteLine(notebook.Name)
}

IEnumerable<OneNotePage> pages = notebooks.Traverse(n => n.Children.Count() > 3).GetPages();

foreach (var page in pages)
{
    Console.WriteLine(page.Section.Name);
}

//Release the COM object to free memory
OneNoteApplication.ReleaseComObject()
```

## Inspired By

- [ScipeBe Common Office](https://github.com/scipbe/ScipBe-Common-Office)
- [OneNote Object Model](https://github.com/idvorkin/onom)
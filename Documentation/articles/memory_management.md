# Memory Management

A COM object is required to use the OneNote Interop API, by default this is acquired lazily, i.e. the first time you call a method that requires a COM object, the library gets one.

However, acquiring a COM object is _slow_ and once retrieved, it is visible in the Task Manager as shown below.

![task manager screenshot](~/images/task_manager.png)

If you want to choose when this operation occurs, you can call ``OneNoteApplication.InitComObject()`` to forcible acquire the COM object (it does nothing if one has already been attained).

To free up the memory that the COM object takes up, rather than wait for your application to exit you can call  ``OneNoteApplication.ReleaseComObject()``.

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
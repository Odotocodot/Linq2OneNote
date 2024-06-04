# Additional Information

### The Hierarchy Conundrum

Currently, the full hierarchy is returned when calling `OneNoteApplication.GetNotebooks()`. 
Which means depending on the number of notebooks, section groups, sections and pages you have it can take a significant amount of time.

Furthermore, when calling `OneNoteApplication.FindPages()` the hierarchy returned is _partially full_. 
The pages returned have references all the way to the notebooks that owns them, but those notebooks will not have all there 
descendants (section groups, sections and pages) present, only the ones related to the pages returned.

This can lead to weird scenarios such as:
```csharp
var page = OneNoteApplication.FindPages("A unique page").First();
var searchNotebook = page.Notebook;
//or alternatively
IOneNoteItem item = page;
while (item is not OneNoteNotebook)
{
    item = item.Parent;
}

Console.WriteLine(item == searchNotebook); //Prints TRUE

var getAllNotebook = OneNoteApplication.GetNotebooks().First(n => n.Name == searchNotebook.Name);
//Here's where things get weird
Console.WriteLine(searchNotebook == getAllNotebook); //Prints FALSE
Console.WriteLine(searchNotebook.ID == getAllNotebook.ID); //Prints TRUE
Console.WriteLine(searchNotebook.Children.Traverse().Count() ==
                  getAllNotebook.Children.Traverse().Count()); //Prints FALSE
```

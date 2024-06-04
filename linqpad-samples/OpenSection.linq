<Query Kind="Statements">
  <NuGetReference>Odotocodot.OneNote.Linq</NuGetReference>
  <Namespace>Odotocodot.OneNote.Linq</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

using Odotocodot.OneNote.Linq;

var page = OneNoteApplication.FindPages("This specific search").MaxBy(p => p.LastModified);
if(page == null)
{
	Console.WriteLine("No page found with that search, try changing it!");
}
else
{
	OneNoteApplication.OpenInOneNote(page.Section);
}
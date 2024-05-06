using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestSubject(typeof(Parser))]
	public class ParserTests
	{

		[Test]
		public void ParsePage()
		{
			var xml = File.ReadAllText(@"Inputs\Page.xml");
			var notebook = new OneNoteNotebook();
			var section = new OneNoteSection();
			section.Notebook = notebook;
			
			var item = Parser.ParseUnknown(xml, section);
			
			Assert.Multiple(() =>
			{
				Assert.IsInstanceOf<OneNotePage>(item);
				var page = (OneNotePage)item;
				Assert.AreEqual("{1B9CDD3C-6836-4DC6-9C44-0EDC06A9B8CB}{1}{E19481616267573963101920151005250203326127411}", page.ID);
				Assert.AreEqual("Important Info", page.Name);
				Assert.AreEqual(new DateTime(2022, 12, 1, 18, 10, 2), page.Created);
				Assert.AreEqual(new DateTime(2022, 12, 1, 18, 10, 34), page.LastModified);
				Assert.AreEqual(1, page.Level);
				Assert.IsTrue(page.IsUnread);
				Assert.IsFalse(page.IsInRecycleBin);
				
				Assert.AreSame(section, page.Section);
				Assert.AreSame(notebook, page.Notebook);
			});

		}

		[Test]
		[TestCase(typeof(OneNoteNotebook), 4)]
		[TestCase(typeof(OneNoteSectionGroup), 7)]
		[TestCase(typeof(OneNoteSection), 20)]							
		[TestCase(typeof(OneNotePage), 28)]
		public void ParseNotebooks_CorrectNumberOfItems(Type itemType, int expectedCount)
		{
			// var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Inputs", "Notebooks.xml");
			var xml = File.ReadAllText(@"Inputs\Notebooks.xml");
			
			var result = Parser.ParseNotebooks(xml);
			var items = result.Traverse(item => item.GetType() == itemType);
			
			Assert.AreEqual(expectedCount, items.Count());
		}
	}

}
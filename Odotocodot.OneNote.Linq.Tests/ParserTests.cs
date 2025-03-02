using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Odotocodot.OneNote.Linq.Parsers;

namespace Odotocodot.OneNote.Linq.Tests
{
    [TestFixture]
	[TestOf(typeof(XElementXmlParser))]
	public class ParserTests
	{
		[Test]
		[TestCase(typeof(OneNoteNotebook), 4)]
		[TestCase(typeof(OneNoteSectionGroup), 7)]
		[TestCase(typeof(OneNoteSection), 20)]							
		[TestCase(typeof(OneNotePage), 28)]
		public void ParseNotebooks_CorrectNumberOfItems(Type itemType, int expectedCount)
		{
			var xml = File.ReadAllText(@"Inputs\Notebooks.xml");
			
			var result = XElementXmlParser.ParseNotebooks(xml);
			var items = result.Traverse(item => item.GetType() == itemType);
			
			Assert.AreEqual(expectedCount, items.Count());
		}
	}
}
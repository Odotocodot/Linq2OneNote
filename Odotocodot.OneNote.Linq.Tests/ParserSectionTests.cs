using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestOf(typeof(OneNoteParser))]
	[TestOf(typeof(OneNoteSection))]
	public class ParserSectionTests : BaseParserTest<OneNoteSection>
	{
		public override void OneTimeSetUp()
		{
			var xml = File.ReadAllText(@"Inputs\Section.xml");
			notebook = new OneNoteNotebook(){ Name = "Test Notebook" };
			id = "{6BB816F6-D431-4430-B7A2-F9DEB7A28F67}{1}{B0}";
			name = "Locked Section";
			isUnread = false;
			lastModified = new DateTime(2023, 06, 17, 11, 00, 52);
			relativePath = $"Test Notebook{OneNoteParser.RelativePathSeparator}Locked Section";
			parent = notebook;
			item = OneNoteParser.ParseUnknown(xml, notebook);
		}

		[Test]
		public void PathCheck() =>
			Assert.AreEqual(@"C:\Users\User\Documents\OneNote Notebooks\Test Notebook\Locked Section.one",
				TypedItem.Path);

		[Test]
		public void ColorCheck() => Assert.AreEqual(ColorTranslator.FromHtml("#BA7575"), TypedItem.Color);
		
		[Test]
		public void LockedCheck() => Assert.IsTrue(TypedItem.Locked);
		
		[Test]
		public void EncryptedCheck() => Assert.IsTrue(TypedItem.Encrypted);
		
		[Test]
		public void IsInRecycleBinCheck() => Assert.IsFalse(TypedItem.IsInRecycleBin);
		
		[Test]
		public void IsDeletedPagesCheck() => Assert.IsFalse(TypedItem.IsDeletedPages);

		[Test]
		public void PagesCheck() => Assert.IsEmpty(TypedItem.Pages);
	}
}
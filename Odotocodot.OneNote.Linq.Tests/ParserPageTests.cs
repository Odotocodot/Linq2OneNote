using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestOf(typeof(OneNoteParser))]
	[TestOf(typeof(OneNotePage))]
	public class ParserPageTests : BaseParserTest<OneNotePage>
	{
		public override void OneTimeSetUp()
		{
			var xml = File.ReadAllText(@"Inputs\Page.xml");
			notebook = new OneNoteNotebook() { Name = "Test Notebook" };
			var section = new OneNoteSection
			{
				RelativePath = "Test Notebook\\Test Section",
				Notebook = notebook
			};
			id = "{1B9CDD3C-6836-4DC6-9C44-0EDC06A9B8CB}{1}{E19481616267573963101920151005250203326127411}";
			name = "Important Info";
			isUnread = true;
			lastModified = new DateTime(2022, 12, 1, 18, 10, 34);
			relativePath = $"Test Notebook{OneNoteParser.RelativePathSeparator}Test Section{OneNoteParser.RelativePathSeparator}Important Info";
			parent = section;
			item = OneNoteParser.ParseUnknown(xml, section);
		}
		
		
		[Test]
		public void CreatedCheck() => Assert.AreEqual(new DateTime(2022, 12, 1, 18, 10, 2), TypedItem.Created);
		
		[Test]
		public void LevelCheck() => Assert.AreEqual(1, TypedItem.Level);
		
		[Test]
		public void IsInRecycleBinCheck() => Assert.IsFalse(TypedItem.IsInRecycleBin);

		[Test]
		public void SectionCheck() => Assert.AreSame(parent, TypedItem.Section);

	}
}
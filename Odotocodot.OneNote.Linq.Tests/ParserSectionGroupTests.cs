using System;
using System.IO;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestOf(typeof(XmlParser))]
	[TestOf(typeof(OneNoteSectionGroup))]
	public class ParserSectionGroupTests : BaseParserTest<OneNoteSectionGroup>
	{
		public override void OneTimeSetUp()
		{
			var xml = File.ReadAllText(@"Inputs\SectionGroup.xml");
			notebook = new OneNoteNotebook() { Name = "Test Notebook" };
			id = "{C55815E0-8F65-4790-8408-2E2C1EC74AB2}{1}{B0}";
			name = "Section Group 1";
			isUnread = false;
			lastModified = new DateTime(2023, 10, 04, 20, 48, 19);
			relativePath = $"Test Notebook{XmlParser.RelativePathSeparator}Section Group 1";
			parent = notebook;
			item = XmlParser.ParseUnknown(xml, notebook);
		}

		[Test]
		public void PathCheck() =>
			Assert.AreEqual(@"C:\Users\User\Documents\OneNote Notebooks\Test Notebook\Section Group 1", TypedItem.Path);
		
		[Test]
		public void IsRecycleBinCheck() => Assert.IsFalse(TypedItem.IsRecycleBin);
		
		[Test]
		public void SectionsCheck() => Assert.IsEmpty(TypedItem.Sections);
		
		[Test]
		public void SectionGroupsCheck() => Assert.IsEmpty(TypedItem.SectionGroups);
	}
}
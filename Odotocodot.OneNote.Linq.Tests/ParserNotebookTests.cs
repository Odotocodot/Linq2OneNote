using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestOf(typeof(OneNoteParser))]
	[TestOf(typeof(OneNoteNotebook))]
	public class ParserNotebookTests : BaseParserTest<OneNoteNotebook>
	{
		public override void OneTimeSetUp()
		{
			var xml = File.ReadAllText(@"Inputs\Notebook.xml");
			id = "{81B591F0-CB49-4F8C-BFB1-98DA213B93FC}{1}{B0}";
			name = "Its A Notebook";
			isUnread = false;
			lastModified = new DateTime(2023, 10, 04, 15, 15, 45);
			relativePath = name;
			parent = null;
			item = OneNoteParser.ParseUnknown(xml, null);
			notebook = (OneNoteNotebook)item;
		}
		[Test]
		public void NickNameCheck() => Assert.AreEqual("It's A Notebook", TypedItem.NickName);
		
		[Test]
		public void ColorCheck() => Assert.AreEqual(ColorTranslator.FromHtml("#EE9597"), TypedItem.Color);
		
		[Test]
		public void PathCheck() => Assert.AreEqual(@"C:\Users\User\Desktop\Its A Notebook", TypedItem.Path);
		
		[Test]
		public void SectionsCheck() => Assert.IsEmpty(TypedItem.Sections);
		
		[Test]
		public void SectionGroupsCheck() => Assert.IsEmpty(TypedItem.SectionGroups);
	}
}
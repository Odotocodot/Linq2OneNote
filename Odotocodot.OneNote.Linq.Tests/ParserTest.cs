using System;
using System.Drawing;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestSubject(typeof(Parser))]
	[TestOf(typeof(Parser))]
	public class ParserTests
	{
		[Test]
		public void ParsePage()
		{
			var xml = File.ReadAllText(@"Inputs\Page.xml");
			var notebook = new OneNoteNotebook() { Name = "Test Notebook" };
			var section = new OneNoteSection
			{
				RelativePath = "Test Notebook\\Test Section",
				Notebook = notebook
			};

			var item = Parser.ParseUnknown(xml, section);
			
			Assert.Multiple(() =>
			{
				Assert.IsInstanceOf<OneNotePage>(item);
				var page = (OneNotePage)item;
				Assert.AreEqual("{1B9CDD3C-6836-4DC6-9C44-0EDC06A9B8CB}{1}{E19481616267573963101920151005250203326127411}",
					page.ID);
				Assert.AreEqual("Important Info", page.Name);
				Assert.AreEqual(new DateTime(2022, 12, 1, 18, 10, 2), page.Created);
				Assert.AreEqual(new DateTime(2022, 12, 1, 18, 10, 34), page.LastModified);
				Assert.AreEqual(1, page.Level);
				Assert.IsTrue(page.IsUnread);
				Assert.IsFalse(page.IsInRecycleBin);
				
				Assert.AreSame(section, page.Section);
				Assert.AreSame(notebook, page.Notebook);
				Assert.AreEqual($"Test Notebook{Parser.RelativePathSeparator}Test Section{Parser.RelativePathSeparator}Important Info",
					page.RelativePath);
			});

		}

		[Test]
		public void ParseSection()
		{
			var xml = File.ReadAllText(@"Inputs\Section.xml");
			var notebook = new OneNoteNotebook(){ Name = "Test Notebook" };
			var item = Parser.ParseUnknown(xml, notebook);
			Assert.Multiple(() =>
			{
				Assert.IsInstanceOf<OneNoteSection>(item);
				var section = (OneNoteSection)item;
				Assert.AreEqual("{6BB816F6-D431-4430-B7A2-F9DEB7A28F67}{1}{B0}", section.ID);
				Assert.AreEqual("Locked Section", section.Name);
				Assert.AreEqual(new DateTime(2023, 06, 17, 11, 00, 52), section.LastModified);
				Assert.AreEqual(@"C:\Users\User\Documents\OneNote Notebooks\Test Notebook\Locked Section.one",
					section.Path);
				Assert.AreEqual(ColorTranslator.FromHtml("#BA7575"), section.Color);
				Assert.IsTrue(section.Encrypted);
				Assert.IsTrue(section.Locked);

				Assert.IsFalse(section.IsUnread);
				Assert.IsFalse(section.IsInRecycleBin);
				Assert.IsFalse(section.IsDeletedPages);

				Assert.AreSame(notebook, section.Parent);
				Assert.IsNull(section.Notebook);
				Assert.AreEqual($"Test Notebook{Parser.RelativePathSeparator}Locked Section", section.RelativePath);
				
				Assert.IsEmpty(section.Pages);
			});
		}

		[Test]
		public void ParseSectionGroup()
		{
			var xml = File.ReadAllText(@"Inputs\SectionGroup.xml");
			var notebook = new OneNoteNotebook() { Name = "Test Notebook" };
			var item = Parser.ParseUnknown(xml, notebook);
			
			Assert.Multiple(() =>
			{
				Assert.IsInstanceOf<OneNoteSectionGroup>(item);
				var sectionGroup = (OneNoteSectionGroup)item;
				Assert.AreEqual("{C55815E0-8F65-4790-8408-2E2C1EC74AB2}{1}{B0}", sectionGroup.ID);
				Assert.AreEqual("Section Group 1", sectionGroup.Name);
				Assert.AreEqual(new DateTime(2023, 10, 04, 20, 48, 19), sectionGroup.LastModified);
				Assert.AreEqual(@"C:\Users\User\Documents\OneNote Notebooks\Test Notebook\Section Group 1",
					sectionGroup.Path);
				
				Assert.IsFalse(sectionGroup.IsUnread);
				Assert.IsFalse(sectionGroup.IsRecycleBin);
				
				Assert.AreSame(notebook, sectionGroup.Parent);
				Assert.IsNull(sectionGroup.Notebook);
				
				Assert.AreEqual($"Test Notebook{Parser.RelativePathSeparator}Section Group 1", sectionGroup.RelativePath);
				
				Assert.IsEmpty(sectionGroup.Sections);
				Assert.IsEmpty(sectionGroup.SectionGroups);
				
			});
		}

		[Test]
		public void ParseNotebook()
		{
			var xml = File.ReadAllText(@"Inputs\Notebook.xml");
			var item = Parser.ParseUnknown(xml, null);
			
			Assert.Multiple(() =>
			{
				Assert.IsInstanceOf<OneNoteNotebook>(item);
				var notebook = (OneNoteNotebook)item;
				Assert.AreEqual("{81B591F0-CB49-4F8C-BFB1-98DA213B93FC}{1}{B0}", notebook.ID);
				Assert.AreEqual("Its A Notebook", notebook.Name);
				Assert.AreEqual("It's A Notebook", notebook.NickName);
				Assert.AreEqual(new DateTime(2023,10,04,15,15,45), notebook.LastModified);
				Assert.AreEqual(@"C:\Users\User\Desktop\Its A Notebook", notebook.Path);
				
				Assert.AreEqual(ColorTranslator.FromHtml("#EE9597"), notebook.Color);
				Assert.IsFalse(notebook.IsUnread);
				
				Assert.IsEmpty(notebook.Sections);
				Assert.IsEmpty(notebook.SectionGroups);
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
using NUnit.Framework;
using Odotocodot.OneNote.Linq.Parsers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Odotocodot.OneNote.Linq.Tests
{
    [TestFixture(typeof(XElementXmlParser))]
    [TestFixture(typeof(XmlReaderXmlParser))]
    internal class ParserTests<TXmlParser> where TXmlParser : IXmlParser
    {
        private IXmlParser xmlParser;
        private OneNoteNotebook notebookStub;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            xmlParser = Activator.CreateInstance<TXmlParser>();
            notebookStub = new OneNoteNotebook { Name = "Test Notebook" };
        }

        [Test]
        [TestCase(typeof(OneNoteNotebook), 4)]
        [TestCase(typeof(OneNoteSectionGroup), 7)]
        [TestCase(typeof(OneNoteSection), 20)]
        [TestCase(typeof(OneNotePage), 28)]
        public void ParseNotebooks_CorrectNumberOfItems(Type itemType, int expectedCount)
        {
            var xml = File.ReadAllText(@"Inputs\Notebooks.xml");

            var result = xmlParser.ParseNotebooks(xml);
            var items = result.Traverse(item => item.GetType() == itemType);

            Assert.That(items.Count(), Is.EqualTo(expectedCount));
        }

        private static void AssertStandardProperties<T>(IOneNoteItem item, string expectedName, string expectedID,
            bool expectedIsUnread, DateTime expectedLastModified, string expectedRelativePath,
            IOneNoteItem expectedParent, OneNoteNotebook expectedNotebook) where T : IOneNoteItem
        {
            Assert.Multiple(() =>
            {
                Assert.That(item, Is.Not.Null);
                Assert.That(item.Name, Is.EqualTo(expectedName));
                Assert.That(item.ID, Is.EqualTo(expectedID));
                Assert.That(item.IsUnread, Is.EqualTo(expectedIsUnread));
                Assert.That(item.LastModified, Is.EqualTo(expectedLastModified));
                Assert.That(item.RelativePath, Is.EqualTo(expectedRelativePath));
                Assert.That(item.Parent, Is.EqualTo(expectedParent));
                Assert.That(item.Notebook, Is.SameAs(expectedNotebook));
                Assert.That(item, Is.InstanceOf<T>());
            });
        }

        [Test]
        public void ParseNotebook_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\Notebook.xml");
            var item = xmlParser.ParseUnknown(xml, null);
            var expectedName = "Its A Notebook";
            AssertStandardProperties<OneNoteNotebook>(
                item,
                expectedName,
                "{81B591F0-CB49-4F8C-BFB1-98DA213B93FC}{1}{B0}",
                false,
                new DateTime(2023, 10, 04, 15, 15, 45),
                expectedName,
                null,
                (OneNoteNotebook)item);

            var notebook = (OneNoteNotebook)item;
            Assert.Multiple(() =>
            {
                Assert.That(notebook.NickName, Is.EqualTo("It's A Notebook"));
                Assert.That(notebook.Color, Is.EqualTo(ColorTranslator.FromHtml("#EE9597")));
                Assert.That(notebook.Path, Is.EqualTo(@$"C:\Users\User\Desktop\{expectedName}"));
                Assert.That(notebook.Sections, Is.Empty);
                Assert.That(notebook.SectionGroups, Is.Empty);
            });
        }

        [Test]
        public void ParseSectionGroup_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\SectionGroup.xml");
            var item = xmlParser.ParseUnknown(xml, notebookStub);
            var expectedName = "Section Group 1";
            AssertStandardProperties<OneNoteSectionGroup>(
                item,
                expectedName,
                "{C55815E0-8F65-4790-8408-2E2C1EC74AB2}{1}{B0}",
                false,
                new DateTime(2023, 10, 04, 20, 48, 19),
                $@"{notebookStub.Name}{XmlParserHelpers.RelativePathSeparator}{expectedName}",
                notebookStub,
                notebookStub);

            var sectionGroup = (OneNoteSectionGroup)item;
            Assert.Multiple(() =>
            {
                Assert.That(sectionGroup.IsRecycleBin, Is.False);
                Assert.That(sectionGroup.Path, Is.EqualTo(@$"C:\Users\User\Documents\OneNote Notebooks\{notebookStub.Name}\{expectedName}"));
                Assert.That(sectionGroup.Sections, Is.Empty);
                Assert.That(sectionGroup.SectionGroups, Is.Empty);
            });
        }

        [Test]
        public void ParseSection_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\Section.xml");
            var item = xmlParser.ParseUnknown(xml, notebookStub);
            var expectedName = "Locked Section";
            AssertStandardProperties<OneNoteSection>(
                item,
                expectedName,
                "{6BB816F6-D431-4430-B7A2-F9DEB7A28F67}{1}{B0}",
                false,
                new DateTime(2023, 06, 17, 11, 00, 52),
                $@"{notebookStub.Name}{XmlParserHelpers.RelativePathSeparator}{expectedName}",
                notebookStub,
                notebookStub);

            var section = (OneNoteSection)item;
            Assert.Multiple(() =>
            {
                Assert.That(section.Locked, Is.True);
                Assert.That(section.Encrypted, Is.True);
                Assert.That(section.Color, Is.EqualTo(ColorTranslator.FromHtml("#BA7575")));
                Assert.That(section.Path, Is.EqualTo(@$"C:\Users\User\Documents\OneNote Notebooks\{notebookStub.Name}\{expectedName}.one"));
                Assert.That(section.IsInRecycleBin, Is.False);
                Assert.That(section.IsDeletedPages, Is.False);
                Assert.That(section.Pages, Is.Empty);
            });
        }

        [Test]
        public void ParsePage_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\Page.xml");
            var sectionStub = new OneNoteSection
            {
                RelativePath = $@"{notebookStub.Name}{XmlParserHelpers.RelativePathSeparator}Test Section",
                Notebook = notebookStub
            };
            var item = xmlParser.ParseUnknown(xml, sectionStub);
            var expectedName = "Important Info";
            AssertStandardProperties<OneNotePage>(
                item,
                expectedName,
                "{1B9CDD3C-6836-4DC6-9C44-0EDC06A9B8CB}{1}{E19481616267573963101920151005250203326127411}",
                true,
                new DateTime(2022, 12, 01, 18, 10, 34),
                $@"{notebookStub.Name}{XmlParserHelpers.RelativePathSeparator}Test Section{XmlParserHelpers.RelativePathSeparator}{expectedName}",
                sectionStub,
                notebookStub);

            var page = (OneNotePage)item;
            Assert.Multiple(() =>
            {
                Assert.That(page.Section, Is.SameAs(sectionStub));
                Assert.That(page.Level, Is.EqualTo(1));
                Assert.That(page.Created, Is.EqualTo(new DateTime(2022, 12, 01, 18, 10, 02)));
                Assert.That(page.IsInRecycleBin, Is.False);
            });
        }

    }
}
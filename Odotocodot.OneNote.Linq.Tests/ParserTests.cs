using NUnit.Framework;
using Odotocodot.OneNote.Linq.Parsers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Odotocodot.OneNote.Linq.Tests
{
    [TestFixture(typeof(XmlParserXElement))]
    [TestFixture(typeof(XmlParserXmlReader))]
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

        private static void AssertProperties<T>(IOneNoteItem item, T expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(item, Is.Not.Null);
                Assert.That(item, Is.InstanceOf<T>());
                Assert.That(item, Is.EqualTo(expected).UsingPropertiesComparer());
            });
        }

        [Test]
        public void ParseNotebook_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\Notebook.xml");
            var item = xmlParser.ParseUnknown(xml, null);
            var expectedName = "Its A Notebook";
            var expected = new OneNoteNotebook
            {
                Name = expectedName,
                ID = "{81B591F0-CB49-4F8C-BFB1-98DA213B93FC}{1}{B0}",
                IsUnread = false,
                LastModified = new DateTime(2023, 10, 04, 15, 15, 45),
                RelativePath = expectedName,
                Parent = null,
                Notebook = null,
                Children = Enumerable.Empty<IOneNoteItem>(),

                NickName = "It's A Notebook",
                Color = ColorTranslator.FromHtml("#EE9597"),
                Path = $@"C:\Users\User\Desktop\{expectedName}",
            };

            AssertProperties(item, expected);
        }

        [Test]
        public void ParseSectionGroup_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\SectionGroup.xml");
            var item = xmlParser.ParseUnknown(xml, notebookStub);
            var expectedName = "Section Group 1";
            var expected = new OneNoteSectionGroup
            {
                Name = expectedName,
                ID = "{C55815E0-8F65-4790-8408-2E2C1EC74AB2}{1}{B0}",
                IsUnread = false,
                LastModified = new DateTime(2023, 10, 04, 20, 48, 19),
                RelativePath = $"{notebookStub.Name}{Constants.RelativePathSeparator}{expectedName}",
                Parent = notebookStub,
                Notebook = notebookStub,
                Children = Enumerable.Empty<IOneNoteItem>(),

                IsRecycleBin = false,
                Path = @$"C:\Users\User\Documents\OneNote Notebooks\{notebookStub.Name}\{expectedName}"
            };

            AssertProperties(item, expected);
        }

        [Test]
        public void ParseSection_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\Section.xml");
            var item = xmlParser.ParseUnknown(xml, notebookStub);
            var expectedName = "Locked Section";
            var expected = new OneNoteSection
            {
                Name = expectedName,
                ID = "{6BB816F6-D431-4430-B7A2-F9DEB7A28F67}{1}{B0}",
                IsUnread = false,
                LastModified = new DateTime(2023, 06, 17, 11, 00, 52),
                RelativePath = $"{notebookStub.Name}{Constants.RelativePathSeparator}{expectedName}",
                Parent = notebookStub,
                Notebook = notebookStub,
                Children = Enumerable.Empty<IOneNoteItem>(),

                Locked = true,
                Encrypted = true,
                Color = ColorTranslator.FromHtml("#BA7575"),
                Path = @$"C:\Users\User\Documents\OneNote Notebooks\{notebookStub.Name}\{expectedName}.one",
                IsInRecycleBin = false,
                IsDeletedPages = false,
            };

            AssertProperties(item, expected);
        }

        [Test]
        public void ParsePage_CorrectProperties()
        {
            var xml = File.ReadAllText(@"Inputs\Page.xml");
            var sectionStub = new OneNoteSection
            {
                RelativePath = $"{notebookStub.Name}{Constants.RelativePathSeparator}Test Section",
                Notebook = notebookStub
            };
            var item = xmlParser.ParseUnknown(xml, sectionStub);
            var expectedName = "Important Info";
            var expectedPage = new OneNotePage
            {
                Name = expectedName,
                ID = "{1B9CDD3C-6836-4DC6-9C44-0EDC06A9B8CB}{1}{E19481616267573963101920151005250203326127411}",
                IsUnread = true,
                LastModified = new DateTime(2022, 12, 01, 18, 10, 34),
                RelativePath = $"{notebookStub.Name}{Constants.RelativePathSeparator}Test Section{Constants.RelativePathSeparator}{expectedName}",
                Parent = sectionStub,
                Notebook = notebookStub,
                Children = Enumerable.Empty<IOneNoteItem>(),

                IsInRecycleBin = false,
                Created = new DateTime(2022, 12, 01, 18, 10, 02),
                Level = 1
            };

            AssertProperties(item, expectedPage);
        }
    }
}
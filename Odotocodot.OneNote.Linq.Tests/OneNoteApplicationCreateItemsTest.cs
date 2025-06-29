using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Odotocodot.OneNote.Linq.Tests
{
    // NOTE: Requires a OneNote notebook named "Test Notebook" with a section group named "Test Section Group" and a section named "Test Section" in it.
    [TestFixture]
    [TestOf(typeof(OneNoteApplication))]
    public class OneNoteApplicationCreateItemsTest
    {
        private List<IOneNoteItem> itemsToDelete = new List<IOneNoteItem>();
        private static readonly Random random = new Random();

        private static class Parents
        {
            public static readonly OneNoteNotebook Notebook = OneNoteApplication.GetNotebooks().Single(n => n.Name == "Test Notebook");
            public static readonly OneNoteSectionGroup SectionGroup = Notebook.SectionGroups.Single(sg => sg.Name == "Test Section Group");
            public static readonly OneNoteSection Section = Notebook.Sections.Single(s => s.Name == "Test Section");
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OneNoteApplication.InitComObject();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OneNoteApplication.InitComObject();
            foreach (var item in itemsToDelete)
            {
                if (item is OneNoteNotebook notebook)
                {

                    OneNoteApplication.ComObject.CloseNotebook(notebook.ID, true);
                    Directory.Delete(notebook.Path, true);
                }
                else
                {
                    OneNoteApplication.ComObject.DeleteHierarchy(item.ID, deletePermanently: true);
                }
            }
            itemsToDelete.Clear();
            OneNoteApplication.ReleaseComObject();
        }

        private static string GenerateName(IReadOnlyList<char> invalidChars = null)
        {
            var name = Guid.NewGuid().ToString();
            if (invalidChars == null)
            {
                return name;
            }

            for (var i = 0; i < 5; i++)
            {
                var index = random.Next(invalidChars.Count);
                var invalidChar = invalidChars[index];
                index = random.Next(name.Length);
                name = name.Insert(index, invalidChar.ToString());
            }

            return name;
        }

        private static TestCaseData CreateTestCaseData<T>(Type newItemType, T parent, Func<T, string, bool, string> createItem, bool validName) where T : IOneNoteItem
        {
            string newItemName;
            Constraint expectedThrow;
            Constraint expectedBool;
            if (validName)
            {
                newItemName = GenerateName();
                expectedThrow = Throws.Nothing;
                expectedBool = Is.True;
            }
            else
            {
                IReadOnlyList<char> invalidChars = newItemType.Name switch
                {
                    nameof(OneNoteNotebook) => OneNoteApplication.InvalidNotebookChars,
                    nameof(OneNoteSectionGroup) => OneNoteApplication.InvalidSectionGroupChars,
                    nameof(OneNoteSection) => OneNoteApplication.InvalidSectionChars,
                };

                newItemName = GenerateName(invalidChars);
                expectedThrow = Throws.ArgumentException;
                expectedBool = Is.False;
            }

            var testData = new TestDataWrapper
            {
                CreateItem = () => createItem(parent, newItemName, false),
                NewItemType = newItemType,
                NewItemName = newItemName,
                ExpectedThrow = expectedThrow,
                ExpectedBool = expectedBool
            };
            var extra = parent != null ? $" in a {typeof(T).Name.Replace("OneNote", "")}" : "";
            return new TestCaseData(testData).SetArgDisplayNames($"{newItemType.Name.Replace("OneNote", "")}" + extra, "Valid Name: " + validName);
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            for (var i = 0; i < 2; i++)
            {
                yield return CreateTestCaseData(typeof(OneNoteSection), Parents.Notebook, OneNoteApplication.CreateSection, i != 0);
                yield return CreateTestCaseData(typeof(OneNoteSection), Parents.SectionGroup, OneNoteApplication.CreateSection, i != 0);
                yield return CreateTestCaseData(typeof(OneNoteSectionGroup), Parents.Notebook, OneNoteApplication.CreateSectionGroup, i != 0);
                yield return CreateTestCaseData(typeof(OneNoteSectionGroup), Parents.SectionGroup, OneNoteApplication.CreateSectionGroup, i != 0);
                yield return CreateTestCaseData<OneNoteNotebook>(typeof(OneNoteNotebook), null, (_, name, _) => OneNoteApplication.CreateNotebook(name, false), i != 0);
            }
            yield return CreateTestCaseData(typeof(OneNotePage), Parents.Section, OneNoteApplication.CreatePage, true);
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void CreateItem(TestDataWrapper testData)
        {
            Assert.Multiple(() =>
                {
                    Assert.That(() => testData.CreateItem(), testData.ExpectedThrow);
                    Assert.That(() => CheckItemIExist(testData), testData.ExpectedBool);
                }
            );
        }

        private bool CheckItemIExist(TestDataWrapper testData)
        {
            IOneNoteItem item;
            try
            {
                item = OneNoteApplication.GetNotebooks().Traverse().Single(i => i.Name == testData.NewItemName);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            if (item.GetType() != testData.NewItemType)
            {
                return false;
            }

            itemsToDelete.Add(item);
            return true;
        }

        public class TestDataWrapper
        {
            public Type NewItemType { get; init; }
            public string NewItemName { get; init; }
            public Action CreateItem { get; init; }
            public Constraint ExpectedThrow { get; init; }
            public Constraint ExpectedBool { get; init; }
        }
    }
}
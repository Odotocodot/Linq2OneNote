using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestOf(typeof(OneNoteApplication))]
	public class OneNoteApplicationCreateItemsTest
	{
		private List<IOneNoteItem> itemsToDelete = new List<IOneNoteItem>();
		private static readonly Random random = new Random();
		private const int CreateNameTestCount = 3;

		private static IEnumerable<TestCaseData> CreateNotebookTestCases() =>
			CreateItemTestCases(OneNoteApplication.InvalidNotebookChars);

		private static IEnumerable<TestCaseData> CreateSectionTestCases() =>
			CreateItemTestCases(OneNoteApplication.InvalidSectionChars);

		private static IEnumerable<TestCaseData> CreateSectionGroupTestCases() =>
			CreateItemTestCases(OneNoteApplication.InvalidSectionGroupChars);
		

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
		private static IEnumerable<TestCaseData> CreateItemTestCases(IReadOnlyList<char> invalidChars)
		{
			for (var i = 0; i < CreateNameTestCount; i++)
			{
				yield return new TestCaseData(GenerateName(), Throws.Nothing, Is.True).SetDescription("Valid Name Test");
			}

			for (var i = 0; i < CreateNameTestCount; i++)
			{
				yield return new TestCaseData(GenerateName(invalidChars), Throws.ArgumentException, Is.False).SetDescription("Invalid Name Test");
			}
		}
		private bool CheckItemIExist<T>(string name) where T : IOneNoteItem
		{
			IOneNoteItem item;
			try
			{
				item = OneNoteApplication.GetNotebooks().Traverse().Single(i => i.Name == name);
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			if (item is not T)
			{
				return false;
			}

			itemsToDelete.Add(item);
			return true;
		}



		[Test]
		[TestCaseSource(nameof(CreateNotebookTestCases))]
		public void CreateNotebook(string name, Constraint expectedThrow, Constraint expectedReturn)
		{
			Assert.Multiple(() =>
				{
					Assert.That(() => OneNoteApplication.CreateNotebook(name, false), expectedThrow);
					Assert.That(() => CheckItemIExist<OneNoteNotebook>(name), expectedReturn);
				}
			);
		}

		[Test]
		[TestCaseSource(nameof(CreateSectionTestCases))]
		public void CreateSection(string name, Constraint expectedThrow, Constraint expectedReturn)
		{
			var notebook = OneNoteApplication.GetNotebooks().Single(n => n.Name == "Test Notebook");

			Assert.Multiple(() =>
				{
					Assert.That(() => OneNoteApplication.CreateSection(notebook, name, false), expectedThrow);
					Assert.That(() => CheckItemIExist<OneNoteSection>(name), expectedReturn);
				}
			);
		}

		[Test]
		[TestCaseSource(nameof(CreateSectionGroupTestCases))]
		public void CreateSectionGroup(string name, Constraint expectedThrow, Constraint expectedReturn)
		{
			var notebook = OneNoteApplication.GetNotebooks().Single(n => n.Name == "Test Notebook");

			Assert.Multiple(() =>
				{
					Assert.That(() => OneNoteApplication.CreateSectionGroup(notebook, name, false), expectedThrow);
					Assert.That(() => CheckItemIExist<OneNoteSectionGroup>(name), expectedReturn);
				}
			);
		}

		[Test]
		public void CreatePage()
		{
			var notebook = OneNoteApplication.GetNotebooks().Single(n => n.Name == "Test Notebook");
			var section = notebook.Sections.Single(s => s.Name == "Section 2");
			var name = GenerateName();
			OneNoteApplication.CreatePage(section, name, false);
			Assert.IsTrue(CheckItemIExist<OneNotePage>(name));
		}
	}
}
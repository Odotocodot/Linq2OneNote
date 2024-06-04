using System;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	public abstract class BaseParserTest<T> where T : IOneNoteItem
	{
		protected string id;
		protected string name;
		protected bool isUnread;
		protected DateTime lastModified;
		protected string relativePath;
		protected IOneNoteItem parent;
		protected OneNoteNotebook notebook;

		protected IOneNoteItem item;
		protected T TypedItem => (T)item;

		[OneTimeSetUp]
		public abstract void OneTimeSetUp();

		[OneTimeTearDown]
		public void OneTimeTearDown() => OneNoteApplication.ReleaseComObject();
		
		[Test]
		public void TypeCheck() => Assert.IsInstanceOf<T>(item);

		[Test]
		public void IdCheck() => Assert.AreEqual(id, item.ID);

		[Test]
		public void NameCheck() => Assert.AreEqual(name, item.Name);
		
		[Test]
		public void IsUnreadCheck() => Assert.AreEqual(isUnread, item.IsUnread);
		
		[Test]
		public void LastModifiedCheck() => Assert.AreEqual(lastModified, item.LastModified);
		
		[Test]
		public void RelativePathCheck() => Assert.AreEqual(relativePath, item.RelativePath);
		
		[Test]
		public void ParentCheck() => Assert.AreSame(parent, item.Parent);
		
		[Test]
		public void NotebookCheck() => Assert.AreSame(notebook, item.Notebook);
	}
}
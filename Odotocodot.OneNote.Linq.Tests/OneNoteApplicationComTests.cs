using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	[TestOf(typeof(OneNoteApplication))]
	public class OneNoteApplicationComTests
	{
		[SetUp]
		public void Setup()
		{
			OneNoteApplication.ReleaseComObject();
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			OneNoteApplication.ReleaseComObject();
		}
		
		[Test]
		public void ReleaseComObject_WhenInit_DoesNotThrowException()
		{
			OneNoteApplication.InitComObject();
			Assert.DoesNotThrow(OneNoteApplication.ReleaseComObject);
		}

		[Test]
		public void HasComObject_WhenNotInit_ReturnsFalse()
		{
            Assert.That(OneNoteApplication.HasComObject, Is.False);
		}

		[Test]
		public void HasComObject_WhenInit_ReturnsTrue()
		{
			OneNoteApplication.InitComObject();
            Assert.That(OneNoteApplication.HasComObject, Is.True);
		}

		[Test]
		public void HasComObject_WhenInitAndRelease_ReturnsTrue()
		{
			OneNoteApplication.InitComObject();
			OneNoteApplication.ReleaseComObject();
            Assert.That(OneNoteApplication.HasComObject, Is.False);
		}
	}
}
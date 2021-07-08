using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Notes;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Notes
{
	public class NoteListTest
	{
		[Fact]
		public void ShouldCreateNoteList()
		{
			var noteList = new NoteList();

			var customerServiceMock = new StrictMock<ICustomerService>();
			var noteServiceMock = new StrictMock<INoteService>();

			var noteListCustom = new NoteList(customerServiceMock.Object, noteServiceMock.Object);

			Assert.NotNull(noteList);
			Assert.NotNull(noteListCustom);
		}

		[Fact]
		public void ShouldLoadCustomerWithNotes()
		{
			// Given
			var customerId = 5;
			var expectedCustomer = new Customer()
			{
				CustomerId = customerId,
				Notes = new() { new() { Content = "a" } }
			};


			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Get(customerId, false, true))
				.Returns(expectedCustomer);

			var noteServiceMock = new StrictMock<INoteService>();

			var noteList = new NoteList(customerServiceMock.Object, noteServiceMock.Object);

			// When
			var customer = noteList.LoadCustomerWithNotes(customerId);

			// Then
			Assert.Equal(expectedCustomer, customer);
			customerServiceMock.Verify(s => s.Get(customerId, false, true), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldDeleteNote(bool foundAndDeletedNote)
		{
			// Given
			var noteId = 5;

			var noteServiceMock = new StrictMock<INoteService>();
			noteServiceMock.Setup(s => s.Delete(noteId)).Returns(foundAndDeletedNote);

			var customerServiceMock = new StrictMock<ICustomerService>();

			var noteList = new NoteList(customerServiceMock.Object, noteServiceMock.Object);

			// When
			noteList.DeleteNote(noteId);

			// Then
			noteServiceMock.Verify(s => s.Delete(noteId), Times.Once);
		}
	}
}

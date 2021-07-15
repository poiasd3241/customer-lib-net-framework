using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Notes;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Notes
{
	public class NoteEditTest
	{
		[Fact]
		public void ShouldCreateNoteEdit()
		{
			var noteEdit = new NoteEdit();

			var customerServiceMock = new Mock<ICustomerService>();
			var noteServiceMock = new Mock<INoteService>();

			var noteEditCustom = new NoteEdit(customerServiceMock.Object, noteServiceMock.Object);

			Assert.NotNull(noteEdit);
			Assert.NotNull(noteEditCustom);
		}

		[Theory]
		[InlineData(5, true)]
		[InlineData(8, false)]
		public void ShouldCheckIfCustomerExists(int customerId, bool exists)
		{
			// Given
			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Exists(customerId)).Returns(exists);

			var noteServiceMock = new StrictMock<INoteService>();

			var noteEdit = new NoteEdit(customerServiceMock.Object, noteServiceMock.Object);

			// When
			var actualExists = noteEdit.CheckCustomerExists(customerId);

			// Then
			Assert.Equal(exists, actualExists);
			customerServiceMock.Verify(s => s.Exists(customerId), Times.Once);
		}

		private class LoadNoteData : TheoryData<int, Note, bool>
		{
			public LoadNoteData()
			{
				Add(5, new(), true);
				Add(8, null, false);
			}
		}

		[Theory]
		[ClassData(typeof(LoadNoteData))]
		public void ShouldLoadNote(int noteId, Note note, bool isNotNull)
		{
			// Given
			var noteServiceMock = new StrictMock<INoteService>();
			noteServiceMock.Setup(s => s.Get(noteId)).Returns(note);

			var customerServiceMock = new StrictMock<ICustomerService>();

			var noteEdit = new NoteEdit(customerServiceMock.Object, noteServiceMock.Object);

			// When
			var loadedNotNull = noteEdit.LoadNote(noteId);

			// Then
			Assert.Equal(isNotNull, loadedNotNull);
			Assert.Equal(note, noteEdit.Note);
			noteServiceMock.Verify(s => s.Get(noteId), Times.Once);
		}

		[Fact]
		public void ShouldLoadCustomer()
		{
			// Given
			var customerId = 5;
			var expectedCustomer = new Customer();

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Get(customerId, false, false))
				.Returns(expectedCustomer);

			var noteServiceMock = new Mock<INoteService>(MockBehavior.Strict);

			var noteEdit = new NoteEdit(customerServiceMock.Object, noteServiceMock.Object);

			// When
			var customer = noteEdit.LoadCustomer(customerId);

			// Then
			Assert.Equal(expectedCustomer, customer);
			customerServiceMock.Verify(s => s.Get(customerId, false, false), Times.Once);
		}

		[Fact]
		public void ShouldCreateNote()
		{
			// Given
			var note = new Note();

			var customerServiceMock = new StrictMock<ICustomerService>();

			var noteServiceMock = new Mock<INoteService>(MockBehavior.Strict);
			noteServiceMock.Setup(s => s.Save(note)).Returns(true);

			var noteEdit = new NoteEdit(customerServiceMock.Object, noteServiceMock.Object)
			{
				IsCreate = true,
				Note = note
			};

			// When
			noteEdit.CreateNote();

			// Then
			noteServiceMock.Verify(s => s.Save(note), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldUpdateNote(bool foundAndUpdatedNote)
		{
			// Given
			var note = new Note();

			var customerServiceMock = new StrictMock<ICustomerService>();

			var noteServiceMock = new Mock<INoteService>(MockBehavior.Strict);
			noteServiceMock.Setup(s => s.Update(note)).Returns(foundAndUpdatedNote);

			var noteEdit = new NoteEdit(customerServiceMock.Object, noteServiceMock.Object)
			{
				IsCreate = false,
				Note = note
			};

			// When
			noteEdit.SaveExistingNote();

			// Then
			noteServiceMock.Verify(s => s.Update(note), Times.Once);
		}

		[Theory]
		[InlineData("", null, false)]
		[InlineData(null, null, false)]
		[InlineData(" ", " ", false)]
		[InlineData("a", "a", true)]
		public void ShouldValidateNote(string input, string expectedContent, bool isValidExpected)
		{
			// Given
			var noteEdit = new NoteEdit() { Note = new() { Content = "q" } };

			Assert.NotEqual(expectedContent, noteEdit.Note.Content);

			// When
			var isValid = noteEdit.ValidateNote(input).IsValid;

			// Then
			Assert.Equal(isValidExpected, isValid);
			Assert.Equal(expectedContent, noteEdit.Note.Content);
		}
	}
}

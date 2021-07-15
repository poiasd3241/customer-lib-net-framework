using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Notes;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Notes
{
	public class NoteEditModelTest
	{
		[Fact]
		public void ShouldCreateNoteEditModelDefaultConstructor()
		{
			var model = new NoteEditModel();

			Assert.Null(model.Title);
			Assert.Null(model.SubmitButtonText);
			Assert.Null(model.Note);
		}

		[Fact]
		public void ShouldCreateNoteEditModelFromNote()
		{
			// Given
			var note = MockNote();

			var model = new NoteEditModel(note);

			Assert.Equal(note, model.Note);
			Assert.Null(model.Title);
			Assert.Null(model.SubmitButtonText);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var note = MockNote();
			var title = "t";
			var submitButtonText = "s";

			// When
			var model = new NoteEditModel()
			{
				Note = note,
				Title = title,
				SubmitButtonText = submitButtonText
			};

			// Then
			Assert.Equal(note, model.Note);
			Assert.Equal(title, model.Title);
			Assert.Equal(submitButtonText, model.SubmitButtonText);
		}

		private Note MockNote() => new()
		{
			NoteId = 5,
			CustomerId = 8,
			Content = "text"
		};
	}
}

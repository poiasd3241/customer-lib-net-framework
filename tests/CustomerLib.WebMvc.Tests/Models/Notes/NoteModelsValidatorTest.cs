using CustomerLib.Business.Entities;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Models.Notes;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Notes
{
	public class NoteModelsValidatorTest
	{
		[Fact]
		public void ShouldCreateNoteModelsValidator()
		{
			var validator = new NoteModelsValidator();

			Assert.NotNull(validator);
		}

		[Fact]
		public void ShouldValidateEditModel()
		{
			// Given
			var note = new Note()
			{
				Content = "text"
			};

			var model = new NoteEditModel(note);

			var validator = new NoteModelsValidator();

			// When
			var result = validator.ValidateEditModel(model);

			// Then
			Assert.Empty(result);
		}

		[Fact]
		public void ShouldInvalidateEditModel()
		{
			// Given
			var note = new Note()
			{
				Content = null
			};

			var model = new NoteEditModel(note);

			var validator = new NoteModelsValidator();

			// When
			var result = validator.ValidateEditModel(model);

			// Then
			var error = Assert.Single(result);

			Assert.Equal("Note.Content", error.Key);
			Assert.False(string.IsNullOrEmpty(error.Value));

		}
	}
}

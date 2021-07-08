using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;
using Xunit;
namespace CustomerLib.Business.Tests.Validators
{
	public class NoteValidatorTest
	{
		#region Private members

		private static readonly NoteValidator _noteValidator = new();

		#endregion

		#region Valid

		[Fact]
		public void ShouldValidateNote()
		{
			var result = _noteValidator.Validate(NoteValidatorFixture.MockNote());

			Assert.True(result.IsValid);
		}

		#endregion

		#region Invalid

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public void ShouldInvalidateNoteByContentNullEmptyOrWhitespace(string content)
		{
			// Given
			var note = new Note() { Content = content };

			// When
			var errors = _noteValidator.Validate(note).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Note cannot be empty or whitespace.", errors[0].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateNoteByContentTooLong()
		{
			// Given
			var note = new Note() { Content = new('a', 1001) };

			// When
			var errors = _noteValidator.Validate(note).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Note: max 1000 characters.", errors[0].ErrorMessage);
		}

		#endregion
	}

	public class NoteValidatorFixture
	{
		/// <returns>The mocked note with valid properties 
		/// (according to <see cref="NoteValidator"/>).</returns>
		public static Note MockNote() => new()
		{
			Content = "text"
		};
	}
}

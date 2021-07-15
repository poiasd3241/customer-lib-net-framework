using CustomerLib.Business.Entities;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Models.Notes;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Notes
{
	public class NoteModelsMapperTest
	{
		[Fact]
		public void ShouldMapDetailsModelToEntity()
		{
			// Given
			var note = MockNote();

			var model = new NoteEditModel(note);

			var mapper = new NoteModelsMapper();

			// When
			var entity = mapper.ToEntity(model);

			// Then
			Assert.True(note.EqualsByValue(entity));
		}

		private Note MockNote() => new()
		{
			NoteId = 5,
			CustomerId = 8,
			Content = "text"
		};
	}
}

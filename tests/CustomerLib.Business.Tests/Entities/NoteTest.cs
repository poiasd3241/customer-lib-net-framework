using CustomerLib.Business.Entities;
using Xunit;

namespace CustomerLib.Business.Tests.Entities
{
	public class NoteTest
	{
		[Fact]
		public void ShouldCreateNote()
		{
			Note note = new();

			Assert.Equal(0, note.NoteId);
			Assert.Equal(0, note.CustomerId);
			Assert.Null(note.Content);
		}

		[Fact]
		public void ShouldSetAddressProperties()
		{
			Note note = new();

			note.NoteId = 1;
			note.CustomerId = 1;
			note.Content = "a";

			Assert.Equal(1, note.NoteId);
			Assert.Equal(1, note.CustomerId);
			Assert.Equal("a", note.Content);
		}
	}
}

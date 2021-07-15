using CustomerLib.Business.Entities;
using CustomerLib.Data.Repositories.Implementations;
using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class NoteRepositoryTest
	{
		[Fact]
		public void ShouldCreateNoteRepository()
		{
			var repo = new NoteRepository();

			Assert.NotNull(repo);
		}

		#region Exists

		[Theory]
		[InlineData(2)]
		[InlineData(3)]
		public void ShouldCheckIfNoteExistsById(int noteId)
		{
			// Given
			var noteRepository = new NoteRepository();
			NoteRepositoryFixture.CreateMockNote(amount: 2);

			// When
			var exists = noteRepository.Exists(noteId);

			// Then
			if (noteId == 2)
			{
				Assert.True(exists);
			}
			if (noteId == 3)
			{
				Assert.False(exists);
			}
		}

		#endregion

		[Fact]
		public void ShouldCreateNote()
		{
			// Given
			var noteRepository = new NoteRepository();
			CustomerRepositoryFixture.CreateMockCustomer();
			NoteRepository.DeleteAll();

			var note = NoteRepositoryFixture.MockNote();
			note.CustomerId = 1;

			// When, Then
			noteRepository.Create(note);
		}

		[Fact]
		public void ShouldReadNoteNotFound()
		{
			// Given
			var noteRepository = new NoteRepository();
			NoteRepository.DeleteAll();

			// When
			var readNote = noteRepository.Read(1);

			// Then
			Assert.Null(readNote);
		}

		[Fact]
		public void ShouldReadNote()
		{
			// Given
			var noteRepository = new NoteRepository();
			var note = NoteRepositoryFixture.CreateMockNote();

			// When
			var createdNote = noteRepository.Read(1);

			// Then
			Assert.NotNull(createdNote);
			Assert.Equal(note.CustomerId, createdNote.CustomerId);
			Assert.Equal(note.Content, createdNote.Content);
		}

		[Fact]
		public void ShouldReadAllNotesByCustomer()
		{
			// Given
			var noteRepository = new NoteRepository();
			var note = NoteRepositoryFixture.CreateMockNote(2);

			// When
			var readNotes = noteRepository.ReadByCustomer(note.CustomerId);

			// Then
			Assert.NotNull(readNotes);
			Assert.Equal(2, readNotes.Count);

			foreach (var readNote in readNotes)
			{
				Assert.Equal(note.CustomerId, readNote.CustomerId);
				Assert.Equal(note.Content, readNote.Content);
			}
		}

		[Fact]
		public void ShouldReadAllNotesByCustomerNotFound()
		{
			// Given
			var noteRepository = new NoteRepository();
			NoteRepository.DeleteAll();

			// When
			var readNotes = noteRepository.ReadByCustomer(1);

			// Then
			Assert.Empty(readNotes);
		}

		[Fact]
		public void ShouldUpdateNote()
		{
			// Given
			var noteRepository = new NoteRepository();
			var note = NoteRepositoryFixture.CreateMockNote();

			var createdNote = noteRepository.Read(1);
			createdNote.Content = "New content!";

			// When
			noteRepository.Update(createdNote);

			var updatedNote = noteRepository.Read(1);

			// Then
			Assert.NotNull(updatedNote);
			Assert.Equal(note.CustomerId, updatedNote.CustomerId);
			Assert.Equal("New content!", updatedNote.Content);
		}

		[Fact]
		public void ShouldDeleteNote()
		{
			// Given
			var noteRepository = new NoteRepository();
			NoteRepositoryFixture.CreateMockNote();

			var createdNote = noteRepository.Read(1);
			Assert.NotNull(createdNote);

			// When
			noteRepository.Delete(1);

			// Then
			var deletedNote = noteRepository.Read(1);
			Assert.Null(deletedNote);
		}
	}

	public class NoteRepositoryFixture
	{
		/// <summary>
		/// Clears the Notes table, then creates the specified amount of mocked notes
		/// with repo-relevant valid properties, <see cref="Note.CustomerId"/> = 1.
		/// </summary>
		/// <param name="amount">The amount of notes to create.</param>
		/// <returns>The mocked note with repo-relevant valid properties, 
		/// <see cref="Note.CustomerId"/> = 1.</returns>
		public static Note CreateMockNote(int amount = 1)
		{
			var noteRepository = new NoteRepository();
			CustomerRepositoryFixture.CreateMockCustomer();
			NoteRepository.DeleteAll();

			var note = MockNote();
			note.CustomerId = 1;

			for (int i = 0; i < amount; i++)
			{
				noteRepository.Create(note);
			}

			return note;
		}

		/// <returns>The mocked note with repo-relevant valid properties.</returns>
		public static Note MockNote() => new()
		{
			Content = "text"
		};
	}
}

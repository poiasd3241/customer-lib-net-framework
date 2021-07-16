using CustomerLib.Business.Entities;
using CustomerLib.Data.Repositories.EF;
using Xunit;
using static CustomerLib.Data.IntegrationTests.Repositories.EF.CustomerRepositoryTest;

namespace CustomerLib.Data.IntegrationTests.Repositories.EF
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class NoteRepositoryTest
	{
		[Fact]
		public void ShouldCreateNoteRepositoryDefaultConstructor()
		{
			var repo = new NoteRepository();

			Assert.NotNull(repo);
		}

		[Fact]
		public void ShouldCreateNoteRepository()
		{
			var context = new CustomerLibDataContext();

			var repo = new NoteRepository(context);

			Assert.NotNull(repo);
		}

		[Theory]
		[InlineData(2, true)]
		[InlineData(3, false)]
		public void ShouldCheckIfNoteExistsById(int noteId, bool expectedExists)
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			NoteRepositoryFixture.CreateMockNote(amount: 2);

			// When
			var exists = repo.Exists(noteId);

			// Then
			Assert.Equal(expectedExists, exists);
		}

		[Fact]
		public void ShouldCreateNote()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();

			var note = NoteRepositoryFixture.MockNote();
			note.CustomerId = 1;

			// When
			var createdId = repo.Create(note);

			// Then
			Assert.Equal(1, createdId);
		}

		[Fact]
		public void ShouldReadNoteNotFound()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();

			// When
			var readNote = repo.Read(1);

			// Then
			Assert.Null(readNote);
		}

		[Fact]
		public void ShouldReadNote()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			var note = NoteRepositoryFixture.CreateMockNote();

			// When
			var readNote = repo.Read(1);

			// Then
			Assert.Equal(1, readNote.NoteId);
			Assert.Equal(note.CustomerId, readNote.CustomerId);
			Assert.Equal(note.Content, readNote.Content);
		}

		[Fact]
		public void ShouldReadAllNotesByCustomer()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			var note = NoteRepositoryFixture.CreateMockNote(2);

			// When
			var readNotes = repo.ReadByCustomer(note.CustomerId);

			// Then
			Assert.Equal(2, readNotes.Count);

			foreach (var readNote in readNotes)
			{
				Assert.Equal(note.CustomerId, readNote.CustomerId);
				Assert.Equal(note.Content, readNote.Content);
			}
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(81723)]
		public void ShouldReadAllNotesByCustomerBothNotFoundAndEmpty(int customerId)
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();

			// When
			var readNotes = repo.ReadByCustomer(customerId);

			// Then
			Assert.Empty(readNotes);
		}

		[Fact]
		public void ShouldUpdateNote()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			var note = NoteRepositoryFixture.CreateMockNote();

			var createdNote = repo.Read(1);
			createdNote.Content = "New content!";

			// When
			repo.Update(createdNote);

			var updatedNote = repo.Read(1);

			// Then
			Assert.Equal(1, createdNote.NoteId);
			Assert.Equal(createdNote.NoteId, updatedNote.NoteId);

			Assert.Equal(note.CustomerId, updatedNote.CustomerId);
			Assert.Equal("New content!", updatedNote.Content);
		}

		[Fact]
		public void ShouldDeleteNote()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			NoteRepositoryFixture.CreateMockNote();

			var createdNote = repo.Read(1);
			Assert.NotNull(createdNote);

			// When
			repo.Delete(1);

			// Then
			var deletedNote = repo.Read(1);
			Assert.Null(deletedNote);
		}

		[Fact]
		public void ShouldDeleteNotesByCustomerId()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			NoteRepositoryFixture.CreateMockNote(2);

			var createdNotes = repo.ReadByCustomer(1);
			Assert.Equal(2, createdNotes.Count);

			// When
			repo.DeleteByCustomer(1);

			// Then
			var deletedNotes = repo.ReadByCustomer(1);
			Assert.Empty(deletedNotes);
		}

		[Fact]
		public void ShouldDeleteAllNotes()
		{
			// Given
			var repo = NoteRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			NoteRepositoryFixture.CreateMockNote(2);

			var createdNotes = repo.ReadByCustomer(1);
			Assert.Equal(2, createdNotes.Count);

			// When
			repo.DeleteAll();

			// Then
			var deletedNotes = repo.ReadByCustomer(1);
			Assert.Empty(deletedNotes);
		}

		public class NoteRepositoryFixture
		{
			/// <summary>
			/// Creates the empty repository, containing a single customer
			/// with <see cref="Customer.CustomerId"/> = 1 and no notes.
			/// </summary>
			/// <returns>The empty note repository.</returns>
			public static NoteRepository CreateEmptyRepositoryWithCustomer()
			{
				CustomerRepositoryFixture.CreateMockCustomer();

				return new();
			}

			/// <summary>
			/// Creates the specified amount of mocked notes
			/// with repo-relevant valid properties, <see cref="Note.CustomerId"/> = 1.
			/// </summary>
			/// <param name="amount">The amount of notes to create.</param>
			/// <returns>The mocked note with repo-relevant valid properties, 
			/// <see cref="Note.CustomerId"/> = 1.</returns>
			public static Note CreateMockNote(int amount = 1)
			{
				var repo = new NoteRepository();

				var note = MockNote();
				note.CustomerId = 1;

				for (int i = 0; i < amount; i++)
				{
					repo.Create(note);
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
}

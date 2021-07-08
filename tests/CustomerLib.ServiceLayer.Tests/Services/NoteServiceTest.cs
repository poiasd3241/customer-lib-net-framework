using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Data.Repositories;
using CustomerLib.ServiceLayer.Services.Implementations;
using Moq;
using Xunit;

namespace CustomerLib.ServiceLayer.Tests.Services
{
	public class NoteServiceTest
	{
		[Fact]
		public void ShouldCreateNoteService()
		{
			var noteService = new NoteService();

			Assert.NotNull(noteService);
		}

		#region Exists

		public class ExistsByIdData : TheoryData<int, bool>
		{
			public ExistsByIdData()
			{
				Add(1, true);
				Add(2, false);
			}
		}

		[Theory]
		[ClassData(typeof(ExistsByIdData))]
		public void ShouldCheckIfNoteExistsById(int noteId, bool existsExpected)
		{
			// Given
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			noteRepoMock.Setup(r => r.Exists(noteId)).Returns(existsExpected);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var exists = service.Exists(noteId);

			// Then
			Assert.Equal(existsExpected, exists);
			noteRepoMock.Verify(r => r.Exists(noteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnCheckIfNoteExistsByBadId()
		{
			// Given
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Exists(0));

			// Then
			Assert.Equal("noteId", exception.ParamName);
		}

		#endregion

		#region Save

		[Fact]
		public void ShouldSave()
		{
			// Given
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedNote = NoteServiceFixture.MockNote();
			noteRepoMock.Setup(r => r.Create(expectedNote)).Returns(5);
			var service = new NoteService(noteRepoMock.Object);

			// When
			service.Save(expectedNote);

			// Then
			noteRepoMock.Verify(r => r.Create(expectedNote), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnSaveByBadNote()
		{
			var service = NoteServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Save(new Note()));
		}

		#endregion

		#region Get by Id

		[Fact]
		public void ShouldGetNoteById()
		{
			// Given
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedNote = NoteServiceFixture.MockNote();
			noteRepoMock.Setup(r => r.Read(1)).Returns(expectedNote);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var note = service.Get(1);

			// Then
			Assert.Equal(expectedNote, note);
			noteRepoMock.Verify(r => r.Read(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetNoteByBadId()
		{
			// Given
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Get(0));

			// Then
			Assert.Equal("noteId", exception.ParamName);
		}

		#endregion

		#region Find by customer Id

		[Fact]
		public void ShouldFindByCustomerId()
		{
			// Given
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedNotes = NoteServiceFixture.MockNotes();
			noteRepoMock.Setup(r => r.ReadByCustomer(1)).Returns(expectedNotes);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var notes = service.FindByCustomer(1);

			// Then
			Assert.Equal(expectedNotes, notes);
			noteRepoMock.Verify(r => r.ReadByCustomer(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnFindByCustomerByBadId()
		{
			// Given
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.FindByCustomer(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		#endregion

		#region Update

		[Fact]
		public void ShouldUpdateExistingNote()
		{
			// Given
			var note = NoteServiceFixture.MockNote();
			note.NoteId = 5;

			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			noteRepoMock.Setup(r => r.Exists(note.NoteId)).Returns(true);
			noteRepoMock.Setup(r => r.Update(note));
			var service = new NoteService(noteRepoMock.Object);

			// When
			var result = service.Update(note);

			// Then
			Assert.True(result);
			noteRepoMock.Verify(r => r.Exists(note.NoteId), Times.Once);
			noteRepoMock.Verify(r => r.Update(note), Times.Once);
		}

		[Fact]
		public void ShouldNotUpdateNotFoundNote()
		{
			// Given
			var note = NoteServiceFixture.MockNote();
			note.NoteId = 5;

			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			noteRepoMock.Setup(r => r.Exists(note.NoteId)).Returns(false);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var result = service.Update(note);

			// Then
			Assert.False(result);
			noteRepoMock.Verify(r => r.Exists(note.NoteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadNote()
		{
			var service = NoteServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Note()));
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDeleteExistingNote()
		{
			// Given
			var noteId = 5;

			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			noteRepoMock.Setup(r => r.Exists(noteId)).Returns(true);
			noteRepoMock.Setup(r => r.Delete(noteId));
			var service = new NoteService(noteRepoMock.Object);

			// When
			var result = service.Delete(noteId);

			// Then
			Assert.True(result);
			noteRepoMock.Verify(r => r.Exists(noteId), Times.Once);
			noteRepoMock.Verify(r => r.Delete(noteId), Times.Once);
		}

		[Fact]
		public void ShouldNotDeleteNotFoundNote()
		{
			// Given
			var noteId = 5;

			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			noteRepoMock.Setup(r => r.Exists(noteId)).Returns(false);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var result = service.Delete(noteId);

			// Then
			Assert.False(result);
			noteRepoMock.Verify(r => r.Exists(noteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Given
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Delete(0));

			// Then
			Assert.Equal("noteId", exception.ParamName);
		}

		#endregion
	}

	public class NoteServiceFixture
	{
		/// <returns>The mocked note with repo-relevant valid properties.</returns>
		public static Note MockNote() => new()
		{
			Content = "text"
		};

		/// <returns>The list containing 2 mocked notes with repo-relevant properties.</returns>
		public static List<Note> MockNotes() => new() { MockNote(), MockNote() };

		public static Mock<INoteRepository> MockNoteRepository() => new(MockBehavior.Strict);

		public static NoteService MockService() => new(MockNoteRepository().Object);
	}
}

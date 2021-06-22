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

		#region Save

		[Fact]
		public void ShouldSave()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedNote = NoteServiceFixture.MockNote();
			noteRepoMock.Setup(r => r.Create(expectedNote));
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

		[Fact]
		public void ShouldRethrowOnSave()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedException = new Exception("oops");
			var note = NoteServiceFixture.MockNote();
			noteRepoMock.Setup(r => r.Create(note)).Throws(expectedException);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Save(note));

			// Then
			Assert.Equal(expectedException, exception);
			noteRepoMock.Verify(r => r.Create(note), Times.Once);
		}

		#endregion

		#region Get by Id

		[Fact]
		public void ShouldGetNoteById()
		{
			// Then
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
			// Then
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Get(0));

			// Then
			Assert.Equal("noteId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnGetNoteById()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedException = new Exception("oops");
			noteRepoMock.Setup(r => r.Read(1)).Throws(expectedException);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Get(1));

			// Then
			Assert.Equal(expectedException, exception);
			noteRepoMock.Verify(r => r.Read(1), Times.Once);
		}

		#endregion

		#region Find by customer Id

		[Fact]
		public void ShouldFindByCustomerId()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedNotes = NoteServiceFixture.MockNotes();
			noteRepoMock.Setup(r => r.ReadAllByCustomer(1)).Returns(expectedNotes);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var notes = service.FindByCustomer(1);

			// Then
			Assert.Equal(expectedNotes, notes);
			noteRepoMock.Verify(r => r.ReadAllByCustomer(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnFindByCustomerByBadId()
		{
			// Then
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.FindByCustomer(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnFindByCustomer()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedException = new Exception("oops");
			noteRepoMock.Setup(r => r.ReadAllByCustomer(1)).Throws(expectedException);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.FindByCustomer(1));

			// Then
			Assert.Equal(expectedException, exception);
			noteRepoMock.Verify(r => r.ReadAllByCustomer(1), Times.Once);
		}

		#endregion

		#region Update

		[Fact]
		public void ShouldUpdate()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedNote = NoteServiceFixture.MockNote();
			noteRepoMock.Setup(r => r.Update(expectedNote));
			var service = new NoteService(noteRepoMock.Object);

			// When
			service.Update(expectedNote);

			// Then
			noteRepoMock.Verify(r => r.Update(expectedNote), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadNote()
		{
			var service = NoteServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Note()));
		}

		[Fact]
		public void ShouldRethrowOnUpdate()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedException = new Exception("oops");
			var note = NoteServiceFixture.MockNote();
			noteRepoMock.Setup(r => r.Update(note)).Throws(expectedException);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Update(note));

			// Then
			Assert.Equal(expectedException, exception);
			noteRepoMock.Verify(r => r.Update(note), Times.Once);
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDelete()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			noteRepoMock.Setup(r => r.Delete(1));
			var service = new NoteService(noteRepoMock.Object);

			// When
			service.Delete(1);

			// Then
			noteRepoMock.Verify(r => r.Delete(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Then
			var service = NoteServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Delete(0));

			// Then
			Assert.Equal("noteId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnDelete()
		{
			// Then
			var noteRepoMock = NoteServiceFixture.MockNoteRepository();
			var expectedException = new Exception("oops");
			noteRepoMock.Setup(r => r.Delete(1)).Throws(expectedException);
			var service = new NoteService(noteRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Delete(1));

			// Then
			Assert.Equal(expectedException, exception);
			noteRepoMock.Verify(r => r.Delete(1), Times.Once);
		}

		#endregion
	}

	public class NoteServiceFixture
	{
		public static Note MockNote() => new()
		{
			Content = "text"
		};

		public static List<Note> MockNotes() => new() { MockNote(), MockNote() };

		public static Mock<INoteRepository> MockNoteRepository() => new(MockBehavior.Strict);

		public static NoteService MockService() => new(MockNoteRepository().Object);
	}
}

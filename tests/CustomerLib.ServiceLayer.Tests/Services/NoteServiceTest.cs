using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Data.Repositories;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.TestHelpers;
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
			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.Exists(noteId)).Returns(existsExpected);

			var service = fixture.CreateService();

			// When
			var exists = service.Exists(noteId);

			// Then
			Assert.Equal(existsExpected, exists);
			fixture.MockNoteRepository.Verify(r => r.Exists(noteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnCheckIfNoteExistsByBadId()
		{
			// Given
			var service = new NoteServiceFixture().CreateService();

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
			var note = NoteServiceFixture.MockNote();
			var customerId = 5;
			note.CustomerId = customerId;

			var fixture = new NoteServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(true);
			fixture.MockNoteRepository.Setup(r => r.Create(note)).Returns(8);

			var service = fixture.CreateService();

			// When
			var result = service.Save(note);

			// Then
			Assert.True(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
			fixture.MockNoteRepository.Verify(r => r.Create(note), Times.Once);
		}

		[Fact]
		public void ShouldNotSaveByCustomerNotFound()
		{
			// Given
			var note = NoteServiceFixture.MockNote();
			var customerId = 5;
			note.CustomerId = customerId;

			var fixture = new NoteServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Save(note);

			// Then
			Assert.False(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnSaveByBadNote()
		{
			var service = new NoteServiceFixture().CreateService();

			Assert.Throws<EntityValidationException>(() => service.Save(new Note()));
		}

		#endregion

		#region Get by Id

		[Fact]
		public void ShouldGetNoteById()
		{
			// Given
			var noteId = 5;
			var expectedNote = NoteServiceFixture.MockNote();

			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.Read(noteId)).Returns(expectedNote);

			var service = fixture.CreateService();

			// When
			var note = service.Get(noteId);

			// Then
			Assert.Equal(expectedNote, note);
			fixture.MockNoteRepository.Verify(r => r.Read(noteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetNoteByBadId()
		{
			// Given
			var service = new NoteServiceFixture().CreateService();

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
			var customerId = 5;
			var expectedNotes = NoteServiceFixture.MockNotes();

			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.ReadByCustomer(customerId))
				.Returns(expectedNotes);

			var service = fixture.CreateService();

			// When
			var notes = service.FindByCustomer(customerId);

			// Then
			Assert.Equal(expectedNotes, notes);
			fixture.MockNoteRepository.Verify(r => r.ReadByCustomer(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnFindByCustomerByBadId()
		{
			// Given
			var service = new NoteServiceFixture().CreateService();

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
			var noteId = 5;
			note.NoteId = noteId;

			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.Exists(noteId)).Returns(true);
			fixture.MockNoteRepository.Setup(r => r.Update(note));

			var service = fixture.CreateService();

			// When
			var result = service.Update(note);

			// Then
			Assert.True(result);
			fixture.MockNoteRepository.Verify(r => r.Exists(noteId), Times.Once);
			fixture.MockNoteRepository.Verify(r => r.Update(note), Times.Once);
		}

		[Fact]
		public void ShouldNotUpdateNotFoundNote()
		{
			// Given
			var note = NoteServiceFixture.MockNote();
			var noteId = 5;
			note.NoteId = noteId;

			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.Exists(noteId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Update(note);

			// Then
			Assert.False(result);
			fixture.MockNoteRepository.Verify(r => r.Exists(noteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadNote()
		{
			var service = new NoteServiceFixture().CreateService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Note()));
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDeleteExistingNote()
		{
			// Given
			var noteId = 5;

			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.Exists(noteId)).Returns(true);
			fixture.MockNoteRepository.Setup(r => r.Delete(noteId));

			var service = fixture.CreateService();

			// When
			var result = service.Delete(noteId);

			// Then
			Assert.True(result);
			fixture.MockNoteRepository.Verify(r => r.Exists(noteId), Times.Once);
			fixture.MockNoteRepository.Verify(r => r.Delete(noteId), Times.Once);
		}

		[Fact]
		public void ShouldNotDeleteNotFoundNote()
		{
			// Given
			var noteId = 5;

			var fixture = new NoteServiceFixture();
			fixture.MockNoteRepository.Setup(r => r.Exists(noteId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Delete(noteId);

			// Then
			Assert.False(result);
			fixture.MockNoteRepository.Verify(r => r.Exists(noteId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Given
			var service = new NoteServiceFixture().CreateService();

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

		public StrictMock<ICustomerRepository> MockCustomerRepository { get; set; }
		public StrictMock<INoteRepository> MockNoteRepository { get; set; }

		public NoteServiceFixture()
		{
			MockCustomerRepository = new();
			MockNoteRepository = new();
		}

		public NoteService CreateService() =>
			new(MockCustomerRepository.Object, MockNoteRepository.Object);
	}
}

using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Notes;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Notes
{
	public class NotesModelTest
	{
		[Fact]
		public void ShouldThrowOnCreateWithNullAdresses()
		{
			var exception = Assert.Throws<ArgumentException>(() => new NotesModel(null));

			Assert.Equal("notes", exception.ParamName);
		}

		[Fact]
		public void ShouldCreateNotesModelFromNotes()
		{
			// Given
			var notes = MockNotes();

			// When
			var model = new NotesModel(notes);

			// Then
			Assert.Null(model.Title);
			Assert.Equal(notes, model.Notes);
			Assert.True(model.HasNotes);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given, When
			var notes = MockNotes();
			var model = new NotesModel(notes) { Title = "t" };

			// Then
			Assert.Equal("t", model.Title);
		}

		private class HasNotesData : TheoryData<List<Note>, bool>
		{
			public HasNotesData()
			{
				Add(new(), false);
				Add(MockNotes(), true);
			}
		}

		[Theory]
		[ClassData(typeof(HasNotesData))]
		public void ShouldCheckIfHasNotes(List<Note> notes, bool hasNotes)
		{
			// Given, When
			var model = new NotesModel(notes);

			// Then
			Assert.Equal(hasNotes, model.HasNotes);
		}

		private static List<Note> MockNotes() => new()
		{
			new(),
			new()
		};
	}
}

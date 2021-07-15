using System;
using System.Collections.Generic;
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

		#region Equals by value

		[Fact]
		public void ShouldThrowOnEqualsByValueByBadObjectType()
		{
			// Given
			var note1 = new Note();
			var whatever = "whatever";

			// When
			var exception = Assert.Throws<ArgumentException>(() => note1.EqualsByValue(whatever));

			// Then
			Assert.Equal("Must use the same entity type for comparison", exception.Message);
		}

		[Fact]
		public void ShouldConfirmEqualsByValue()
		{
			// Given
			var note1 = MockNote();
			var note2 = MockNote();

			// When
			var equalsByValue = note1.EqualsByValue(note2);

			// Then
			Assert.True(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByNull()
		{
			// Given
			var note1 = MockNote();
			Note note2 = null;

			// When
			var equalsByValue = note1.EqualsByValue(note2);

			// Then
			Assert.False(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByNoteId()
		{
			// Given
			var noteId1 = 5;
			var noteId2 = 7;

			var note1 = MockNote();
			var note2 = MockNote();

			note1.NoteId = noteId1;
			note2.NoteId = noteId2;

			// When
			var equalsByValue = note1.EqualsByValue(note2);

			// Then
			Assert.False(equalsByValue);
		}

		#endregion

		#region Lists equal by value

		private class NullAndNotNullListsData : TheoryData<List<Note>, List<Note>>
		{
			public NullAndNotNullListsData()
			{
				Add(null, new());
				Add(new(), null);
			}
		}

		[Theory]
		[ClassData(typeof(NullAndNotNullListsData))]
		public void ShouldRefuteListsEqualByValueByOneListNull(
			List<Note> list1, List<Note> list2)
		{
			// When
			var equalByValue = Note.ListsEqualByValues(list1, list2);

			// Then
			Assert.False(equalByValue);
		}

		[Fact]
		public void ShouldRefuteListsEqualByValueByCountMismatch()
		{
			// Given
			var list1 = new List<Note>();
			var list2 = new List<Note>() { new() };

			// When
			var equalByValue = Note.ListsEqualByValues(list1, list2);

			// Then
			Assert.False(equalByValue);
		}

		[Fact]
		public void ShouldConfirmListsEqualByValueByBothNull()
		{
			// Given
			List<Note> list1 = null;
			List<Note> list2 = null;

			// When
			var equalByValue = Note.ListsEqualByValues(list1, list2);

			// Then
			Assert.True(equalByValue);
		}

		private class NotNullEqualListsData : TheoryData<List<Note>, List<Note>>
		{
			public NotNullEqualListsData()
			{
				Add(new(), new());

				Add(new() { null }, new() { null });
				Add(new() { MockNote() }, new() { MockNote() });
			}
		}

		[Theory]
		[ClassData(typeof(NotNullEqualListsData))]
		public void ShouldConfirmListsEqualNotNull(List<Note> list1, List<Note> list2)
		{
			// When
			var equalByValue = Note.ListsEqualByValues(list1, list2);

			// Then
			Assert.True(equalByValue);
		}

		private class NotNullNotEqualListsData : TheoryData<List<Note>, List<Note>>
		{
			public NotNullNotEqualListsData()
			{
				Add(new() { null }, new() { MockNote() });
				Add(new() { MockNote() }, new() { null });

				var noteId1 = 5;
				var noteId2 = 7;

				var note1 = MockNote();
				var note2 = MockNote();

				note1.NoteId = noteId1;
				note2.NoteId = noteId2;

				Add(new() { note1 }, new() { note2 });
			}
		}

		[Theory]
		[ClassData(typeof(NotNullNotEqualListsData))]
		public void ShouldRefuteListsEqualNotNull(List<Note> list1, List<Note> list2)
		{
			// When
			var equalByValue = Note.ListsEqualByValues(list1, list2);

			// Then
			Assert.False(equalByValue);
		}

		#endregion

		private static Note MockNote() => new()
		{
			NoteId = 5,
			CustomerId = 8,
			Content = "text",
		};
	}
}

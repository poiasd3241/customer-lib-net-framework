using System;
using System.Collections.Generic;

namespace CustomerLib.Business.Entities
{
	[Serializable]
	public class Note : Entity
	{
		public int NoteId { get; set; }
		public int CustomerId { get; set; }
		public string Content { get; set; }

		public override bool EqualsByValue(object noteToCompareTo)
		{
			if (noteToCompareTo is null)
			{
				return false;
			}

			EnsureSameEntityType(noteToCompareTo);
			var note = (Note)noteToCompareTo;

			return
				NoteId == note.NoteId &&
				CustomerId == note.CustomerId &&
				Content == note.Content;
		}

		public static bool ListsEqualByValues(IEnumerable<Note> list1, IEnumerable<Note> list2) =>
			EntitiesHelper.ListsEqualByValues(list1, list2);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Notes
{
	public class NotesModel
	{
		public string Title { get; set; }
		public IEnumerable<Note> Notes { get; }
		public bool HasNotes => Notes.Count() > 0;

		public NotesModel(IEnumerable<Note> notes)
		{
			if (notes is null)
			{
				throw new ArgumentException("The notes cannot be null", nameof(notes));
			}

			Notes = notes;
		}
	}
}

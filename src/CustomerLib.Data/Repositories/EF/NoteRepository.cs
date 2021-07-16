using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories.EF
{
	public class NoteRepository : INoteRepository
	{
		private readonly CustomerLibDataContext _context;

		public NoteRepository(CustomerLibDataContext context)
		{
			_context = context;
		}

		public NoteRepository()
		{
			_context = new();
		}

		public bool Exists(int noteId)
		{
			var note = _context.Notes.Find(noteId);

			return note is not null;
		}

		public int Create(Note note)
		{
			var created = _context.Notes.Add(note);

			_context.SaveChanges();

			return created.NoteId;
		}

		public Note Read(int noteId)
		{
			var note = _context.Notes.Find(noteId);

			return note;
		}

		public IReadOnlyCollection<Note> ReadByCustomer(int customerId)
		{
			var notes = _context.Notes
				.Where(note => note.CustomerId == customerId)
				.ToArray();

			return notes;
		}

		public void Update(Note note)
		{
			var found = _context.Notes.Find(note.NoteId);

			if (found is not null)
			{
				_context.Entry(found).CurrentValues.SetValues(note);

				_context.SaveChanges();
			}
		}

		public void Delete(int noteId)
		{
			var found = _context.Notes.Find(noteId);

			if (found is not null)
			{
				_context.Notes.Remove(found);

				_context.SaveChanges();
			}
		}

		public void DeleteByCustomer(int customerId)
		{
			var notes = _context.Notes
				.Where(note => note.CustomerId == customerId)
				.ToArray();

			foreach (var note in notes)
			{
				_context.Notes.Remove(note);
			}

			_context.SaveChanges();
		}

		public void DeleteAll()
		{
			var notes = _context.Notes.ToArray();

			foreach (var note in notes)
			{
				_context.Notes.Remove(note);
			}

			_context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('dbo.Notes', RESEED, 0);");

			_context.SaveChanges();
		}
	}
}

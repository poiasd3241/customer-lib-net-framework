using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories
{
	public interface INoteRepository
	{
		bool Exists(int noteId);

		/// <returns>The Id of the created item.</returns>
		int Create(Note note);
		Note Read(int noteId);

		/// <returns>An empty collection if no notes found; otherwise, the found notes.</returns>
		IReadOnlyCollection<Note> ReadByCustomer(int customerId);
		void Update(Note note);
		void Delete(int noteId);
	}
}

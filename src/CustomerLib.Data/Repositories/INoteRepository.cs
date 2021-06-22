using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories
{
	public interface INoteRepository
	{
		void Create(Note note);
		Note Read(int noteId);
		List<Note> ReadAllByCustomer(int customerId);
		void Update(Note note);
		void Delete(int noteId);
	}
}

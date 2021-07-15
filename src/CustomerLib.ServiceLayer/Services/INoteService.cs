using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface INoteService
	{
		bool Exists(int noteId);
		bool Save(Note note);
		Note Get(int noteId);

		/// <returns>An empty collection if no notes found; 
		/// otherwise, the found notes.</returns>
		IReadOnlyCollection<Note> FindByCustomer(int customerId);
		bool Update(Note note);
		bool Delete(int noteId);
	}
}

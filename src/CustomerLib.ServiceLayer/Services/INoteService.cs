using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface INoteService
	{
		bool Exists(int noteId);
		void Save(Note note);
		Note Get(int noteId);
		IReadOnlyCollection<Note> FindByCustomer(int customerId);
		bool Update(Note note);
		bool Delete(int noteId);
	}
}

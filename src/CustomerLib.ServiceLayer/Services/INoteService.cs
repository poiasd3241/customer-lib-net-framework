using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface INoteService
	{
		void Save(Note note);
		Note Get(int noteId);
		List<Note> FindByCustomer(int customerId);
		void Update(Note note);
		void Delete(int noteId);
	}
}

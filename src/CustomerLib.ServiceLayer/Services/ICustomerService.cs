using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface ICustomerService
	{
		bool Exists(int customerId);
		void Save(Customer customer);
		Customer Get(int customerId, bool includeAddresses, bool includeNotes);
		IReadOnlyCollection<Customer> GetAll(bool includeAddresses, bool includeNotes);
		int GetCount();
		IReadOnlyCollection<Customer> GetPage(int page, int pageSize,
			bool includeAddresses, bool includeNotes,
			bool checkTotalSame, int expectedTotal);
		bool Update(Customer customer);
		bool Delete(int customerId);
	}
}

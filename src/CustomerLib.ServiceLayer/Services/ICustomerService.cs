using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface ICustomerService
	{
		bool Exists(int customerId);
		void Save(Customer customer);
		Customer Get(int customerId, bool includeAddresses, bool includeNotes);

		/// <returns>An empty collection if no customers found; 
		/// otherwise, the found customers.</returns>
		IReadOnlyCollection<Customer> GetAll(bool includeAddresses, bool includeNotes);

		int GetCount();

		/// <returns>An empty collection if no customers found; 
		/// otherwise, the found customers.</returns>
		IReadOnlyCollection<Customer> GetPage(int page, int pageSize,
			bool includeAddresses, bool includeNotes,
			bool checkTotalSame, int expectedTotal);

		bool Update(Customer customer);
		bool Delete(int customerId);
	}
}

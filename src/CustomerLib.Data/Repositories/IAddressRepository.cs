using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories
{
	public interface IAddressRepository
	{
		bool Exists(int addressId);

		/// <returns>The Id of the created item.</returns>
		int Create(Address address);
		Address Read(int addressId);
		IReadOnlyCollection<Address> ReadByCustomer(int customerId);
		void Update(Address address);
		void Delete(int addressId);
		void DeleteByCustomer(int customerId);
	}
}

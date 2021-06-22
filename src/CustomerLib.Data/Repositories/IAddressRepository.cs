using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.Data.Repositories
{
	public interface IAddressRepository
	{
		void Create(Address address);
		Address Read(int addressId);
		List<Address> ReadAllByCustomer(int customerId);
		void Update(Address address);
		void Delete(int addressId);
		void DeleteByCustomer(int customerId);
	}
}

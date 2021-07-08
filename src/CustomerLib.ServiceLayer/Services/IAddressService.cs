using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface IAddressService
	{
		bool Exists(int addressId);
		void Save(Address address);
		Address Get(int addressId);
		IReadOnlyCollection<Address> FindByCustomer(int customerId);
		bool Update(Address address);
		bool Delete(int addressId);
		void DeleteByCustomer(int customerId);
	}
}

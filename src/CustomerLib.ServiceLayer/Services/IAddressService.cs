using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface IAddressService
	{
		void Save(Address address);
		Address Get(int addressId);
		List<Address> FindByCustomer(int customerId);
		void Update(Address address);
		void Delete(int addressId);
		void DeleteByCustomer(int customerId);
	}
}

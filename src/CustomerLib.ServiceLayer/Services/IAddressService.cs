using System.Collections.Generic;
using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface IAddressService
	{
		bool Exists(int addressId);
		bool Save(Address address);
		Address Get(int addressId);

		/// <returns>An empty collection if no addresses found; 
		/// otherwise, the found addresses.</returns>
		IReadOnlyCollection<Address> FindByCustomer(int customerId);
		bool Update(Address address);
		bool Delete(int addressId);
		void DeleteByCustomer(int customerId);
	}
}

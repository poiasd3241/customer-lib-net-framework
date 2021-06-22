using CustomerLib.Business.Entities;

namespace CustomerLib.ServiceLayer.Services
{
	public interface ICustomerService
	{
		void Save(Customer customer);
		Customer Get(int customerId);
		void Update(Customer customer);
		void Delete(int customerId);
	}
}

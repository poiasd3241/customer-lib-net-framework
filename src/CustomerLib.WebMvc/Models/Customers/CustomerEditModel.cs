using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Customers
{
	public class CustomerEditModel : CustomerBasicDetailsModel
	{
		public CustomerEditModel() { }
		public CustomerEditModel(Customer customer) : base(customer) { }
	}
}

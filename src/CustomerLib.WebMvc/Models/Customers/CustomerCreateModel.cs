using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Addresses;

namespace CustomerLib.WebMvc.Models.Customers
{
	public class CustomerCreateModel
	{
		public CustomerBasicDetailsModel BasicDetails { get; set; }
		public AddressDetailsModel AddressDetails { get; set; }
		public Note Note { get; set; } = new();

		public CustomerCreateModel() { }

		public CustomerCreateModel(Customer customer, Address address)
		{
			BasicDetails = new(customer);
			AddressDetails = new(address);
		}
	}
}

using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Addresses
{
	public class AddressDetailsModel
	{
		public Address Address { get; set; }

		public AddressDetailsModel(Address address)
		{
			Address = address;
		}
		public AddressDetailsModel() { }
	}
}

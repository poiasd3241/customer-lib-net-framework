using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Addresses
{
	public class AddressEditModel
	{
		public string Title { get; set; }
		public string SubmitButtonText { get; set; }
		public AddressDetailsModel AddressDetails { get; set; }

		public AddressEditModel() { }
		public AddressEditModel(Address address)
		{
			AddressDetails = new(address);
		}
	}
}

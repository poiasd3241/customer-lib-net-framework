using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Customers
{
	public class CustomerBasicDetailsModel
	{
		public int CustomerId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public string TotalPurchasesAmount { get; set; }

		public CustomerBasicDetailsModel(Customer customer)
		{
			CustomerId = customer.CustomerId;
			FirstName = customer.FirstName;
			LastName = customer.LastName;
			PhoneNumber = customer.PhoneNumber;
			Email = customer.Email;
			TotalPurchasesAmount = customer.TotalPurchasesAmount.ToString();
		}

		public CustomerBasicDetailsModel() { }
	}
}

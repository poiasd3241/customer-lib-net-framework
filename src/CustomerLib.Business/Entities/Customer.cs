using System.Collections.Generic;

namespace CustomerLib.Business.Entities
{
	public class Customer : Person
	{
		public int CustomerId { get; set; }
		public List<Address> Addresses { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public List<Note> Notes { get; set; }
		public decimal? TotalPurchasesAmount { get; set; }
	}
}

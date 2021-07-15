using System;
using System.Collections.Generic;

namespace CustomerLib.Business.Entities
{
	[Serializable]
	public class Customer : Person
	{
		public int CustomerId { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public decimal? TotalPurchasesAmount { get; set; }
		public List<Address> Addresses { get; set; }
		public List<Note> Notes { get; set; }

		public override bool EqualsByValue(object customerToCompareTo)
		{
			if (customerToCompareTo is null)
			{
				return false;
			}

			EnsureSameEntityType(customerToCompareTo);
			var customer = (Customer)customerToCompareTo;

			return
				CustomerId == customer.CustomerId &&
				PhoneNumber == customer.PhoneNumber &&
				Email == customer.Email &&
				TotalPurchasesAmount == customer.TotalPurchasesAmount &&
				Address.ListsEqualByValues(Addresses, customer.Addresses) &&
				Note.ListsEqualByValues(Notes, customer.Notes);
		}
	}
}

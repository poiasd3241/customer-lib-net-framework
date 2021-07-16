using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CustomerLib.Business.Enums;

namespace CustomerLib.Business.Entities
{
	[Serializable]
	public class Address : Entity
	{
		public int AddressId { get; set; }
		public int CustomerId { get; set; }
		public string AddressLine { get; set; }
		public string AddressLine2 { get; set; }

		[Column("AddressTypeId")]
		public AddressType Type { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
		public string State { get; set; }
		public string Country { get; set; }

		public override bool EqualsByValue(object addressToCompareTo)
		{
			if (addressToCompareTo is null)
			{
				return false;
			}

			EnsureSameEntityType(addressToCompareTo);
			var address = (Address)addressToCompareTo;

			return
				AddressId == address.AddressId &&
				CustomerId == address.CustomerId &&
				AddressLine == address.AddressLine &&
				AddressLine2 == address.AddressLine2 &&
				Type == address.Type &&
				City == address.City &&
				PostalCode == address.PostalCode &&
				State == address.State &&
				Country == address.Country;
		}

		public static bool ListsEqualByValues(
			IEnumerable<Address> list1, IEnumerable<Address> list2) =>
				EntitiesHelper.ListsEqualByValues(list1, list2);
	}
}

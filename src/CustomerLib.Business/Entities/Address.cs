using System;
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
		public AddressType Type { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
	}
}

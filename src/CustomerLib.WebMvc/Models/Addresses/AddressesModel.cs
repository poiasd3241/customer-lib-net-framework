using System;
using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;

namespace CustomerLib.WebMvc.Models.Addresses
{
	public class AddressesModel
	{
		public string Title { get; set; }
		public IEnumerable<Address> Addresses { get; }
		public bool HasAddresses => Addresses.Count() > 0;

		public AddressesModel(IEnumerable<Address> addresses)
		{
			if (addresses is null)
			{
				throw new ArgumentException("The addresses cannot be null", nameof(addresses));
			}

			Addresses = addresses;
		}
	}
}

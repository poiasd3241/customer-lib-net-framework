using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using Xunit;

namespace CustomerLib.Business.Tests.Entities
{
	public class AddressTest
	{
		[Fact]
		public void ShouldCreateAddress()
		{
			Address address = new();

			Assert.Equal(0, address.AddressId);
			Assert.Equal(0, address.CustomerId);
			Assert.Null(address.AddressLine);
			Assert.Null(address.AddressLine2);
			Assert.Equal(0, (int)address.Type);
			Assert.Null(address.City);
			Assert.Null(address.PostalCode);
			Assert.Null(address.State);
			Assert.Null(address.Country);
		}

		[Fact]
		public void ShouldSetAddressProperties()
		{
			var text = "a";
			var type = AddressType.Billing;

			Address address = new();

			address.AddressId = 1;
			address.CustomerId = 1;
			address.AddressLine = text;
			address.AddressLine2 = text;
			address.Type = type;
			address.City = text;
			address.PostalCode = text;
			address.State = text;
			address.Country = text;

			Assert.Equal(1, address.AddressId);
			Assert.Equal(1, address.CustomerId);
			Assert.Equal(text, address.AddressLine);
			Assert.Equal(text, address.AddressLine2);
			Assert.Equal(type, address.Type);
			Assert.Equal(text, address.City);
			Assert.Equal(text, address.PostalCode);
			Assert.Equal(text, address.State);
			Assert.Equal(text, address.Country);
		}
	}
}

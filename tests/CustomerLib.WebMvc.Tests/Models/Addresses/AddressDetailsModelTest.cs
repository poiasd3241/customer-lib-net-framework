using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Addresses;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Addresses
{
	public class AddressDetailsModelTest
	{
		[Fact]
		public void ShouldCreateAddressDetailsModelDefaultConstructor()
		{
			var model = new AddressDetailsModel();

			Assert.Null(model.Address);
		}

		[Fact]
		public void ShouldCreateAddressDetailsModelFromAddress()
		{
			// Given
			var address = MockAddress();

			var model = new AddressDetailsModel(address);

			Assert.Equal(address, model.Address);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var address = MockAddress();

			// When
			var model = new AddressDetailsModel()
			{
				Address = address,
			};

			// Then
			Assert.Equal(address, model.Address);
		}

		private Address MockAddress() => new()
		{
			AddressId = 5,
			CustomerId = 8,
			AddressLine = "1",
			AddressLine2 = "2",
			Type = Business.Enums.AddressType.Billing,
			City = "city x",
			PostalCode = "111",
			State = "state x",
			Country = "Canada"
		};
	}
}

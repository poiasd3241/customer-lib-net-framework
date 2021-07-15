using CustomerLib.Business.Entities;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Models.Addresses;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Addresses
{
	public class AddressModelsMapperTest
	{
		[Fact]
		public void ShouldMapDetailsModelToEntity()
		{
			// Given
			var address = MockAddress();

			var model = new AddressDetailsModel(address);

			var mapper = new AddressModelsMapper();

			// When
			var entity = mapper.ToEntity(model);

			// Then
			Assert.True(address.EqualsByValue(entity));
		}

		[Fact]
		public void ShouldMapEditModelToEntity()
		{
			// Given
			var address = MockAddress();

			var model = new AddressEditModel(address);

			var mapper = new AddressModelsMapper();

			// When
			var entity = mapper.ToEntity(model);

			// Then
			Assert.True(address.EqualsByValue(entity));
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

using System;
using System.Linq;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.WebMvc.Models.Addresses;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Addresses
{
	public class AddressModelsValidatorTest
	{
		[Fact]
		public void ShouldCreateAddressModelsValidator()
		{
			var validator = new AddressModelsValidator();

			Assert.NotNull(validator);
		}

		#region DetailsModel

		[Fact]
		public void ShouldValidateDetailsModel()
		{
			// Given
			var address = MockAddress();

			var model = new AddressDetailsModel(address);

			var validator = new AddressModelsValidator();

			// When
			var result = validator.ValidateDetailsModel(model);

			// Then
			Assert.Empty(result);
		}

		[Fact]
		public void ShouldInvalidateDetailsModel()
		{
			// Given
			var address = MockAddress();
			address.AddressLine = null;
			address.PostalCode = "   ";

			var model = new AddressDetailsModel(address);

			var validator = new AddressModelsValidator();

			// When
			var result = validator.ValidateDetailsModel(model);

			// Then
			Assert.Equal(2, result.Count);

			Assert.Equal("Address.AddressLine", result.Keys.ElementAt(0));
			Assert.Equal("Address.PostalCode", result.Keys.ElementAt(1));

			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(0)));
			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(1)));
		}

		[Theory]
		[ClassData(typeof(UnexpectedAddressDetailsData))]
		public void ShouldThrowOnValidatingUnexpectedDetailsModel(Address address)
		{
			// Given
			var model = new AddressDetailsModel(address);

			var validator = new AddressModelsValidator();

			// When
			var exception = Assert.Throws<Exception>(() =>
				validator.ValidateDetailsModel(model));

			// Then
			Assert.Equal("Unexpected model data.", exception.Message);
		}

		#endregion

		#region EditModel

		[Fact]
		public void ShouldValidateEditModel()
		{
			// Given
			var address = MockAddress();

			var model = new AddressEditModel(address);

			var validator = new AddressModelsValidator();

			// When
			var result = validator.ValidateEditModel(model);

			// Then
			Assert.Empty(result);
		}

		[Fact]
		public void ShouldInvalidateEditModel()
		{
			// Given
			var address = MockAddress();
			address.AddressLine = null;
			address.PostalCode = "   ";

			var model = new AddressEditModel(address);

			var validator = new AddressModelsValidator();

			// When
			var result = validator.ValidateEditModel(model);

			// Then
			Assert.Equal(2, result.Count);

			Assert.Equal("AddressDetails.Address.AddressLine", result.Keys.ElementAt(0));
			Assert.Equal("AddressDetails.Address.PostalCode", result.Keys.ElementAt(1));

			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(0)));
			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(1)));
		}

		[Theory]
		[ClassData(typeof(UnexpectedAddressDetailsData))]
		public void ShouldThrowOnValidatingUnexpectedEditModel(Address address)
		{
			// Given
			var model = new AddressEditModel(address);

			var validator = new AddressModelsValidator();

			// When
			var exception = Assert.Throws<Exception>(() =>
				validator.ValidateEditModel(model));

			// Then
			Assert.Equal("Unexpected model data.", exception.Message);
		}

		#endregion

		public class UnexpectedAddressDetailsData : TheoryData<Address>
		{
			public UnexpectedAddressDetailsData()
			{
				var badType = MockAddress();
				badType.Type = (AddressType)666;

				var badCountry = MockAddress();
				badCountry.Country = "Whatever";

				Add(badType);
				Add(badCountry);
			}
		}

		private static Address MockAddress() => new()
		{
			AddressLine = "one",
			Type = AddressType.Billing,
			City = "city x",
			PostalCode = "666",
			State = "state x",
			Country = "Canada"
		};
	}
}

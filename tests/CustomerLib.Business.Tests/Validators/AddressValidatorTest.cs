using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Validators;
using Xunit;

namespace CustomerLib.Business.Tests.Validators
{
	public class AddressValidatorTest
	{
		#region Private members

		private static readonly AddressValidator _addressValidator = new();

		#endregion

		#region Valid

		private class MockAddressData : TheoryData<Address>
		{
			public MockAddressData()
			{
				Add(AddressValidatorFixture.MockAddress());
				Add(AddressValidatorFixture.MockOptionalAddress());
			}
		}

		[Theory]
		[ClassData(typeof(MockAddressData))]
		public void ShouldValidateAddressIncludingNullOptionaProperties(Address address)
		{
			var result = _addressValidator.Validate(address);

			Assert.True(result.IsValid);
		}

		#endregion

		#region Invalid

		[Fact]
		public void ShouldInvalidateAddressByRequiredPropertiesNull()
		{
			// Given
			var address = AddressValidatorFixture.MockAddress();

			address.AddressLine = null;
			address.City = null;
			address.PostalCode = null;
			address.State = null;
			address.Country = null;

			// When
			var errors = _addressValidator.Validate(address).Errors;

			// Then
			Assert.Equal(5, errors.Count);
			Assert.Equal("Address line is required.", errors[0].ErrorMessage);
			Assert.Equal("City is required.", errors[1].ErrorMessage);
			Assert.Equal("Postal code is required.", errors[2].ErrorMessage);
			Assert.Equal("State is required.", errors[3].ErrorMessage);
			Assert.Equal("Country is required.", errors[4].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateAddressByWhitespace()
		{
			// Given
			var whitespace = " ";
			var address = AddressValidatorFixture.MockAddress();

			address.AddressLine = whitespace;
			address.AddressLine2 = whitespace;
			address.City = whitespace;
			address.PostalCode = whitespace;
			address.State = whitespace;
			address.Country = whitespace;

			// When
			var errors = _addressValidator.Validate(address).Errors;

			// Then
			Assert.Equal(6, errors.Count);
			Assert.Equal("Address line cannot be empty or whitespace.", errors[0].ErrorMessage);
			Assert.Equal("Address line2 cannot be empty or whitespace.", errors[1].ErrorMessage);
			Assert.Equal("City cannot be empty or whitespace.", errors[2].ErrorMessage);
			Assert.Equal("Postal code cannot be empty or whitespace.", errors[3].ErrorMessage);
			Assert.Equal("State cannot be empty or whitespace.", errors[4].ErrorMessage);
			Assert.Equal("Country cannot be empty or whitespace.", errors[5].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateAddressByLength()
		{
			// Given
			var address = AddressValidatorFixture.MockAddress();

			address.AddressLine = new('a', 101);
			address.AddressLine2 = new('a', 101);
			address.City = new('a', 51);
			address.PostalCode = new('a', 7);
			address.State = new('a', 21);

			// When
			var errors = _addressValidator.Validate(address).Errors;

			// Then
			Assert.Equal(5, errors.Count);
			Assert.Equal("Address line: max 100 characters.", errors[0].ErrorMessage);
			Assert.Equal("Address line2: max 100 characters.", errors[1].ErrorMessage);
			Assert.Equal("City: max 50 characters.", errors[2].ErrorMessage);
			Assert.Equal("Postal code: max 6 characters.", errors[3].ErrorMessage);
			Assert.Equal("State: max 20 characters.", errors[4].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateAddressByBadType()
		{
			// Given
			var address = AddressValidatorFixture.MockAddress();

			address.Type = (AddressType)666;

			// When
			var errors = _addressValidator.Validate(address).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Unknown type.", errors[0].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateAddressByAllowedCountry()
		{
			// Given
			var address = AddressValidatorFixture.MockAddress();

			address.Country = "Japan";

			// When
			var errors = _addressValidator.Validate(address).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Country: allowed only United States, Canada.", errors[0].ErrorMessage);
		}

		#endregion
	}

	public class AddressValidatorFixture
	{
		/// <returns>The mocked address with valid properties 
		/// (according to <see cref="AddressValidator"/>), optional properties not null.</returns>
		public static Address MockAddress() => new()
		{
			AddressLine = "line",
			AddressLine2 = "line2",
			Type = AddressType.Shipping,
			City = "city",
			PostalCode = "code",
			State = "state",
			Country = "United States"
		};

		/// <returns>The mocked address with valid properties 
		/// (according to <see cref="AddressValidator"/>), optional properties null.</returns>
		public static Address MockOptionalAddress() => new()
		{
			AddressLine = "line",
			AddressLine2 = null,
			Type = AddressType.Shipping,
			City = "city",
			PostalCode = "code",
			State = "state",
			Country = "United States"
		};
	}
}

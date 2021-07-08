using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;
using Xunit;

namespace CustomerLib.Business.Tests.Validators
{
	public class CustomerValidatorTest
	{
		#region Private members

		private static readonly CustomerValidator _customerValidator = new();

		#endregion

		#region Valid

		private class MockCustomerData : TheoryData<Customer>
		{
			public MockCustomerData()
			{
				Add(CustomerValidatorFixture.MockCustomer());
				Add(CustomerValidatorFixture.MockOptionalCustomer());
			}
		}

		[Theory]
		[ClassData(typeof(MockCustomerData))]
		public void ShouldValidateCustomerIncludingNullOptionaProperties(Customer customer)
		{
			var result = _customerValidator.Validate(customer);

			Assert.True(result.IsValid);
		}

		#endregion

		#region Invalid

		[Fact]
		public void ShouldInvalidateCustomerByRequiredTextPropertiesNull()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.LastName = null;

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Last name is required.", errors[0].ErrorMessage);
		}

		private class MockInvalidAddressesAndNotesData : TheoryData<List<Address>, List<Note>>
		{
			public MockInvalidAddressesAndNotesData()
			{
				Add(null, null);
				Add(new(), new());
			}
		}

		[Theory]
		[ClassData(typeof(MockInvalidAddressesAndNotesData))]
		public void ShouldInvalidateCustomerByRequiredListPropertiesNullOrEmpty(
			List<Address> addresses, List<Note> notes)
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Addresses = addresses;
			customer.Notes = notes;

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			Assert.Equal(2, errors.Count);
			Assert.Equal("At least one address is required.", errors[0].ErrorMessage);
			Assert.Equal("At least one note is required.", errors[1].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateCustomerByWhitespaceProperties()
		{
			// Given
			var whitespace = " ";
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.FirstName = whitespace;
			customer.LastName = whitespace;
			customer.PhoneNumber = whitespace;
			customer.Email = whitespace;

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			Assert.Equal(4, errors.Count);
			Assert.Equal("First name cannot be empty or whitespace.", errors[0].ErrorMessage);
			Assert.Equal("Last name cannot be empty or whitespace.", errors[1].ErrorMessage);
			Assert.Equal("Phone number cannot be empty or whitespace.", errors[2].ErrorMessage);
			Assert.Equal("Email cannot be empty or whitespace.", errors[3].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateCustomerByLength()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.FirstName = new('a', 51);
			customer.LastName = new('a', 51);

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			Assert.Equal(2, errors.Count);
			Assert.Equal("First name: max 50 characters.", errors[0].ErrorMessage);
			Assert.Equal("Last name: max 50 characters.", errors[1].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateCustomerByPhoneNumberFormat()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.PhoneNumber = "+012345";

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Phone number: must be in E.164 format.", errors[0].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateCustomerByEmailFormat()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Email = "@me@asd.com";

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("Invalid email.", errors[0].ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateCustomerByBadAddress()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Addresses[0].AddressLine = null;

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			var error = Assert.Single(errors);
			Assert.Equal("Address line is required.", error.ErrorMessage);
			Assert.Equal("Addresses[0].AddressLine", error.PropertyName);
		}

		[Fact]
		public void ShouldInvalidateCustomerByBadNote()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Notes[0].Content = "   ";

			// When
			var errors = _customerValidator.Validate(customer).Errors;

			// Then
			var error = Assert.Single(errors);
			Assert.Equal("Note cannot be empty or whitespace.", error.ErrorMessage);
			Assert.Equal("Notes[0].Content", error.PropertyName);
		}

		#endregion

		#region Validation without Addresses and Notes

		[Fact]
		public void ShouldValidateCustomerExcludingAddressesAndNotes()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Addresses = null;
			customer.Notes = null;

			// When
			var result = _customerValidator.ValidateWithoutAddressesAndNotes(customer);

			// Then
			Assert.True(result.IsValid);
		}


		[Fact]
		public void ShouldInvalidateCustomerExcludingAddressesAndNotes()
		{
			// Given
			var whitespace = " ";
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.FirstName = whitespace;
			customer.LastName = whitespace;
			customer.PhoneNumber = whitespace;
			customer.Email = whitespace;

			// When
			var errors = _customerValidator.ValidateWithoutAddressesAndNotes(customer).Errors;

			// Then
			Assert.Equal(4, errors.Count);
			Assert.Equal("First name cannot be empty or whitespace.", errors[0].ErrorMessage);
			Assert.Equal("Last name cannot be empty or whitespace.", errors[1].ErrorMessage);
			Assert.Equal("Phone number cannot be empty or whitespace.", errors[2].ErrorMessage);
			Assert.Equal("Email cannot be empty or whitespace.", errors[3].ErrorMessage);
		}

		#endregion
	}

	public class CustomerValidatorFixture
	{
		/// <returns>The mocked customer with valid properties 
		/// (according to <see cref="CustomerValidator"/>), optional properties not null.</returns>
		public static Customer MockCustomer(string email = "john@doe.com") => new()
		{
			FirstName = "a",
			LastName = "a",
			Addresses = new() { AddressValidatorFixture.MockAddress() },
			PhoneNumber = "+123",
			Email = email,
			Notes = new() { NoteValidatorFixture.MockNote() },
			TotalPurchasesAmount = 123,
		};

		/// <returns>The mocked customer with valid properties 
		/// (according to <see cref="CustomerValidator"/>), optional properties null.</returns>
		public static Customer MockOptionalCustomer() => new()
		{
			FirstName = null,
			LastName = "a",
			Addresses = new() { AddressValidatorFixture.MockAddress() },
			PhoneNumber = null,
			Email = null,
			Notes = new() { NoteValidatorFixture.MockNote() },
			TotalPurchasesAmount = null
		};
	}
}

using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;
using FluentValidation;
using Xunit;

namespace CustomerLib.Business.Tests.Validators
{
	public class CustomerValidatorTest
	{
		#region Private members

		private static readonly CustomerValidator _customerValidator = new();

		#endregion

		#region Invalid property - First name

		private class InvalidFirstNameData : TheoryData<string, string>
		{
			public InvalidFirstNameData()
			{
				Add("", "First name cannot be empty or whitespace.");
				Add(" ", "First name cannot be empty or whitespace.");
				Add(new('a', 51), "First name: max 50 characters.");
			}
		}

		[Theory]
		[ClassData(typeof(InvalidFirstNameData))]
		public void ShouldInvalidateByBadFirstName(string firstName, string errorMessage)
		{
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.FirstName = firstName;

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.FirstName))).Errors;

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Customer.FirstName), error.PropertyName);
			Assert.Equal(errorMessage, error.ErrorMessage);
		}

		#endregion

		#region Invalid property - Last name

		private class InvalidLastNameData : TheoryData<string, string>
		{
			public InvalidLastNameData()
			{
				Add(null, "Last name is required.");
				Add("", "Last name cannot be empty or whitespace.");
				Add(" ", "Last name cannot be empty or whitespace.");
				Add(new('a', 51), "Last name: max 50 characters.");
			}
		}

		[Theory]
		[ClassData(typeof(InvalidLastNameData))]
		public void ShouldInvalidateByBadLastName(string lastName, string errorMessage)
		{
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.LastName = lastName;

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.LastName))).Errors;

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Customer.LastName), error.PropertyName);
			Assert.Equal(errorMessage, error.ErrorMessage);
		}

		#endregion

		#region Invalid property - Phone number

		private class InvalidPhoneNumberData : TheoryData<string, string>
		{
			public InvalidPhoneNumberData()
			{
				Add("", "Phone number cannot be empty or whitespace.");
				Add(" ", "Phone number cannot be empty or whitespace.");
				Add("123456", "Phone number: must be in E.164 format.");
			}
		}

		[Theory]
		[ClassData(typeof(InvalidPhoneNumberData))]
		public void ShouldInvalidateByBadPhoneNumber(string phoneNumber, string errorMessage)
		{
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.PhoneNumber = phoneNumber;

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.PhoneNumber))).Errors;

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Customer.PhoneNumber), error.PropertyName);
			Assert.Equal(errorMessage, error.ErrorMessage);
		}

		#endregion

		#region Invalid property - Email

		private class InvalidEmailData : TheoryData<string, string>
		{
			public InvalidEmailData()
			{
				Add("", "Email cannot be empty or whitespace.");
				Add(" ", "Email cannot be empty or whitespace.");
				Add("a@a@a", "Invalid email.");
			}
		}

		[Theory]
		[ClassData(typeof(InvalidEmailData))]
		public void ShouldInvalidateByBadEmail(string email, string errorMessage)
		{
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Email = email;

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.Email))).Errors;

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Customer.Email), error.PropertyName);
			Assert.Equal(errorMessage, error.ErrorMessage);
		}

		#endregion

		#region Invalid property - Addresses

		private class InvalidAddressesData : TheoryData<List<Address>, string>
		{
			public InvalidAddressesData()
			{
				Add(null, "At least one address is required.");
				Add(new(), "At least one address is required.");
			}
		}

		[Theory]
		[ClassData(typeof(InvalidAddressesData))]
		public void ShouldInvalidateByBadAddresses(List<Address> addresses, string errorMessage)
		{
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Addresses = addresses;

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.Addresses))).Errors;

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Customer.Addresses), error.PropertyName);
			Assert.Equal(errorMessage, error.ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateByBadAddressContent()
		{
			var address = AddressValidatorFixture.MockAddress();
			address.AddressLine = null;
			address.State = "";
			var addressLineErrorMsg = "Address line is required.";
			var stateErrorMsg = "State cannot be empty or whitespace.";


			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Addresses = new() { address };

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.Addresses))).Errors;

			Assert.Equal(2, errors.Count);

			Assert.Equal($"{nameof(Customer.Addresses)}[0].{nameof(Address.AddressLine)}",
				errors[0].PropertyName);
			Assert.Equal($"{nameof(Customer.Addresses)}[0].{nameof(Address.State)}",
				errors[1].PropertyName);

			Assert.Equal(addressLineErrorMsg, errors[0].ErrorMessage);
			Assert.Equal(stateErrorMsg, errors[1].ErrorMessage);

		}

		#endregion

		#region Invalid property - Notes

		private class InvalidNotesData : TheoryData<List<Note>, string>
		{
			public InvalidNotesData()
			{
				Add(null, "At least one note is required.");
				Add(new(), "At least one note is required.");
			}
		}

		[Theory]
		[ClassData(typeof(InvalidNotesData))]
		public void ShouldInvalidateByBadNotes(List<Note> notes, string errorMessage)
		{
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Notes = notes;

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.Notes))).Errors;

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Customer.Notes), error.PropertyName);
			Assert.Equal(errorMessage, error.ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateByBadNoteContent()
		{
			var note = NoteValidatorFixture.MockNote();
			note.Content = null;
			var contentErrorMsg = "Note cannot be empty or whitespace.";

			var customer = CustomerValidatorFixture.MockCustomer();
			customer.Notes = new() { note };

			var errors = _customerValidator.Validate(customer, options =>
				options.IncludeProperties(nameof(Customer.Notes))).Errors;

			var error = Assert.Single(errors);

			Assert.Equal($"{nameof(Customer.Notes)}[0].{nameof(Note.Content)}", error.PropertyName);
			Assert.Equal(contentErrorMsg, error.ErrorMessage);
		}

		#endregion

		#region Own properties RuleSet

		[Fact]
		public void ShouldValidateCustomerOwnRuleSet()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();

			// Make non-Own properties invalid.
			customer.Addresses = null;
			customer.Notes = null;

			var resultOwnRuleSet = _customerValidator.Validate(
				customer, options => options.IncludeRuleSets("Own"));

			var allPropertiesErrors = _customerValidator.Validate(
				customer, options => options.IncludeAllRuleSets()).Errors;

			Assert.True(resultOwnRuleSet.IsValid);

			Assert.Equal(2, allPropertiesErrors.Count);
			Assert.Equal("Addresses", allPropertiesErrors[0].PropertyName);
			Assert.Equal("Notes", allPropertiesErrors[1].PropertyName);
		}

		[Fact]
		public void ShouldInvalidateCustomerOwnRuleSet()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();
			var whitespace = " ";
			customer.FirstName = whitespace;
			customer.LastName = whitespace;
			customer.PhoneNumber = whitespace;
			customer.Email = whitespace;

			// When
			var errors = _customerValidator.Validate(
				customer, options => options.IncludeRuleSets("Own")).Errors;

			// Then
			var errorPropertyNames = errors.Select(e => e.PropertyName);

			Assert.Equal(4, errorPropertyNames.Count());
			Assert.Contains(nameof(Customer.FirstName), errorPropertyNames);
			Assert.Contains(nameof(Customer.LastName), errorPropertyNames);
			Assert.Contains(nameof(Customer.PhoneNumber), errorPropertyNames);
			Assert.Contains(nameof(Customer.Email), errorPropertyNames);
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

		#region Full validation - all RuleSets

		[Fact]
		public void ShouldValidateFullCustomer()
		{
			// Given
			var customer = CustomerValidatorFixture.MockCustomer();

			// When
			var result = _customerValidator.ValidateFull(customer);

			// Then
			Assert.True(result.IsValid);
		}

		[Fact]
		public void ShouldValidateFullCustomerWithOptionalNullProperties()
		{
			// Given
			var customer = CustomerValidatorFixture.MockOptionalCustomer();

			// When
			var result = _customerValidator.ValidateFull(customer);

			// Then
			Assert.True(result.IsValid);
		}

		[Fact]
		public void ShouldInvalidateFullCustomer()
		{
			// Given
			var whitespace = " ";
			var customer = CustomerValidatorFixture.MockCustomer();
			customer.FirstName = whitespace;
			customer.LastName = whitespace;
			customer.PhoneNumber = whitespace;
			customer.Email = whitespace;

			customer.Addresses = null;
			customer.Notes = null;

			// When
			var errors = _customerValidator.ValidateFull(customer).Errors;

			// Then
			Assert.Equal(6, errors.Count);
			Assert.Equal("First name cannot be empty or whitespace.", errors[0].ErrorMessage);
			Assert.Equal("Last name cannot be empty or whitespace.", errors[1].ErrorMessage);
			Assert.Equal("Phone number cannot be empty or whitespace.", errors[2].ErrorMessage);
			Assert.Equal("Email cannot be empty or whitespace.", errors[3].ErrorMessage);
			Assert.Equal("At least one address is required.", errors[4].ErrorMessage);
			Assert.Equal("At least one note is required.", errors[5].ErrorMessage);
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

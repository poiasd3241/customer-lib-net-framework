using System;
using System.Linq;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Localization;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;
using static CustomerLib.WebMvc.Tests.Models.Addresses.AddressModelsValidatorTest;

namespace CustomerLib.WebMvc.Tests.Models.Customers
{
	public class CustomerModelsValidatorTest
	{
		[Fact]
		public void ShouldCreateCustomerModelsValidator()
		{
			var validator = new CustomerModelsValidator();

			Assert.NotNull(validator);
		}

		#region BasicDetailsModel

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("666")]
		public void ShouldValidateBasicDetailsModel(string totalPurchasesAmount)
		{
			// Given
			var customer = MockCustomer();

			var model = new CustomerBasicDetailsModel(customer)
			{
				TotalPurchasesAmount = totalPurchasesAmount
			};

			var validator = new CustomerModelsValidator();

			// When
			var result = validator.ValidateBasicDetailsModel(model);

			// Then
			Assert.Empty(result);
		}

		[Fact]
		public void ShouldInvalidateBasicDetailsModel()
		{
			// Given
			var customer = MockCustomer();
			customer.LastName = null;

			var model = new CustomerBasicDetailsModel(customer)
			{
				TotalPurchasesAmount = "bad"
			};

			var totalPurchasesAmountErrorMessage = "Total purchases amount must be a decimal number.\n*optional - clear the field if you don't have any value to enter.";

			var validator = new CustomerModelsValidator();

			// When
			var result = validator.ValidateBasicDetailsModel(model);

			// Then
			Assert.Equal(2, result.Count);

			Assert.Equal("TotalPurchasesAmount", result.Keys.ElementAt(0));
			Assert.Equal("LastName", result.Keys.ElementAt(1));

			Assert.Equal(totalPurchasesAmountErrorMessage, result.Values.ElementAt(0));
			Assert.Equal(ValidationRules.PERSON_LAST_NAME_REQUIRED, result.Values.ElementAt(1));
		}

		#endregion

		#region EditModel

		[Fact]
		public void ShouldValidateEditModel()
		{
			// Given
			var customer = MockCustomer();

			var model = new CustomerEditModel(customer);

			var validator = new CustomerModelsValidator();

			// When
			var result = validator.ValidateEditModel(model);

			// Then
			Assert.Empty(result);
		}

		[Fact]
		public void ShouldInvalidateEditModel()
		{
			// Given
			var customer = MockCustomer();
			customer.FirstName = " ";
			customer.LastName = null;

			var model = new CustomerEditModel(customer);

			var validator = new CustomerModelsValidator();

			// When
			var result = validator.ValidateEditModel(model);

			// Then
			Assert.Equal(2, result.Count);

			Assert.Equal("FirstName", result.Keys.ElementAt(0));
			Assert.Equal("LastName", result.Keys.ElementAt(1));

			Assert.Equal(ValidationRules.PERSON_FIRST_NAME_EMPTY_OR_WHITESPACE,
				result.Values.ElementAt(0));
			Assert.Equal(ValidationRules.PERSON_LAST_NAME_REQUIRED,
				result.Values.ElementAt(1));
		}

		#endregion

		#region CreateModel

		[Fact]
		public void ShouldValidateCreateModel()
		{
			// Given
			var customer = MockCustomer();
			var address = MockAddress();
			var note = MockNote();

			var model = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var validator = new CustomerModelsValidator();

			// When
			var result = validator.ValidateCreateModel(model);

			// Then
			Assert.Empty(result);
		}

		[Fact]
		public void ShouldInvalidateCreateModel()
		{
			// Given
			var customer = MockCustomer();
			customer.LastName = null;

			var address = MockAddress();
			address.City = "";

			var note = MockNote();
			note.Content = null;

			var model = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var validator = new CustomerModelsValidator();

			// When
			var result = validator.ValidateCreateModel(model);

			// Then
			Assert.Equal(3, result.Count);

			Assert.Equal("BasicDetails.LastName", result.Keys.ElementAt(0));
			Assert.Equal("AddressDetails.Address.City", result.Keys.ElementAt(1));
			Assert.Equal("Note.Content", result.Keys.ElementAt(2));

			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(0)));
			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(1)));
			Assert.False(string.IsNullOrEmpty(result.Values.ElementAt(2)));
		}

		[Theory]
		[ClassData(typeof(UnexpectedAddressDetailsData))]
		public void ShouldThrowOnValidateCreateModelByUnexpectedAddressDetails(Address address)
		{
			// Given
			var customer = MockCustomer();
			var note = MockNote();

			var model = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var validator = new CustomerModelsValidator();

			// When
			var exception = Assert.Throws<Exception>(() => validator.ValidateCreateModel(model));

			// Then
			Assert.Equal("Unexpected model data.", exception.Message);
		}

		#endregion

		private static Customer MockCustomer() => new()
		{
			FirstName = "one",
			LastName = "two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = null,
			Addresses = null,
			Notes = null
		};

		private static Address MockAddress() => new()
		{
			AddressLine = "one",
			Type = AddressType.Billing,
			City = "city x",
			PostalCode = "666",
			State = "state x",
			Country = "Canada"
		};

		private static Note MockNote() => new()
		{
			Content = "text"
		};
	}
}

using CustomerLib.Business.Entities;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Models.Addresses;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Customers
{
	public class CustomerCreateModelTest
	{
		[Fact]
		public void ShouldCreateCustomerCreateModelDefaultConstructor()
		{
			var model = new CustomerCreateModel();

			Assert.Null(model.BasicDetails);
			Assert.Null(model.AddressDetails);

			Assert.NotNull(model.Note);
			Assert.Null(model.Note.Content);
		}

		[Fact]
		public void ShouldCreateCustomerCreateModel()
		{
			// Given
			var customer = MockCustomer();
			var address = MockAddress();

			// When
			var model = new CustomerCreateModel(customer, address);

			// Then
			Assert.Equal(customer.CustomerId, model.BasicDetails.CustomerId);
			Assert.Equal(customer.FirstName, model.BasicDetails.FirstName);
			Assert.Equal(customer.LastName, model.BasicDetails.LastName);
			Assert.Equal(customer.PhoneNumber, model.BasicDetails.PhoneNumber);
			Assert.Equal(customer.Email, model.BasicDetails.Email);
			Assert.Equal(customer.TotalPurchasesAmount.ToString(),
				model.BasicDetails.TotalPurchasesAmount);

			Assert.Equal(address, model.AddressDetails.Address);

			Assert.NotNull(model.Note);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var note = new Note();

			var customerBasicDetailsModel = new CustomerBasicDetailsModel();
			var addressDetailsModel = new AddressDetailsModel();

			var model = new CustomerCreateModel();
			Assert.Null(model.BasicDetails);
			Assert.Null(model.AddressDetails);
			Assert.NotEqual(note, model.Note);

			// When
			model.BasicDetails = customerBasicDetailsModel;
			model.AddressDetails = addressDetailsModel;
			model.Note = note;

			// Then
			Assert.Equal(customerBasicDetailsModel, model.BasicDetails);
			Assert.Equal(addressDetailsModel, model.AddressDetails);

			Assert.Equal(note, model.Note);
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

		private Customer MockCustomer() => new()
		{
			CustomerId = 5,
			FirstName = "one",
			LastName = "two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = null,
			Addresses = null,
			Notes = null
		};
	}
}

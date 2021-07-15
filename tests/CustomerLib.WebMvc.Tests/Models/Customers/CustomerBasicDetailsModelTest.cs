using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Customers
{
	public class CustomerBasicDetailsModelTest
	{
		[Fact]
		public void ShouldCreateCustomerBasicDetailsModelDefaultConstructor()
		{
			var model = new CustomerBasicDetailsModel();

			Assert.Equal(0, model.CustomerId);
			Assert.Null(model.FirstName);
			Assert.Null(model.LastName);
			Assert.Null(model.PhoneNumber);
			Assert.Null(model.Email);
			Assert.Null(model.TotalPurchasesAmount);
		}

		[Fact]
		public void ShouldCreateCustomerBasicDetailsModelFromCustomer()
		{
			var customer = MockCustomer();

			var model = new CustomerBasicDetailsModel(customer);

			Assert.Equal(customer.CustomerId, model.CustomerId);
			Assert.Equal(customer.FirstName, model.FirstName);
			Assert.Equal(customer.LastName, model.LastName);
			Assert.Equal(customer.PhoneNumber, model.PhoneNumber);
			Assert.Equal(customer.Email, model.Email);
			Assert.Equal(customer.TotalPurchasesAmount.ToString(), model.TotalPurchasesAmount);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var customerId = 5;
			var firstName = "one";
			var lastName = "two";
			var phoneNumber = "+123";
			var email = "a@a.aa";
			var totalPurchasesAmount = "666";

			var model = new CustomerBasicDetailsModel(new Customer());

			Assert.NotEqual(customerId, model.CustomerId);
			Assert.NotEqual(firstName, model.FirstName);
			Assert.NotEqual(lastName, model.LastName);
			Assert.NotEqual(phoneNumber, model.PhoneNumber);
			Assert.NotEqual(email, model.Email);
			Assert.NotEqual(totalPurchasesAmount, model.TotalPurchasesAmount);

			// When
			model.CustomerId = customerId;
			model.FirstName = firstName;
			model.LastName = lastName;
			model.PhoneNumber = phoneNumber;
			model.Email = email;
			model.TotalPurchasesAmount = totalPurchasesAmount;

			// Then
			Assert.Equal(customerId, model.CustomerId);
			Assert.Equal(firstName, model.FirstName);
			Assert.Equal(lastName, model.LastName);
			Assert.Equal(phoneNumber, model.PhoneNumber);
			Assert.Equal(email, model.Email);
			Assert.Equal(totalPurchasesAmount, model.TotalPurchasesAmount);
		}

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

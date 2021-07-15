using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Customers
{
	public class CustomerEditModelTest
	{
		[Fact]
		public void ShouldCreateCustomerEditModelDefaultConstructor()
		{
			// Given
			var model = new CustomerEditModel();

			Assert.Equal(0, model.CustomerId);
			Assert.Null(model.FirstName);
			Assert.Null(model.LastName);
			Assert.Null(model.PhoneNumber);
			Assert.Null(model.Email);
			Assert.Null(model.TotalPurchasesAmount);
		}

		[Fact]
		public void ShouldCreateCustomerEditModelFromCustomer()
		{
			// Given
			var customer = MockCustomer();

			var model = new CustomerEditModel(customer);

			Assert.Equal(customer.CustomerId, model.CustomerId);
			Assert.Equal(customer.FirstName, model.FirstName);
			Assert.Equal(customer.LastName, model.LastName);
			Assert.Equal(customer.PhoneNumber, model.PhoneNumber);
			Assert.Equal(customer.Email, model.Email);
			Assert.Equal(customer.TotalPurchasesAmount.ToString(), model.TotalPurchasesAmount);
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

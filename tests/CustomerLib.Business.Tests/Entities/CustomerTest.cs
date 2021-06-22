using System.Collections.Generic;
using CustomerLib.Business.Entities;
using Xunit;

namespace CustomerLib.Business.Tests.Entities
{
	public class CustomerTest
	{
		[Fact]
		public void ShouldCreateCustomer()
		{
			Customer customer = new();

			Assert.Equal(0, customer.CustomerId);
			Assert.Null(customer.FirstName);
			Assert.Null(customer.LastName);
			Assert.Null(customer.Addresses);
			Assert.Null(customer.PhoneNumber);
			Assert.Null(customer.Email);
			Assert.Null(customer.Notes);
			Assert.Null(customer.TotalPurchasesAmount);
		}

		[Fact]
		public void ShouldSetCustomerProperties()
		{
			var text = "a";
			var addresses = new List<Address>();
			var notes = new List<Note>();
			var total = 2m;

			Customer customer = new();
			customer.CustomerId = 1;
			customer.FirstName = text;
			customer.LastName = text;
			customer.Addresses = addresses;
			customer.PhoneNumber = text;
			customer.Email = text;
			customer.Notes = notes;
			customer.TotalPurchasesAmount = total;

			Assert.Equal(1, customer.CustomerId);
			Assert.Equal(text, customer.FirstName);
			Assert.Equal(text, customer.LastName);
			Assert.Equal(addresses, customer.Addresses);
			Assert.Equal(text, customer.PhoneNumber);
			Assert.Equal(text, customer.Email);
			Assert.Equal(notes, customer.Notes);
			Assert.Equal(total, customer.TotalPurchasesAmount);
		}
	}
}

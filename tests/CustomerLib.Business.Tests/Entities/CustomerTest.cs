using System;
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
			// Given
			var text = "a";
			var addresses = new List<Address>();
			var notes = new List<Note>();
			var total = 2m;

			// When
			Customer customer = new();
			customer.CustomerId = 1;
			customer.FirstName = text;
			customer.LastName = text;
			customer.Addresses = addresses;
			customer.PhoneNumber = text;
			customer.Email = text;
			customer.Notes = notes;
			customer.TotalPurchasesAmount = total;

			// Then
			Assert.Equal(1, customer.CustomerId);
			Assert.Equal(text, customer.FirstName);
			Assert.Equal(text, customer.LastName);
			Assert.Equal(addresses, customer.Addresses);
			Assert.Equal(text, customer.PhoneNumber);
			Assert.Equal(text, customer.Email);
			Assert.Equal(notes, customer.Notes);
			Assert.Equal(total, customer.TotalPurchasesAmount);
		}

		#region Equals by value

		[Fact]
		public void ShouldThrowOnEqualsByValueByBadObjectType()
		{
			// Given
			var customer1 = new Customer();
			var whatever = "whatever";

			// When
			var exception = Assert.Throws<ArgumentException>(() =>
				customer1.EqualsByValue(whatever));

			// Then
			Assert.Equal("Must use the same entity type for comparison", exception.Message);
		}

		[Fact]
		public void ShouldConfirmEqualsByValue()
		{
			// Given
			var customer1 = MockCustomer();
			var customer2 = MockCustomer();

			// When
			var equalsByValue = customer1.EqualsByValue(customer2);

			// Then
			Assert.True(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByNull()
		{
			// Given
			var customer1 = MockCustomer();
			Customer customer2 = null;

			// When
			var equalsByValue = customer1.EqualsByValue(customer2);

			// Then
			Assert.False(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByCustomerId()
		{
			// Given
			var customerId1 = 5;
			var customerId2 = 7;

			var customer1 = MockCustomer();
			var customer2 = MockCustomer();

			customer1.CustomerId = customerId1;
			customer2.CustomerId = customerId2;

			// When
			var equalsByValue = customer1.EqualsByValue(customer2);

			// Then
			Assert.False(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByAddresses()
		{
			// Given
			var addresses1 = new List<Address>();
			var addresses2 = new List<Address>() { new() };

			var customer1 = MockCustomer();
			var customer2 = MockCustomer();

			customer1.Addresses = addresses1;
			customer2.Addresses = addresses2;

			// When
			var equalsByValue = customer1.EqualsByValue(customer2);

			// Then
			Assert.False(equalsByValue);
		}

		[Fact]
		public void ShouldRefuteEqualsByValueByNotes()
		{
			// Given
			var notes1 = new List<Note>();
			var notes2 = new List<Note>() { new() };

			var customer1 = MockCustomer();
			var customer2 = MockCustomer();

			customer1.Notes = notes1;
			customer2.Notes = notes2;

			// When
			var equalsByValue = customer1.EqualsByValue(customer2);

			// Then
			Assert.False(equalsByValue);
		}

		#endregion

		private static Customer MockCustomer() => new()
		{
			CustomerId = 8,
			FirstName = "one",
			LastName = "two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = 666,
			Addresses = new(),
			Notes = new() { null },
		};
	}
}

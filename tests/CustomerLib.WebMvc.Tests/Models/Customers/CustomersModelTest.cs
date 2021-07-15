using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Customers
{
	public class CustomersModelTest
	{
		[Fact]
		public void ShouldThrowOnCreateWithNullCustomers()
		{
			var exception = Assert.Throws<ArgumentException>(() => new CustomersModel(null));

			Assert.Equal("customers", exception.ParamName);
		}

		[Fact]
		public void ShouldCreateCustomersModelFromCustomers()
		{
			// Given
			var customers = MockCustomers(5);

			// When
			var model = new CustomersModel(customers);

			// Then
			Assert.Equal(customers, model.Customers);
			Assert.Equal(5, model.CustomersCount);
			Assert.True(model.HasCustomers);
			Assert.Equal(1, model.Page);
			Assert.Equal(1, model.PageSize);
			Assert.Equal(5, model.TotalPages);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var page = 2;
			var pageSize = 10;
			var totalPages = 11;

			var customers = MockCustomers(101);

			var model = new CustomersModel(customers);
			Assert.NotEqual(page, model.Page);
			Assert.NotEqual(pageSize, model.PageSize);
			Assert.NotEqual(totalPages, model.TotalPages);

			// When
			model.Page = page;
			model.PageSize = pageSize;

			// Then
			Assert.Equal(page, model.Page);
			Assert.Equal(pageSize, model.PageSize);
			Assert.Equal(totalPages, model.TotalPages);
		}

		private class HasCustomersData : TheoryData<List<Customer>, bool>
		{
			public HasCustomersData()
			{
				Add(new(), false);
				Add(MockCustomers(1), true);
			}
		}

		[Theory]
		[ClassData(typeof(HasCustomersData))]
		public void ShouldCheckIfHasCustomers(List<Customer> customers, bool hasCustomers)
		{
			// Given, When
			var model = new CustomersModel(customers);

			// Then
			Assert.Equal(hasCustomers, model.HasCustomers);
		}

		private static List<Customer> MockCustomers(int count)
		{
			var result = new List<Customer>();

			for (int i = 0; i < count; i++)
			{
				result.Add(new());
			}

			return result;
		}
	}
}

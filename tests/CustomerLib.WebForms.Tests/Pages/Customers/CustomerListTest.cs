using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Customers;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Customers
{
	public class CustomerListTest
	{
		[Fact]
		public void ShouldCreateCustomerList()
		{
			var customerList = new CustomerList();

			var customerServiceMock = new Mock<ICustomerService>();

			var customerListCustom = new CustomerList(customerServiceMock.Object);

			Assert.NotNull(customerList);
			Assert.NotNull(customerListCustom);
		}

		[Fact]
		public void ShouldLoadCustomers()
		{
			// Given
			var page = 12;
			var pageSize = 123;
			var expectedCustomersCount = 12345;
			var expectedCustomers = new List<Customer>();

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.GetPage(
				page, pageSize, false, false, true, expectedCustomersCount))
					.Returns(expectedCustomers);


			var customerList = new CustomerList(customerServiceMock.Object)
			{
				CustomersPerPage = pageSize
			};

			// When
			customerList.LoadCustomers(page, expectedCustomersCount);

			// Then
			Assert.Equal(expectedCustomers, customerList.Customers);
			customerServiceMock.Verify(s => s.GetPage(
				page, pageSize, false, false, true, expectedCustomersCount), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldDeleteCustomer(bool foundAndDeletedCustomer)
		{
			// Given
			var customerId = 5;

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Delete(customerId)).Returns(foundAndDeletedCustomer);

			var customerList = new CustomerList(customerServiceMock.Object);

			// When
			customerList.DeleteCustomer(customerId);

			// Then
			customerServiceMock.Verify(s => s.Delete(customerId), Times.Once);
		}

		[Fact]
		public void ShouldGetPageUrl()
		{
			// Given
			var page = 5;

			// When
			var pageUrl = CustomerList.GetPageUrl(page);

			// Then
			Assert.Equal("Customers?page=5", pageUrl);
		}
	}
}

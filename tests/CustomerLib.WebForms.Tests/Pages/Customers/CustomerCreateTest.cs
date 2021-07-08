using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Customers;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Customers
{
	public class CustomerCreateTest
	{
		[Fact]
		public void ShouldCreateTheCustomerCreate()
		{
			var customerCreate = new CustomerCreate();

			var customerServiceMock = new Mock<ICustomerService>();

			var customerCreateCustom = new CustomerCreate(customerServiceMock.Object);

			Assert.NotNull(customerCreate);
			Assert.NotNull(customerCreateCustom);
		}

		[Fact]
		public void ShouldCreateCustomer()
		{
			// Given
			var customer = new Customer();

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Save(customer));

			var customerCreate = new CustomerCreate(customerServiceMock.Object)
			{
				Customer = customer
			};

			// When
			customerCreate.CreateCustomer();

			// Then
			customerServiceMock.Verify(s => s.Save(customer), Times.Once);
		}

		[Theory]
		[InlineData("123")]
		[InlineData("1,2")]
		[InlineData("1.2")]
		[InlineData("")]
		[InlineData(null)]
		public void ShouldValidateTotalPurchasesAmount(string input)
		{
			// Given
			decimal? value = string.IsNullOrEmpty(input)
				? null
				: decimal.Parse(input);

			var customerCreate = new CustomerCreate() { Customer = new() { TotalPurchasesAmount = 5 } };
			Assert.NotEqual(value, customerCreate.Customer.TotalPurchasesAmount);

			// When
			var isValid = customerCreate.ValidateTotalPurchasesAmount(input);

			// Then
			Assert.True(isValid);
			Assert.Equal(value, customerCreate.Customer.TotalPurchasesAmount);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("a")]
		[InlineData("1.1.1")]
		public void ShouldInvalidateTotalPurchasesAmount(string input)
		{
			// Given
			var customerCreate = new CustomerCreate() { Customer = new() { TotalPurchasesAmount = 5 } };

			// When
			var isValid = customerCreate.ValidateTotalPurchasesAmount(input);

			// Then
			Assert.False(isValid);
			Assert.Null(customerCreate.Customer.TotalPurchasesAmount);
		}

		[Theory]
		[InlineData("", null, false)]
		[InlineData(null, null, false)]
		[InlineData(" ", " ", false)]
		[InlineData("a", "a", true)]
		public void ShouldValidateNote(string input, string expectedContent, bool isValidExpected)
		{
			// Given
			var customerCreate = new CustomerCreate()
			{
				Customer = new()
				{
					Notes = new() { new() { Content = "q" } }
				}
			};
			Assert.NotEqual(expectedContent, customerCreate.Customer.Notes[0].Content);

			// When
			var isValid = customerCreate.ValidateNote(input).IsValid;

			// Then
			Assert.Equal(isValidExpected, isValid);
			Assert.Equal(expectedContent, customerCreate.Customer.Notes[0].Content);
		}
	}
}

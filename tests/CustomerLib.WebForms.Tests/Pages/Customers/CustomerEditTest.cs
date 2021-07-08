using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Customers;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Customers
{
	public class CustomerEditTest
	{
		[Fact]
		public void ShouldCreateCustomerEdit()
		{
			var customerEdit = new CustomerEdit();

			var customerServiceMock = new Mock<ICustomerService>();
			var addressServiceMock = new Mock<IAddressService>();
			var noteServiceMock = new Mock<INoteService>();

			var customerEditCustom = new CustomerEdit(
				customerServiceMock.Object, addressServiceMock.Object, noteServiceMock.Object);

			Assert.NotNull(customerEdit);
			Assert.NotNull(customerEditCustom);
		}

		[Fact]
		public void ShouldLoadCustomer()
		{
			// Given
			var customerId = 5;
			var expectedCustomer = new Customer();

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Get(customerId, true, true))
				.Returns(expectedCustomer);

			var addressServiceMock = new StrictMock<IAddressService>();
			var noteServiceMock = new StrictMock<INoteService>();

			var customerEdit = new CustomerEdit(
				customerServiceMock.Object, addressServiceMock.Object, noteServiceMock.Object);

			// When
			customerEdit.LoadCustomer(customerId);

			// Then
			Assert.Equal(expectedCustomer, customerEdit.Customer);
			customerServiceMock.Verify(s => s.Get(customerId, true, true), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldSaveCustomer(bool foundAndUpdatedAddress)
		{
			// Given
			var customer = new Customer();

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Update(customer)).Returns(foundAndUpdatedAddress);

			var addressServiceMock = new StrictMock<IAddressService>();
			var noteServiceMock = new StrictMock<INoteService>();

			var customerEdit = new CustomerEdit(
				customerServiceMock.Object, addressServiceMock.Object, noteServiceMock.Object)
			{
				Customer = customer
			};

			// When
			customerEdit.SaveCustomer();

			// Then
			customerServiceMock.Verify(s => s.Update(customer), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldDeleteAddress(bool foundAndDeletedAddress)
		{
			// Given
			var addressId = 5;

			var addressServiceMock = new StrictMock<IAddressService>();
			addressServiceMock.Setup(s => s.Delete(addressId)).Returns(foundAndDeletedAddress);

			var customerServiceMock = new StrictMock<ICustomerService>();
			var noteServiceMock = new StrictMock<INoteService>();

			var customerEdit = new CustomerEdit(
				customerServiceMock.Object, addressServiceMock.Object, noteServiceMock.Object);

			// When
			customerEdit.DeleteAddress(addressId);

			// Then
			addressServiceMock.Verify(s => s.Delete(addressId), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldDeleteNote(bool foundAndDeletedNote)
		{
			// Given
			var noteId = 5;

			var noteServiceMock = new StrictMock<INoteService>();
			noteServiceMock.Setup(s => s.Delete(noteId)).Returns(foundAndDeletedNote);

			var addressServiceMock = new StrictMock<IAddressService>();
			var customerServiceMock = new StrictMock<ICustomerService>();

			var customerEdit = new CustomerEdit(
				customerServiceMock.Object, addressServiceMock.Object, noteServiceMock.Object);

			// When
			customerEdit.DeleteNote(noteId);

			// Then
			noteServiceMock.Verify(s => s.Delete(noteId), Times.Once);
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

			var customerEdit = new CustomerEdit() { Customer = new() { TotalPurchasesAmount = 5 } };
			Assert.NotEqual(value, customerEdit.Customer.TotalPurchasesAmount);

			// When
			var isValid = customerEdit.ValidateTotalPurchasesAmount(input);

			// Then
			Assert.True(isValid);
			Assert.Equal(value, customerEdit.Customer.TotalPurchasesAmount);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("a")]
		[InlineData("1.1.1")]
		public void ShouldInvalidateTotalPurchasesAmount(string input)
		{
			// Given
			var customerEdit = new CustomerEdit() { Customer = new() { TotalPurchasesAmount = 5 } };

			// When
			var isValid = customerEdit.ValidateTotalPurchasesAmount(input);

			// Then
			Assert.False(isValid);
			Assert.Null(customerEdit.Customer.TotalPurchasesAmount);
		}
	}
}

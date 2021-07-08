using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Addresses;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Addresses
{
	public class AddressListTest
	{
		[Fact]
		public void ShouldCreateAddressList()
		{
			var addressList = new AddressList();

			var customerServiceMock = new StrictMock<ICustomerService>();
			var addressServiceMock = new StrictMock<IAddressService>();

			var addressListCustom = new AddressList(
				customerServiceMock.Object, addressServiceMock.Object);

			Assert.NotNull(addressList);
			Assert.NotNull(addressListCustom);
		}

		[Fact]
		public void ShouldLoadCustomerWithAddresses()
		{
			// Given
			var customerId = 5;
			var expectedCustomer = new Customer()
			{
				CustomerId = customerId,
				Addresses = new() { new() }
			};


			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Get(customerId, true, false))
				.Returns(expectedCustomer);

			var addressServiceMock = new StrictMock<IAddressService>();

			var addressList = new AddressList(
				customerServiceMock.Object, addressServiceMock.Object);

			// When
			var customer = addressList.LoadCustomerWithAddresses(customerId);

			// Then
			Assert.Equal(expectedCustomer, customer);
			customerServiceMock.Verify(s => s.Get(customerId, true, false), Times.Once);
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

			var addressList = new AddressList(
				customerServiceMock.Object, addressServiceMock.Object);

			// When
			addressList.DeleteAddress(addressId);

			// Then
			addressServiceMock.Verify(s => s.Delete(addressId), Times.Once);
		}
	}
}

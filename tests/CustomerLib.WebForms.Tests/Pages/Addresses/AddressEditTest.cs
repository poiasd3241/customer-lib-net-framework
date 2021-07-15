using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebForms.Pages.Addresses;
using Moq;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.Addresses
{
	public class AddressEditTest
	{
		[Fact]
		public void ShouldCreateAddressEdit()
		{
			// Given, When
			var addressEdit = new AddressEdit();

			var customerServiceMock = new Mock<ICustomerService>();
			var addressServiceMock = new Mock<IAddressService>();

			var addressEditCustom = new AddressEdit(
				customerServiceMock.Object, addressServiceMock.Object);

			// Then
			Assert.NotNull(addressEdit);
			Assert.NotNull(addressEditCustom);
		}

		[Theory]
		[InlineData(5, true)]
		[InlineData(8, false)]
		public void ShouldCheckIfCustomerExists(int customerId, bool exists)
		{
			// Given
			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Exists(customerId)).Returns(exists);

			var addressServiceMock = new StrictMock<IAddressService>();

			var addressEdit = new AddressEdit(
				customerServiceMock.Object, addressServiceMock.Object);

			// When
			var actualExists = addressEdit.CheckCustomerExists(customerId);

			// Then
			Assert.Equal(exists, actualExists);
			customerServiceMock.Verify(s => s.Exists(customerId), Times.Once);
		}

		private class LoadAddressData : TheoryData<int, Address, bool>
		{
			public LoadAddressData()
			{
				Add(5, new(), true);
				Add(8, null, false);
			}
		}

		[Theory]
		[ClassData(typeof(LoadAddressData))]
		public void ShouldLoadAddress(int addressId, Address address, bool isNotNull)
		{
			// Given
			var addressServiceMock = new StrictMock<IAddressService>();
			addressServiceMock.Setup(s => s.Get(addressId)).Returns(address);

			var customerServiceMock = new StrictMock<ICustomerService>();

			var addressEdit = new AddressEdit(
				customerServiceMock.Object, addressServiceMock.Object);

			// When
			var loadedNotNull = addressEdit.LoadAddress(addressId);

			// Then
			Assert.Equal(isNotNull, loadedNotNull);
			Assert.Equal(address, addressEdit.Address);
			addressServiceMock.Verify(s => s.Get(addressId), Times.Once);
		}

		[Fact]
		public void ShouldLoadCustomer()
		{
			// Given
			var customerId = 5;
			var expectedCustomer = new Customer();

			var customerServiceMock = new StrictMock<ICustomerService>();
			customerServiceMock.Setup(s => s.Get(customerId, false, false))
				.Returns(expectedCustomer);

			var addressServiceMock = new Mock<IAddressService>(MockBehavior.Strict);

			var addressEdit = new AddressEdit(
				customerServiceMock.Object, addressServiceMock.Object);

			// When
			var customer = addressEdit.LoadCustomer(customerId);

			// Then
			Assert.Equal(expectedCustomer, customer);
			customerServiceMock.Verify(s => s.Get(customerId, false, false), Times.Once);
		}


		[Fact]
		public void ShouldCreateAddress()
		{
			// Given
			var address = new Address();

			var customerServiceMock = new StrictMock<ICustomerService>();

			var addressServiceMock = new Mock<IAddressService>(MockBehavior.Strict);
			addressServiceMock.Setup(s => s.Save(address)).Returns(true);

			var addressEdit = new AddressEdit(customerServiceMock.Object, addressServiceMock.Object)
			{
				IsCreate = true,
				Address = address
			};

			// When
			addressEdit.CreateAddress();

			// Then
			addressServiceMock.Verify(s => s.Save(address), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ShouldUpdateAddress(bool foundAndUpdatedAddress)
		{
			// Given
			var address = new Address();

			var customerServiceMock = new StrictMock<ICustomerService>();

			var addressServiceMock = new Mock<IAddressService>(MockBehavior.Strict);
			addressServiceMock.Setup(s => s.Update(address)).Returns(foundAndUpdatedAddress);

			var addressEdit = new AddressEdit(customerServiceMock.Object, addressServiceMock.Object)
			{
				IsCreate = false,
				Address = address
			};

			// When
			addressEdit.SaveExistingAddress();

			// Then
			addressServiceMock.Verify(s => s.Update(address), Times.Once);
		}
	}
}

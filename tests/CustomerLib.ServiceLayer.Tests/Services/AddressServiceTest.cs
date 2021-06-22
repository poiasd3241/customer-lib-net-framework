using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Exceptions;
using CustomerLib.Data.Repositories;
using CustomerLib.ServiceLayer.Services.Implementations;
using Moq;
using Xunit;

namespace CustomerLib.ServiceLayer.Tests.Services
{
	public class AddressServiceTest
	{
		[Fact]
		public void ShouldCreateAddressService()
		{
			var addressService = new AddressService();

			Assert.NotNull(addressService);
		}

		#region Save

		[Fact]
		public void ShouldSave()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedAddress = AddressServiceFixture.MockAddress();
			addressRepoMock.Setup(r => r.Create(expectedAddress));
			var service = new AddressService(addressRepoMock.Object);

			// When
			service.Save(expectedAddress);

			// Then
			addressRepoMock.Verify(r => r.Create(expectedAddress), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnSaveByBadAddress()
		{
			var service = AddressServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Save(new Address()));
		}

		[Fact]
		public void ShouldRethrowOnSave()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedException = new Exception("oops");
			var address = AddressServiceFixture.MockAddress();
			addressRepoMock.Setup(r => r.Create(address)).Throws(expectedException);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Save(address));

			// Then
			Assert.Equal(expectedException, exception);
			addressRepoMock.Verify(r => r.Create(address), Times.Once);
		}

		#endregion

		#region Get by Id

		[Fact]
		public void ShouldGetAddressById()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedAddress = AddressServiceFixture.MockAddress();
			addressRepoMock.Setup(r => r.Read(1)).Returns(expectedAddress);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var address = service.Get(1);

			// Then
			Assert.Equal(expectedAddress, address);
			addressRepoMock.Verify(r => r.Read(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetAddressByBadId()
		{
			// Given
			var service = AddressServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Get(0));

			// Then
			Assert.Equal("addressId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnGetAddressById()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedException = new Exception("oops");
			addressRepoMock.Setup(r => r.Read(1)).Throws(expectedException);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Get(1));

			// Then
			Assert.Equal(expectedException, exception);
			addressRepoMock.Verify(r => r.Read(1), Times.Once);
		}

		#endregion

		#region Find by customer Id

		[Fact]
		public void ShouldFindByCustomerId()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedAddresses = AddressServiceFixture.MockAddresses();
			addressRepoMock.Setup(r => r.ReadAllByCustomer(1)).Returns(expectedAddresses);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var addresses = service.FindByCustomer(1);

			// Then
			Assert.Equal(expectedAddresses, addresses);
			addressRepoMock.Verify(r => r.ReadAllByCustomer(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnFindByCustomerByBadId()
		{
			// Given
			var service = AddressServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.FindByCustomer(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnFindByCustomer()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedException = new Exception("oops");
			addressRepoMock.Setup(r => r.ReadAllByCustomer(1)).Throws(expectedException);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.FindByCustomer(1));

			// Then
			Assert.Equal(expectedException, exception);
			addressRepoMock.Verify(r => r.ReadAllByCustomer(1), Times.Once);
		}

		#endregion

		#region Update

		[Fact]
		public void ShouldUpdate()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var address = AddressServiceFixture.MockAddress();
			addressRepoMock.Setup(r => r.Update(address));
			var service = new AddressService(addressRepoMock.Object);

			// When
			service.Update(address);

			// Then
			addressRepoMock.Verify(r => r.Update(address), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadAddress()
		{
			var service = AddressServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Address()));
		}

		[Fact]
		public void ShouldRethrowOnUpdate()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedException = new Exception("oops");
			var address = AddressServiceFixture.MockAddress();
			addressRepoMock.Setup(r => r.Update(address)).Throws(expectedException);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Update(address));

			// Then
			Assert.Equal(expectedException, exception);
			addressRepoMock.Verify(r => r.Update(address), Times.Once);
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDelete()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.Delete(1));
			var service = new AddressService(addressRepoMock.Object);

			// When
			service.Delete(1);

			// Then
			addressRepoMock.Verify(r => r.Delete(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Given
			var service = AddressServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Delete(0));

			// Then
			Assert.Equal("addressId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnDelete()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedException = new Exception("oops");
			addressRepoMock.Setup(r => r.Delete(1)).Throws(expectedException);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Delete(1));

			// Then
			Assert.Equal(expectedException, exception);
			addressRepoMock.Verify(r => r.Delete(1), Times.Once);
		}

		#endregion

		#region Delete by customer Id

		[Fact]
		public void ShouldDeleteByCustomerId()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.DeleteByCustomer(1));
			var service = new AddressService(addressRepoMock.Object);

			// When
			service.DeleteByCustomer(1);

			// Then
			addressRepoMock.Verify(r => r.DeleteByCustomer(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByCustomerByBadId()
		{
			// Given
			var service = AddressServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.DeleteByCustomer(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnDeleteByCustomer()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedException = new Exception("oops");
			addressRepoMock.Setup(r => r.DeleteByCustomer(1)).Throws(expectedException);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.DeleteByCustomer(1));

			// Then
			Assert.Equal(expectedException, exception);
			addressRepoMock.Verify(r => r.DeleteByCustomer(1), Times.Once);
		}

		#endregion
	}

	public class AddressServiceFixture
	{
		public static Address MockAddress() => new()
		{
			AddressLine = "line",
			AddressLine2 = "line2",
			Type = AddressType.Shipping,
			City = "city",
			PostalCode = "code",
			State = "state",
			Country = "United States"
		};

		public static List<Address> MockAddresses() => new() { MockAddress(), MockAddress() };

		public static Mock<IAddressRepository> MockAddressRepository() => new(MockBehavior.Strict);

		/// <summary>
		/// Use when dependencies don't need to be mocked (are unused).
		/// </summary>
		public static AddressService MockService() =>
				new(MockAddressRepository().Object);
	}
}

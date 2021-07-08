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

		#region Exists

		public class ExistsByIdData : TheoryData<int, bool>
		{
			public ExistsByIdData()
			{
				Add(1, true);
				Add(2, false);
			}
		}

		[Theory]
		[ClassData(typeof(ExistsByIdData))]
		public void ShouldCheckIfAddressExistsById(int addressId, bool existsExpected)
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.Exists(addressId)).Returns(existsExpected);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var exists = service.Exists(addressId);

			// Then
			Assert.Equal(existsExpected, exists);
			addressRepoMock.Verify(r => r.Exists(addressId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnCheckIfAddressExistsByBadId()
		{
			// Given
			var service = AddressServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Exists(0));

			// Then
			Assert.Equal("addressId", exception.ParamName);
		}

		#endregion

		#region Save

		[Fact]
		public void ShouldSave()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedAddress = AddressServiceFixture.MockAddress();
			addressRepoMock.Setup(r => r.Create(expectedAddress)).Returns(5);
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

		#endregion

		#region Find by customer Id

		[Fact]
		public void ShouldFindByCustomerId()
		{
			// Given
			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			var expectedAddresses = AddressServiceFixture.MockAddresses();
			addressRepoMock.Setup(r => r.ReadByCustomer(1)).Returns(expectedAddresses);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var addresses = service.FindByCustomer(1);

			// Then
			Assert.Equal(expectedAddresses, addresses);
			addressRepoMock.Verify(r => r.ReadByCustomer(1), Times.Once);
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

		#endregion

		#region Update

		[Fact]
		public void ShouldUpdateExistingAddress()
		{
			// Given
			var address = AddressServiceFixture.MockAddress();
			address.AddressId = 5;

			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.Exists(address.AddressId)).Returns(true);
			addressRepoMock.Setup(r => r.Update(address));
			var service = new AddressService(addressRepoMock.Object);

			// When
			var result = service.Update(address);

			// Then
			Assert.True(result);
			addressRepoMock.Verify(r => r.Exists(address.AddressId), Times.Once);
			addressRepoMock.Verify(r => r.Update(address), Times.Once);
		}

		[Fact]
		public void ShouldNotUpdateNotFoundgAddress()
		{
			// Given
			var address = AddressServiceFixture.MockAddress();
			address.AddressId = 5;

			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.Exists(address.AddressId)).Returns(false);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var result = service.Update(address);

			// Then
			Assert.False(result);
			addressRepoMock.Verify(r => r.Exists(address.AddressId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadAddress()
		{
			var service = AddressServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Address()));
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDeleteExistingAddress()
		{
			// Given
			var addressId = 5;

			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.Exists(addressId)).Returns(true);
			addressRepoMock.Setup(r => r.Delete(addressId));
			var service = new AddressService(addressRepoMock.Object);

			// When
			var result = service.Delete(addressId);

			// Then
			Assert.True(result);
			addressRepoMock.Verify(r => r.Exists(addressId), Times.Once);
			addressRepoMock.Verify(r => r.Delete(addressId), Times.Once);
		}

		[Fact]
		public void ShouldNotDeleteNotFoundAddress()
		{
			// Given
			var addressId = 5;

			var addressRepoMock = AddressServiceFixture.MockAddressRepository();
			addressRepoMock.Setup(r => r.Exists(addressId)).Returns(false);
			var service = new AddressService(addressRepoMock.Object);

			// When
			var result = service.Delete(addressId);

			// Then
			Assert.False(result);
			addressRepoMock.Verify(r => r.Exists(addressId), Times.Once);
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

		#endregion
	}

	public class AddressServiceFixture
	{
		/// <returns>The mocked address with repo-relevant valid properties,
		/// optional properties not null.</returns>
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

		/// <returns>The list containing 2 mocked address with repo-relevant valid properties,
		/// optional properties not null</returns>
		public static List<Address> MockAddresses() => new() { MockAddress(), MockAddress() };

		public static Mock<IAddressRepository> MockAddressRepository() => new(MockBehavior.Strict);

		/// <summary>
		/// Use when dependencies don't need to be mocked (are unused).
		/// </summary>
		public static AddressService MockService() =>
				new(MockAddressRepository().Object);
	}
}

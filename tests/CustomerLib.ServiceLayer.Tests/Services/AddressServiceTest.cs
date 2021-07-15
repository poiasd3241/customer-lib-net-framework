using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Exceptions;
using CustomerLib.Data.Repositories;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.TestHelpers;
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
			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.Exists(addressId)).Returns(existsExpected);

			var service = fixture.CreateService();

			// When
			var exists = service.Exists(addressId);

			// Then
			Assert.Equal(existsExpected, exists);
			fixture.MockAddressRepository.Verify(r => r.Exists(addressId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnCheckIfAddressExistsByBadId()
		{
			// Given
			var service = new AddressServiceFixture().CreateService();

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
			var address = AddressServiceFixture.MockAddress();
			var customerId = 5;
			address.CustomerId = customerId;

			var fixture = new AddressServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(true);
			fixture.MockAddressRepository.Setup(r => r.Create(address)).Returns(8);

			var service = fixture.CreateService();

			// When
			var result = service.Save(address);

			// Then
			Assert.True(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
			fixture.MockAddressRepository.Verify(r => r.Create(address), Times.Once);
		}

		[Fact]
		public void ShouldNotSaveByCustomerNotFound()
		{
			// Given
			var address = AddressServiceFixture.MockAddress();
			var customerId = 5;
			address.CustomerId = customerId;

			var fixture = new AddressServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Save(address);

			// Then
			Assert.False(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnSaveByBadAddress()
		{
			var service = new AddressServiceFixture().CreateService();

			Assert.Throws<EntityValidationException>(() => service.Save(new Address()));
		}

		#endregion

		#region Get by Id

		[Fact]
		public void ShouldGetAddressById()
		{
			// Given
			var addressId = 5;
			var expectedAddress = AddressServiceFixture.MockAddress();

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.Read(addressId)).Returns(expectedAddress);

			var service = fixture.CreateService();

			// When
			var address = service.Get(addressId);

			// Then
			Assert.Equal(expectedAddress, address);
			fixture.MockAddressRepository.Verify(r => r.Read(addressId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetAddressByBadId()
		{
			// Given
			var service = new AddressServiceFixture().CreateService();

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
			var customerId = 5;
			var expectedAddresses = AddressServiceFixture.MockAddresses();

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.ReadByCustomer(customerId))
				.Returns(expectedAddresses);

			var service = fixture.CreateService();

			// When
			var addresses = service.FindByCustomer(customerId);

			// Then
			Assert.Equal(expectedAddresses, addresses);
			fixture.MockAddressRepository.Verify(r => r.ReadByCustomer(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnFindByCustomerByBadId()
		{
			// Given
			var service = new AddressServiceFixture().CreateService();

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
			var addressId = 5;
			address.AddressId = addressId;

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.Exists(addressId)).Returns(true);
			fixture.MockAddressRepository.Setup(r => r.Update(address));

			var service = fixture.CreateService();

			// When
			var result = service.Update(address);

			// Then
			Assert.True(result);
			fixture.MockAddressRepository.Verify(r => r.Exists(addressId), Times.Once);
			fixture.MockAddressRepository.Verify(r => r.Update(address), Times.Once);
		}

		[Fact]
		public void ShouldNotUpdateNotFoundAddress()
		{
			// Given
			var address = AddressServiceFixture.MockAddress();
			var addressId = 5;
			address.AddressId = addressId;

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.Exists(addressId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Update(address);

			// Then
			Assert.False(result);
			fixture.MockAddressRepository.Verify(r => r.Exists(addressId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadAddress()
		{
			var service = new AddressServiceFixture().CreateService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Address()));
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDeleteExistingAddress()
		{
			// Given
			var addressId = 5;

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.Exists(addressId)).Returns(true);
			fixture.MockAddressRepository.Setup(r => r.Delete(addressId));

			var service = fixture.CreateService();

			// When
			var result = service.Delete(addressId);

			// Then
			Assert.True(result);
			fixture.MockAddressRepository.Verify(r => r.Exists(addressId), Times.Once);
			fixture.MockAddressRepository.Verify(r => r.Delete(addressId), Times.Once);
		}

		[Fact]
		public void ShouldNotDeleteNotFoundAddress()
		{
			// Given
			var addressId = 5;

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.Exists(addressId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Delete(addressId);

			// Then
			Assert.False(result);
			fixture.MockAddressRepository.Verify(r => r.Exists(addressId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Given
			var service = new AddressServiceFixture().CreateService();

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
			var customerId = 5;

			var fixture = new AddressServiceFixture();
			fixture.MockAddressRepository.Setup(r => r.DeleteByCustomer(customerId));

			var service = fixture.CreateService();

			// When
			service.DeleteByCustomer(customerId);

			// Then
			fixture.MockAddressRepository.Verify(r => r.DeleteByCustomer(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByCustomerByBadId()
		{
			// Given
			var service = new AddressServiceFixture().CreateService();

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

		public StrictMock<ICustomerRepository> MockCustomerRepository { get; set; }
		public StrictMock<IAddressRepository> MockAddressRepository { get; set; }

		public AddressServiceFixture()
		{
			MockCustomerRepository = new();
			MockAddressRepository = new();
		}

		public AddressService CreateService() =>
			new(MockCustomerRepository.Object, MockAddressRepository.Object);
	}
}

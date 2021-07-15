using System;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Data.IntegrationTests.Repositories.TestHelpers;
using CustomerLib.Data.Repositories.Implementations;
using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class AddressRepositoryTest
	{
		[Fact]
		public void ShouldCreateAddressRepository()
		{
			var repo = new AddressRepository();

			Assert.NotNull(repo);
		}

		#region Exists

		[Theory]
		[InlineData(2)]
		[InlineData(3)]
		public void ShouldCheckIfNoteExistsById(int addressId)
		{
			// Given
			var addressRepository = new AddressRepository();
			AddressRepositoryFixture.CreateMockAddress(amount: 2);

			// When
			var exists = addressRepository.Exists(addressId);

			// Then
			if (addressId == 2)
			{
				Assert.True(exists);
			}
			if (addressId == 3)
			{
				Assert.False(exists);
			}
		}

		#endregion

		[Fact]
		public void ShouldCreateAddress()
		{
			// Given
			var addressRepository = new AddressRepository();
			CustomerRepositoryFixture.CreateMockCustomer();
			AddressRepository.DeleteAll();
			AddressTypeHelperRepository.UnsafeRepopulateAddressTypes();

			var address = AddressRepositoryFixture.MockAddress();
			address.CustomerId = 1;

			// When, Then
			addressRepository.Create(address);
		}

		[Fact]
		public void ShouldReadAddressNotFound()
		{
			// Given
			var addressRepository = new AddressRepository();
			AddressRepository.DeleteAll();

			// When
			var readAddress = addressRepository.Read(1);

			// Then
			Assert.Null(readAddress);
		}

		public class CreateMockAddressData : TheoryData<Func<Address>>
		{
			public CreateMockAddressData()
			{
				Add(() => AddressRepositoryFixture.CreateMockAddress());
				Add(() => AddressRepositoryFixture.CreateMockOptionalAddress());
			}
		}

		[Theory]
		[ClassData(typeof(CreateMockAddressData))]
		public void ShouldReadAddressIncludingNullOptionalFields(Func<Address> createMockAddress)
		{
			// Given
			var addressRepository = new AddressRepository();
			var address = createMockAddress.Invoke();

			// When
			var readAddress = addressRepository.Read(1);

			// Then
			Assert.NotNull(readAddress);
			Assert.Equal(address.CustomerId, readAddress.CustomerId);
			Assert.Equal(address.AddressLine, readAddress.AddressLine);
			Assert.Equal(address.AddressLine2, readAddress.AddressLine2);
			Assert.Equal(address.Type, readAddress.Type);
			Assert.Equal(address.City, readAddress.City);
			Assert.Equal(address.PostalCode, readAddress.PostalCode);
			Assert.Equal(address.State, readAddress.State);
			Assert.Equal(address.Country, readAddress.Country);
		}

		[Fact]
		public void ShouldReadAllAddressesByCustomer()
		{
			// Given
			var addressRepository = new AddressRepository();
			var address = AddressRepositoryFixture.CreateMockAddress(2);

			// When
			var readAddresses = addressRepository.ReadByCustomer(address.CustomerId);

			// Then
			Assert.NotNull(readAddresses);
			Assert.Equal(2, readAddresses.Count);

			foreach (var readAddress in readAddresses)
			{
				Assert.Equal(address.CustomerId, readAddress.CustomerId);
				Assert.Equal(address.AddressLine, readAddress.AddressLine);
				Assert.Equal(address.AddressLine2, readAddress.AddressLine2);
				Assert.Equal(address.Type, readAddress.Type);
				Assert.Equal(address.City, readAddress.City);
				Assert.Equal(address.PostalCode, readAddress.PostalCode);
				Assert.Equal(address.State, readAddress.State);
				Assert.Equal(address.Country, readAddress.Country);
			}
		}

		[Fact]
		public void ShouldReadAllAddressesByCustomerNotFound()
		{
			// Given
			var addressRepository = new AddressRepository();
			AddressRepository.DeleteAll();

			// When
			var readAddresses = addressRepository.ReadByCustomer(1);

			// Then
			Assert.Empty(readAddresses);
		}

		[Fact]
		public void ShouldUpdateAddress()
		{
			// Given
			var addressRepository = new AddressRepository();
			var address = AddressRepositoryFixture.CreateMockAddress();

			var createdAddress = addressRepository.Read(1);
			createdAddress.AddressLine = "New line!";

			// When
			addressRepository.Update(createdAddress);

			// Then
			var updatedAddress = addressRepository.Read(1);

			Assert.NotNull(updatedAddress);
			Assert.Equal(address.CustomerId, updatedAddress.CustomerId);
			Assert.Equal("New line!", updatedAddress.AddressLine);
			Assert.Equal(address.AddressLine2, updatedAddress.AddressLine2);
			Assert.Equal(address.Type, updatedAddress.Type);
			Assert.Equal(address.City, updatedAddress.City);
			Assert.Equal(address.PostalCode, updatedAddress.PostalCode);
			Assert.Equal(address.State, updatedAddress.State);
			Assert.Equal(address.Country, updatedAddress.Country);
		}

		[Fact]
		public void ShouldDeleteAddress()
		{
			// Given
			var addressRepository = new AddressRepository();
			AddressRepositoryFixture.CreateMockAddress();

			var createdAddress = addressRepository.Read(1);
			Assert.NotNull(createdAddress);

			// When
			addressRepository.Delete(1);

			// Then
			var deletedAddress = addressRepository.Read(1);
			Assert.Null(deletedAddress);
		}

		[Fact]
		public void ShouldDeleteAddressesByCustomerId()
		{
			// Given
			var addressRepository = new AddressRepository();
			AddressRepositoryFixture.CreateMockAddress(2);

			var createdAddresses = addressRepository.ReadByCustomer(1);
			Assert.Equal(2, createdAddresses.Count);

			// When
			addressRepository.DeleteByCustomer(1);

			// Then
			var deletedAddresses = addressRepository.ReadByCustomer(1);
			Assert.Empty(deletedAddresses);
		}
	}

	public class AddressRepositoryFixture
	{
		/// <summary>
		/// Clears the Addresses table, then creates the specified amount of mocked addresses 
		/// with repo-relevant valid properties, optional properties not null, 
		/// <see cref="Address.CustomerId"/> = 1.
		/// </summary>
		/// <param name="amount">The amount of addresses to create.</param>
		/// <returns>The mocked address with repo-relevant valid properties, 
		/// optional properties null, <see cref="Address.CustomerId"/> = 1.</returns>
		public static Address CreateMockAddress(int amount = 1)
		{
			var addressRepository = new AddressRepository();
			CustomerRepositoryFixture.CreateMockCustomer();
			AddressRepository.DeleteAll();
			AddressTypeHelperRepository.UnsafeRepopulateAddressTypes();

			var address = MockAddress();
			address.CustomerId = 1;

			for (int i = 0; i < amount; i++)
			{
				addressRepository.Create(address);
			}

			return address;
		}

		/// <summary>
		/// Clears the Addresses table, then creates the mocked address
		/// with repo-relevant valid properties, optional properties null, 
		/// <see cref="Address.CustomerId"/> = 1.
		/// </summary>
		/// <returns>The mocked address with repo-relevant valid properties, 
		/// optional properties null, <see cref="Address.CustomerId"/> = 1.</returns>
		public static Address CreateMockOptionalAddress()
		{
			var addressRepository = new AddressRepository();
			CustomerRepositoryFixture.CreateMockCustomer();
			AddressRepository.DeleteAll();

			AddressTypeHelperRepository
				.UnsafeRepopulateAddressTypes();

			var address = MockOptionalAddress();
			address.CustomerId = 1;

			addressRepository.Create(address);

			return address;
		}

		/// <returns>The mocked address with repo-relevant valid properties,
		/// optional properties not null.</returns>
		public static Address MockAddress() => new()
		{
			AddressLine = "one",
			AddressLine2 = "two",
			Type = AddressType.Shipping,
			City = "Seattle",
			PostalCode = "123456",
			State = "WA",
			Country = "United States"
		};

		/// <returns>The mocked address with repo-relevant valid properties,
		/// optional properties null.</returns>
		public static Address MockOptionalAddress()
		{
			var mockAddress = MockAddress();
			mockAddress.AddressLine2 = null;
			return mockAddress;
		}
	}
}

using System;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Data.IntegrationTests.Repositories.TestHelpers;
using CustomerLib.Data.Repositories.EF;
using Xunit;
using static CustomerLib.Data.IntegrationTests.Repositories.EF.CustomerRepositoryTest;

namespace CustomerLib.Data.IntegrationTests.Repositories.EF
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class AddressRepositoryTest
	{
		[Fact]
		public void ShouldCreateAddressRepositoryDefaultConstructor()
		{
			var repo = new AddressRepository();

			Assert.NotNull(repo);
		}

		[Fact]
		public void ShouldCreateAddressRepository()
		{
			var context = new CustomerLibDataContext();

			var repo = new AddressRepository(context);

			Assert.NotNull(repo);
		}

		[Theory]
		[InlineData(2, true)]
		[InlineData(3, false)]
		public void ShouldCheckIfNoteExistsById(int addressId, bool expectedExists)
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			AddressRepositoryFixture.CreateMockAddress(amount: 2);

			// When
			var exists = repo.Exists(addressId);

			// Then
			Assert.Equal(expectedExists, exists);
		}

		[Fact]
		public void ShouldCreateAddress()
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();

			var address = AddressRepositoryFixture.MockAddress();
			address.CustomerId = 1;

			// When
			var createdId = repo.Create(address);

			// Then
			Assert.Equal(1, createdId);
		}

		[Fact]
		public void ShouldReadAddressNotFound()
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();

			// When
			var readAddress = repo.Read(1);

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
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			var address = createMockAddress.Invoke();

			// When
			var readAddress = repo.Read(1);

			// Then
			Assert.Equal(1, readAddress.AddressId);
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
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			var address = AddressRepositoryFixture.CreateMockAddress(2);

			// When
			var readAddresses = repo.ReadByCustomer(address.CustomerId);

			// Then
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

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		public void ShouldReadAllAddressesByCustomerBothNotFoundAndEmpty(int customerId)
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();

			// When
			var readAddresses = repo.ReadByCustomer(customerId);

			// Then
			Assert.Empty(readAddresses);
		}

		[Fact]
		public void ShouldUpdateAddress()
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			var address = AddressRepositoryFixture.CreateMockAddress();

			var createdAddress = repo.Read(1);
			createdAddress.AddressLine = "New line!";

			// When
			repo.Update(createdAddress);

			// Then
			var updatedAddress = repo.Read(1);

			Assert.Equal(1, createdAddress.AddressId);
			Assert.Equal(createdAddress.AddressId, updatedAddress.AddressId);

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
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			AddressRepositoryFixture.CreateMockAddress();

			var createdAddress = repo.Read(1);
			Assert.NotNull(createdAddress);

			// When
			repo.Delete(1);

			// Then
			var deletedAddress = repo.Read(1);
			Assert.Null(deletedAddress);
		}

		[Fact]
		public void ShouldDeleteAddressesByCustomerId()
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			AddressRepositoryFixture.CreateMockAddress(2);

			var createdAddresses = repo.ReadByCustomer(1);
			Assert.Equal(2, createdAddresses.Count);

			// When
			repo.DeleteByCustomer(1);

			// Then
			var deletedAddresses = repo.ReadByCustomer(1);
			Assert.Empty(deletedAddresses);
		}

		[Fact]
		public void ShouldDeleteAllAddresses()
		{
			// Given
			var repo = AddressRepositoryFixture.CreateEmptyRepositoryWithCustomer();
			AddressRepositoryFixture.CreateMockAddress(2);

			var createdAddresses = repo.ReadByCustomer(1);
			Assert.Equal(2, createdAddresses.Count);

			// When
			repo.DeleteAll();

			// Then
			var deletedAddresses = repo.ReadByCustomer(1);
			Assert.Empty(deletedAddresses);
		}

		public class AddressRepositoryFixture
		{
			/// <summary>
			/// Creates the empty repository, containing a single customer
			/// with <see cref="Customer.CustomerId"/> = 1 and no addresses.
			/// <br/>
			/// Also repopulates the <see cref="AddressType"/> table.
			/// </summary>
			/// <returns>The empty address repository.</returns>
			public static AddressRepository CreateEmptyRepositoryWithCustomer()
			{
				CustomerRepositoryFixture.CreateMockCustomer();
				AddressTypeHelperRepository.UnsafeRepopulateAddressTypes();

				return new();
			}

			/// <summary>
			/// Creates the specified amount of mocked addresses 
			/// with repo-relevant valid properties, optional properties not null, 
			/// <see cref="Address.CustomerId"/> = 1.
			/// </summary>
			/// <param name="amount">The amount of addresses to create.</param>
			/// <returns>The mocked address with repo-relevant valid properties, 
			/// optional properties null, <see cref="Address.CustomerId"/> = 1.</returns>
			public static Address CreateMockAddress(int amount = 1)
			{
				var repo = new AddressRepository();

				var address = MockAddress();
				address.CustomerId = 1;

				for (int i = 0; i < amount; i++)
				{
					repo.Create(address);
				}

				return address;
			}

			/// <summary>
			/// Creates the mocked address with repo-relevant valid properties,
			/// optional properties null, <see cref="Address.CustomerId"/> = 1.
			/// </summary>
			/// <returns>The mocked address with repo-relevant valid properties, 
			/// optional properties null, <see cref="Address.CustomerId"/> = 1.</returns>
			public static Address CreateMockOptionalAddress()
			{
				var repo = new AddressRepository();

				var address = MockOptionalAddress();
				address.CustomerId = 1;

				repo.Create(address);

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
}

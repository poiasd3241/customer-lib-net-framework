using System;
using System.Linq;
using CustomerLib.Business.Entities;
using CustomerLib.Data.Repositories.EF;
using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories.EF
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class CustomerRepositoryTest
	{
		[Fact]
		public void ShouldCreateCustomerRepositoryDefaultConstructor()
		{
			var repo = new CustomerRepository();

			Assert.NotNull(repo);
		}

		[Fact]
		public void ShouldCreateCustomerRepository()
		{
			var context = new CustomerLibDataContext();

			var repo = new CustomerRepository(context);

			Assert.NotNull(repo);
		}

		#region Exists

		[Theory]
		[InlineData(2, true)]
		[InlineData(3, false)]
		public void ShouldCheckIfCustomerExistsById(int customerId, bool expectedExists)
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			// When
			var exists = repo.Exists(customerId);

			// Then
			Assert.Equal(expectedExists, exists);
		}

		#endregion

		#region Create, update, delete

		[Fact]
		public void ShouldCreateCustomer()
		{
			// Given
			var repo = CustomerRepositoryFixture.CreateEmptyRepository();
			var customer = CustomerRepositoryFixture.MockCustomer();

			// When
			var createdId = repo.Create(customer);

			// Then
			Assert.Equal(1, createdId);
		}

		[Fact]
		public void ShouldUpdateCustomer()
		{
			// Given
			var repo = new CustomerRepository();
			var customer = CustomerRepositoryFixture.CreateMockCustomer();

			var createdCustomer = repo.Read(1);
			createdCustomer.FirstName = "New FN";

			// When
			repo.Update(createdCustomer);

			// Then
			var updatedCustomer = repo.Read(1);

			Assert.Equal("New FN", updatedCustomer.FirstName);
			Assert.Equal(customer.LastName, updatedCustomer.LastName);
			Assert.Equal(customer.PhoneNumber, updatedCustomer.PhoneNumber);
			Assert.Equal(customer.Email, updatedCustomer.Email);
			Assert.Equal(customer.TotalPurchasesAmount, updatedCustomer.TotalPurchasesAmount);

			Assert.Null(createdCustomer.Addresses);
			Assert.Null(createdCustomer.Notes);
		}

		[Fact]
		public void ShouldDeleteCustomer()
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer();

			var createdCustomer = repo.Read(1);
			Assert.NotNull(createdCustomer);

			// When
			repo.Delete(1);

			// Then
			var deletedCustomer = repo.Read(1);
			Assert.Null(deletedCustomer);
		}

		#endregion

		#region Read by Id

		[Fact]
		public void ShouldReadCustomerNotFound()
		{
			// Given
			var repo = CustomerRepositoryFixture.CreateEmptyRepository();

			// When
			var readCustomer = repo.Read(1);

			// Then
			Assert.Null(readCustomer);
		}

		public class CreateMockCustomerData : TheoryData<Func<Customer>>
		{
			public CreateMockCustomerData()
			{
				Add(() => CustomerRepositoryFixture.CreateMockCustomer());
				Add(() => CustomerRepositoryFixture.CreateMockOptionalCustomer());
			}
		}

		[Theory]
		[ClassData(typeof(CreateMockCustomerData))]
		public void ShouldReadCustomerIncludingNullOptionalFields(Func<Customer> createMockCustomer)
		{
			// Given
			var repo = new CustomerRepository();
			var customer = createMockCustomer.Invoke();

			// When
			var readCustomer = repo.Read(1);

			// Then
			Assert.Equal(1, readCustomer.CustomerId);
			Assert.Equal(customer.FirstName, readCustomer.FirstName);
			Assert.Equal(customer.LastName, readCustomer.LastName);
			Assert.Equal(customer.PhoneNumber, readCustomer.PhoneNumber);
			Assert.Equal(customer.Email, readCustomer.Email);
			Assert.Equal(customer.TotalPurchasesAmount, readCustomer.TotalPurchasesAmount);

			Assert.Null(readCustomer.Addresses);
			Assert.Null(readCustomer.Notes);
		}

		#endregion

		#region Read all

		[Fact]
		public void ShouldReadAllCustomers()
		{
			// Given
			var repo = new CustomerRepository();
			var customer = CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			// When
			var readCustomers = repo.ReadAll();

			// Then
			Assert.Equal(2, readCustomers.Count);

			foreach (var readCustomer in readCustomers)
			{
				Assert.Equal(customer.FirstName, readCustomer.FirstName);
				Assert.Equal(customer.LastName, readCustomer.LastName);
				Assert.Equal(customer.PhoneNumber, readCustomer.PhoneNumber);
				Assert.Equal(customer.Email, readCustomer.Email);
				Assert.Equal(customer.TotalPurchasesAmount, readCustomer.TotalPurchasesAmount);

				Assert.Null(readCustomer.Addresses);
				Assert.Null(readCustomer.Notes);
			}
		}

		[Fact]
		public void ShouldReadAllCustomersNotFound()
		{
			// Given
			var repo = CustomerRepositoryFixture.CreateEmptyRepository();

			// When
			var readCustomers = repo.ReadAll();

			// Then
			Assert.Empty(readCustomers);
		}

		#endregion

		#region Get count

		[Fact]
		public void ShouldGetTotalCustomerCount()
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			// When
			var count = repo.GetCount();

			// Then
			Assert.Equal(2, count);
		}

		[Fact]
		public void ShouldGetTotalCustomerCountWhenEmpty()
		{
			// Given
			var repo = CustomerRepositoryFixture.CreateEmptyRepository();

			// When
			var count = repo.GetCount();

			// Then
			Assert.Equal(0, count);
		}

		#endregion

		#region Read page

		[Fact]
		public void ShouldReadPageOfCustomersFromSinglePage()
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 5);

			// When
			var readCustomers = repo.ReadPage(1, 10);

			// Then
			Assert.Equal(5, readCustomers.Count);
		}

		[Fact]
		public void ShouldReadPageOfCustomersFromMultiplePages()
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 5);

			// When
			var readCustomers = repo.ReadPage(2, 3);

			// Then
			Assert.Equal(2, readCustomers.Count);
			var readCustomersList = readCustomers.ToArray();

			Assert.Equal(4, readCustomersList[0].CustomerId);
			Assert.Equal(5, readCustomersList[1].CustomerId);
		}

		[Fact]
		public void ShouldReadPageOfCustomersNotFound()
		{
			// Given
			var repo = CustomerRepositoryFixture.CreateEmptyRepository();

			// When
			var readCustomers = repo.ReadPage(2, 3);

			// Then
			Assert.Empty(readCustomers);
		}

		#endregion

		#region Email taken checks

		class EmailTakenData : TheoryData<string, bool>
		{
			public EmailTakenData()
			{
				Add("taken@asd.com", true);
				Add("free@asd.com", false);
			}
		}

		[Theory]
		[ClassData(typeof(EmailTakenData))]
		public void ShouldCheckForEmailTaken(string email, bool isTakenExpected)
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer("taken@asd.com");

			// When
			var isTaken = repo.IsEmailTaken(email);

			// Then
			Assert.Equal(isTakenExpected, isTaken);
		}

		[Theory]
		[ClassData(typeof(EmailTakenData))]
		public void ShouldCheckForEmailTakenWithCustomerId(string email, bool isTakenExpected)
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer("taken@asd.com");

			// When
			var (isTaken, takenById) = repo.IsEmailTakenWithCustomerId(email);

			// Then
			Assert.Equal(isTakenExpected, isTaken);
			Assert.Equal(isTaken ? 1 : 0, takenById);
		}

		#endregion

		[Fact]
		public void ShouldDeleteAllCustomers()
		{
			// Given
			var repo = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			var createdCustomers = repo.ReadAll();
			Assert.Equal(2, createdCustomers.Count);

			// When
			repo.DeleteAll();

			// Then
			var deletedCustomers = repo.ReadAll();
			Assert.Empty(deletedCustomers);
		}

		public class CustomerRepositoryFixture
		{
			/// <summary>
			/// Creates the empty repository.
			/// </summary>
			/// <returns>The empty customer repository.</returns>
			public static CustomerRepository CreateEmptyRepository()
			{
				var repo = new CustomerRepository();
				repo.DeleteAll();

				return repo;
			}

			/// <summary>
			/// Deletes all customers, then creates the specified amount of mocked customers 
			/// with repo-relevant valid properties, optional properties not null.
			/// </summary>
			/// <param name="amount">The amount of customers to create.</param>
			/// <returns>The mocked customer with repo-relevant valid properties, 
			/// optional properties not null.</returns>
			public static Customer CreateMockCustomer(string email = "john@doe.com", int amount = 1)
			{
				var repo = CreateEmptyRepository();

				var customer = MockCustomer(email);

				for (int i = 0; i < amount; i++)
				{
					repo.Create(customer);
				}

				return customer;
			}

			/// <summary>
			/// Deletes all customers, then creates the mocked customer with 
			/// repo-relevant valid properties, optional properties null.
			/// </summary>
			/// <returns>The mocked customer with repo-relevant valid properties,
			/// optional properties null.</returns>
			public static Customer CreateMockOptionalCustomer()
			{
				var repo = CreateEmptyRepository();

				var customer = MockOptionalCustomer();
				repo.Create(customer);

				return customer;
			}

			/// <returns>The mocked customer with repo-relevant valid properties,
			/// optional properties not null.</returns>
			public static Customer MockCustomer(string email = "john@doe.com") => new()
			{
				FirstName = "John",
				LastName = "Doe",
				PhoneNumber = "+12345",
				Email = email,
				TotalPurchasesAmount = 123
			};

			/// <returns>The mocked customer with repo-relevant valid properties,
			/// optional properties null.</returns>
			public static Customer MockOptionalCustomer() => new()
			{
				FirstName = null,
				LastName = "Doe",
				PhoneNumber = null,
				Email = null,
				TotalPurchasesAmount = null
			};
		}
	}
}

using System;
using System.Linq;
using CustomerLib.Business.Entities;
using CustomerLib.Data.Repositories.Implementations;
using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class CustomerRepositoryTest
	{
		[Fact]
		public void ShouldCreateCustomerRepository()
		{
			var repo = new CustomerRepository();

			Assert.NotNull(repo);
		}

		#region Exists

		[Theory]
		[InlineData(2)]
		[InlineData(3)]
		public void ShouldCheckIfCustomerExistsById(int customerId)
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			// When
			var exists = customerRepository.Exists(customerId);

			// Then
			if (customerId == 2)
			{
				Assert.True(exists);
			}
			if (customerId == 3)
			{
				Assert.False(exists);
			}
		}

		#endregion


		#region Create, update, delete

		[Fact]
		public void ShouldCreateCustomer()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();
			var customer = CustomerRepositoryFixture.MockCustomer();

			// When
			var createdId = customerRepository.Create(customer);

			// Then
			Assert.Equal(1, createdId);
		}

		[Fact]
		public void ShouldUpdateCustomer()
		{
			// Given
			var customerRepository = new CustomerRepository();
			var customer = CustomerRepositoryFixture.CreateMockCustomer();

			var createdCustomer = customerRepository.Read(1);
			createdCustomer.FirstName = "New FN";

			// When
			customerRepository.Update(createdCustomer);

			// Then
			var updatedCustomer = customerRepository.Read(1);

			Assert.NotNull(updatedCustomer);
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
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer();

			var createdCustomer = customerRepository.Read(1);
			Assert.NotNull(createdCustomer);

			// When
			customerRepository.Delete(1);

			// Then
			var deletedCustomer = customerRepository.Read(1);
			Assert.Null(deletedCustomer);
		}

		#endregion

		#region Read by Id

		[Fact]
		public void ShouldReadCustomerNotFound()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			// When
			var readCustomer = customerRepository.Read(1);

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
			var customerRepository = new CustomerRepository();
			var customer = createMockCustomer.Invoke();

			// When
			var readCustomer = customerRepository.Read(1);

			// Then
			Assert.NotNull(readCustomer);
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
			var customerRepository = new CustomerRepository();
			var customer = CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			// When
			var readCustomers = customerRepository.ReadAll();

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
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			// When
			var readCustomers = customerRepository.ReadAll();

			// Then
			Assert.Empty(readCustomers);
		}

		#endregion

		#region Get count

		[Fact]
		public void ShouldGetTotalCustomerCount()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 2);

			// When
			var count = customerRepository.GetCount();

			// Then
			Assert.Equal(2, count);
		}

		[Fact]
		public void ShouldGetTotalCustomerCountWhenEmpty()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			// When
			var count = customerRepository.GetCount();

			// Then
			Assert.Equal(0, count);
		}

		#endregion

		#region Read page

		[Fact]
		public void ShouldReadPageOfCustomersFromSinglePage()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 5);

			// When
			var readCustomers = customerRepository.ReadPage(1, 10);

			// Then
			Assert.Equal(5, readCustomers.Count);
		}

		[Fact]
		public void ShouldReadPageOfCustomersFromMultiplePages()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer(amount: 5);

			// When
			var readCustomers = customerRepository.ReadPage(2, 3);

			// Then
			Assert.Equal(2, readCustomers.Count);
			var readCustomersList = readCustomers.ToList();

			Assert.Equal(4, readCustomersList[0].CustomerId);
			Assert.Equal(5, readCustomersList[1].CustomerId);
		}

		[Fact]
		public void ShouldReadPageOfCustomersNotFound()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			// When
			var readCustomers = customerRepository.ReadPage(2, 3);

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
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer("taken@asd.com");

			// When
			var isTakenActual = customerRepository.IsEmailTaken(email);

			// Then
			Assert.Equal(isTakenExpected, isTakenActual);
		}

		[Theory]
		[ClassData(typeof(EmailTakenData))]
		public void ShouldCheckForEmailTakenWithCustomerId(string email, bool isTakenExpected)
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepositoryFixture.CreateMockCustomer("taken@asd.com");

			// When
			var (isTakenActual, takenById) = customerRepository.IsEmailTakenWithCustomerId(email);

			// Then
			Assert.Equal(isTakenExpected, isTakenActual);
			Assert.Equal(isTakenActual ? 1 : 0, takenById);
		}

		#endregion

	}

	public class CustomerRepositoryFixture
	{
		/// <summary>
		/// Clears the Customers table, then creates the specified amount of mocked customers 
		/// with repo-relevant valid properties, optional properties not null.
		/// </summary>
		/// <param name="amount">The amount of customers to create.</param>
		/// <returns>The mocked customer with repo-relevant valid properties, 
		/// optional properties not null.</returns>
		public static Customer CreateMockCustomer(string email = "john@doe.com", int amount = 1)
		{
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			var customer = MockCustomer(email);

			for (int i = 0; i < amount; i++)
			{
				customerRepository.Create(customer);
			}

			return customer;
		}

		/// <summary>
		/// Creates the mocked customers with repo-relevant valid properties,
		/// optional properties null.
		/// </summary>
		/// /// <returns>The mocked customer with repo-relevant valid properties,
		/// optional properties null.</returns>
		public static Customer CreateMockOptionalCustomer()
		{
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			var customer = MockOptionalCustomer();
			customerRepository.Create(customer);

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

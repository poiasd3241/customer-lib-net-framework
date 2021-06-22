using System;
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

		[Fact]
		public void ShouldCreateCustomer()
		{
			// Given
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();
			var customer = CustomerRepositoryFixture.MockCustomer();

			// When, Then
			customerRepository.Create(customer);
		}

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
	}

	public class CustomerRepositoryFixture
	{
		/// <returns>The mocked customer with repo-relevant valid properties, 
		/// optional properties not null, <see cref="Customer.CustomerId"/> = 1.</returns>
		public static Customer CreateMockCustomer(string email = "john@doe.com")
		{
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			var customer = MockCustomer(email);
			customerRepository.Create(customer);

			// Simulate identity.
			customer.CustomerId = 1;
			return customer;
		}

		/// <returns>The mocked customer with repo-relevant valid properties,
		/// optional properties null, <see cref="Customer.CustomerId"/> = 1.</returns>
		public static Customer CreateMockOptionalCustomer()
		{
			var customerRepository = new CustomerRepository();
			CustomerRepository.DeleteAll();

			var customer = MockOptionalCustomer();
			customerRepository.Create(customer);

			// Simulate identity.
			customer.CustomerId = 1;
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

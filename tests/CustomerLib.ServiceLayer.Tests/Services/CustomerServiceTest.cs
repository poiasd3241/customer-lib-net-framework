using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Data.Repositories;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using Moq;
using Xunit;

namespace CustomerLib.ServiceLayer.Tests.Services
{
	public class CustomerServiceTest
	{
		[Fact]
		public void ShouldCreateCustomerService()
		{
			var customerService = new CustomerService();

			Assert.NotNull(customerService);
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
		public void ShouldCheckIfCustomerExistsById(int customerId, bool existsExpected)
		{
			// Given
			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(existsExpected);

			var service = fixture.CreateService();

			// When
			var exists = service.Exists(customerId);

			// Then
			Assert.Equal(existsExpected, exists);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnCheckIfCustomerExistsByBadId()
		{
			// Given
			var service = new CustomerServiceFixture().CreateService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Exists(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		#endregion

		#region Save

		[Fact]
		public void ShouldSave()
		{
			// Given
			var createdCustomerId = 5;
			var customer = CustomerServiceFixture.MockCustomer();

			var email = customer.Email;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.IsEmailTaken(email)).Returns(false);
			fixture.MockCustomerRepository.Setup(r => r.Create(customer)).Returns(createdCustomerId);

			var address = customer.Addresses[0];
			address.CustomerId = createdCustomerId;

			var note = customer.Notes[0];
			note.CustomerId = createdCustomerId;

			fixture.MockAddressService.Setup(s => s.Save(address)).Returns(true);
			fixture.MockNoteService.Setup(s => s.Save(note)).Returns(true);

			var service = fixture.CreateService();

			// When
			service.Save(customer);

			// Then
			fixture.MockCustomerRepository.Verify(r => r.Create(customer), Times.Once);
			fixture.MockCustomerRepository.Verify(r => r.IsEmailTaken(email), Times.Once);
			fixture.MockAddressService.Verify(s => s.Save(address), Times.Once);
			fixture.MockNoteService.Verify(s => s.Save(note), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnSaveByBadCustomer()
		{
			var service = new CustomerServiceFixture().CreateService();

			Assert.Throws<EntityValidationException>(() => service.Save(new Customer()));
		}

		[Fact]
		public void ShouldThrowOnSaveByEmailTaken()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var email = customer.Email;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.IsEmailTaken(email)).Returns(true);

			var service = fixture.CreateService();

			// When
			var exception = Assert.Throws<EmailTakenException>(() => service.Save(customer));

			// Then
			Assert.Equal(email, exception.Message);
			fixture.MockCustomerRepository.Verify(r => r.IsEmailTaken(email), Times.Once);
		}

		#endregion

		#region Get by Id

		public class AddressesAndNotesData : TheoryData<List<Address>, List<Note>>
		{
			public AddressesAndNotesData()
			{
				Add(new() { AddressServiceFixture.MockAddress() },
					new() { NoteServiceFixture.MockNote() });
				Add(new(), new());
			}
		}

		[Theory]
		[ClassData(typeof(AddressesAndNotesData))]
		public void ShouldGetCustomerByIdWithAddressesAndNotes(
			List<Address> addresses, List<Note> notes)
		{
			// Given
			var customerId = 5;
			var customer = CustomerServiceFixture.MockCustomer();
			customer.CustomerId = customerId;
			customer.Addresses = addresses;
			customer.Notes = notes;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Read(customerId)).Returns(customer);

			fixture.MockAddressService.Setup(s => s.FindByCustomer(customerId)).Returns(addresses);
			fixture.MockNoteService.Setup(s => s.FindByCustomer(customerId)).Returns(notes);

			var service = fixture.CreateService();

			// When
			var actualCustomer = service.Get(customer.CustomerId, true, true);

			// Then
			Assert.Equal(customer, actualCustomer);
			fixture.MockCustomerRepository.Verify(r => r.Read(customerId), Times.Once);
			fixture.MockAddressService.Verify(s => s.FindByCustomer(customerId), Times.Once);
			fixture.MockNoteService.Verify(s => s.FindByCustomer(customerId), Times.Once);
		}

		[Fact]
		public void ShouldGetCustomerByIdWithoutAddressesAndNotes()
		{
			// Given
			var expectedCustomer = CustomerServiceFixture.MockRepoCustomer();
			expectedCustomer.CustomerId = 1;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Read(1)).Returns(expectedCustomer);

			var service = fixture.CreateService();

			// When
			var customer = service.Get(1, false, false);

			// Then
			Assert.Equal(expectedCustomer, customer);
			fixture.MockCustomerRepository.Verify(r => r.Read(1), Times.Once);
		}

		[Fact]
		public void ShouldGetNullCustomer()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Read(customerId)).Returns((Customer)null);

			var service = fixture.CreateService();

			// When
			var customer = service.Get(customerId, true, true);

			// Then
			Assert.Null(customer);
			fixture.MockCustomerRepository.Verify(r => r.Read(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetCustomerByBadId()
		{
			var service = new CustomerServiceFixture().CreateService();

			var exception = Assert.Throws<ArgumentException>(() => service.Get(0, false, false));

			Assert.Equal("customerId", exception.ParamName);
		}

		#endregion

		#region Get all

		[Theory]
		[ClassData(typeof(AddressesAndNotesData))]
		public void ShouldGetAllCustomersWithAddressesAndNotes(
			List<Address> addresses, List<Note> notes)
		{
			// Given
			var repoCustomers = new List<Customer>()
			{
				CustomerServiceFixture.MockRepoCustomer(),
				CustomerServiceFixture.MockRepoCustomer()
			};

			repoCustomers[0].CustomerId = 5;
			repoCustomers[1].CustomerId = 7;

			var expectedCustomers = new List<Customer>(repoCustomers);

			foreach (var customer in expectedCustomers)
			{
				customer.Addresses = addresses;
				customer.Notes = notes;
			}

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.ReadAll()).Returns(repoCustomers);
			fixture.MockAddressService.Setup(s => s.FindByCustomer(5)).Returns(addresses);
			fixture.MockAddressService.Setup(s => s.FindByCustomer(7)).Returns(addresses);
			fixture.MockNoteService.Setup(s => s.FindByCustomer(5)).Returns(notes);
			fixture.MockNoteService.Setup(s => s.FindByCustomer(7)).Returns(notes);

			var service = fixture.CreateService();

			// When
			var customers = service.GetAll(true, true);

			// Then
			Assert.Equal(expectedCustomers, customers);
			fixture.MockCustomerRepository.Verify(r => r.ReadAll(), Times.Once);
			fixture.MockAddressService.Verify(s => s.FindByCustomer(5), Times.Once);
			fixture.MockAddressService.Verify(s => s.FindByCustomer(7), Times.Once);
			fixture.MockNoteService.Verify(s => s.FindByCustomer(5), Times.Once);
			fixture.MockNoteService.Verify(s => s.FindByCustomer(7), Times.Once);
		}

		[Fact]
		public void ShouldGetAllCustomersWithoutAddressesAndNotes()
		{
			// Given
			var repoCustomers = new List<Customer>()
			{
				CustomerServiceFixture.MockRepoCustomer(),
				CustomerServiceFixture.MockRepoCustomer()
			};

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.ReadAll()).Returns(repoCustomers);

			var service = fixture.CreateService();

			// When
			var customers = service.GetAll(false, false);

			// Then
			Assert.Equal(repoCustomers, customers);
			fixture.MockCustomerRepository.Verify(r => r.ReadAll(), Times.Once);
		}

		[Fact]
		public void ShouldGetAllCustomersEmpty()
		{
			// Given
			var expectedCustomers = new List<Customer>();
			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.ReadAll()).Returns(expectedCustomers);

			var service = fixture.CreateService();

			// When
			var customers = service.GetAll(false, false);

			// Then
			Assert.Equal(expectedCustomers, customers);
			fixture.MockCustomerRepository.Verify(r => r.ReadAll(), Times.Once);
		}

		#endregion

		#region Get count

		[Fact]
		public void ShouldGetTotalCustomersCount()
		{
			// Given
			var expectedCount = 5;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.GetCount()).Returns(expectedCount);

			var service = fixture.CreateService();

			// When
			var count = service.GetCount();

			// Then
			Assert.Equal(expectedCount, count);
			fixture.MockCustomerRepository.Verify(r => r.GetCount(), Times.Once);
		}

		#endregion

		#region Get page

		[Theory]
		[ClassData(typeof(AddressesAndNotesData))]
		public void ShouldGetPageWithAddressesAndNotes(List<Address> addresses, List<Note> notes)
		{
			// Given
			var page = 2;
			var pageSize = 10;

			var repoCustomers = new List<Customer>()
			{
				CustomerServiceFixture.MockRepoCustomer(),
				CustomerServiceFixture.MockRepoCustomer()
			};

			repoCustomers[0].CustomerId = 5;
			repoCustomers[1].CustomerId = 7;

			var expectedCustomers = new List<Customer>(repoCustomers);

			foreach (var customer in expectedCustomers)
			{
				customer.Addresses = addresses;
				customer.Notes = notes;
			}

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.ReadPage(page, pageSize))
				.Returns(repoCustomers);
			fixture.MockAddressService.Setup(s => s.FindByCustomer(5)).Returns(addresses);
			fixture.MockAddressService.Setup(s => s.FindByCustomer(7)).Returns(addresses);
			fixture.MockNoteService.Setup(s => s.FindByCustomer(5)).Returns(notes);
			fixture.MockNoteService.Setup(s => s.FindByCustomer(7)).Returns(notes);

			var service = fixture.CreateService();

			// When
			var customers = service.GetPage(page, pageSize, true, true);

			// Then
			Assert.Equal(expectedCustomers, customers);
			fixture.MockCustomerRepository.Verify(r => r.ReadPage(page, pageSize), Times.Once);
			fixture.MockAddressService.Verify(s => s.FindByCustomer(5), Times.Once);
			fixture.MockAddressService.Verify(s => s.FindByCustomer(7), Times.Once);
			fixture.MockNoteService.Verify(s => s.FindByCustomer(5), Times.Once);
			fixture.MockNoteService.Verify(s => s.FindByCustomer(7), Times.Once);
		}

		[Fact]
		public void ShouldGetPageWithoutAddressesAndNotes()
		{
			// Given
			var page = 2;
			var pageSize = 10;

			var repoCustomers = new List<Customer>()
			{
				CustomerServiceFixture.MockRepoCustomer(),
				CustomerServiceFixture.MockRepoCustomer()
			};

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.ReadPage(page, pageSize))
				.Returns(repoCustomers);

			var service = fixture.CreateService();

			// When
			var customers = service.GetPage(page, pageSize, false, false);

			// Then
			Assert.Equal(repoCustomers, customers);
			fixture.MockCustomerRepository.Verify(r => r.ReadPage(page, pageSize), Times.Once);
		}

		[Fact]
		public void ShouldGetPageWithTotalCustomersCountCheck()
		{
			// Given
			var page = 2;
			var pageSize = 10;
			var total = 0;
			List<Customer> expectedCustomers = new();

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.GetCount()).Returns(total);
			fixture.MockCustomerRepository.Setup(r => r.ReadPage(page, pageSize))
				.Returns(expectedCustomers);

			var service = fixture.CreateService();

			// When
			var customers = service.GetPage(page, pageSize, true, true, checkTotalSame: true,
				expectedTotal: total);

			// Then
			Assert.Equal(expectedCustomers, customers);
			fixture.MockCustomerRepository.Verify(r => r.GetCount(), Times.Once);
			fixture.MockCustomerRepository.Verify(r => r.ReadPage(page, pageSize), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetPageByTotalCustomersCountChanged()
		{
			// Given
			var page = 2;
			var pageSize = 10;
			var expectedTotal = 0;
			var actualTotal = 5;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.GetCount()).Returns(actualTotal);

			var service = fixture.CreateService();

			// When, Then
			Assert.Throws<DataChangedWhileProcessingException>(() =>
				service.GetPage(page, pageSize, true, true, checkTotalSame: true, expectedTotal));

			fixture.MockCustomerRepository.Verify(r => r.GetCount(), Times.Once);
		}

		public class GetPageArgumentsData : TheoryData<int, int, int, string>
		{
			public GetPageArgumentsData()
			{
				Add(0, 1, 0, "page");
				Add(1, 0, 0, "pageSize");
				Add(1, 1, -1, "expectedTotal");
			}
		}

		[Theory]
		[ClassData(typeof(GetPageArgumentsData))]
		public void ShouldThrowOnGetPageByBadArguments(
			int page, int pageSize, int expectedTotal, string paramName)
		{
			var service = new CustomerServiceFixture().CreateService();

			var exception = Assert.Throws<ArgumentException>(() =>
				service.GetPage(page, pageSize, false, false, checkTotalSame: true, expectedTotal));

			Assert.Equal(paramName, exception.ParamName);
		}

		#endregion

		#region Update

		[Fact]
		public void ShouldUpdateCustomer()
		{
			// Given
			var customerId = 5;
			var customer = CustomerServiceFixture.MockCustomer();
			customer.CustomerId = customerId;
			var email = customer.Email;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(true);
			fixture.MockCustomerRepository.Setup(r => r.IsEmailTakenWithCustomerId(email))
				.Returns((false, 0));
			fixture.MockCustomerRepository.Setup(r => r.Update(customer));

			var service = fixture.CreateService();

			// When
			var result = service.Update(customer);

			// Then
			Assert.True(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
			fixture.MockCustomerRepository.Verify(r => r.IsEmailTakenWithCustomerId(email),
				Times.Once);
			fixture.MockCustomerRepository.Verify(r => r.Update(customer), Times.Once);
		}

		[Fact]
		public void ShouldNotUpdateNotFoundCustomer()
		{
			// Given
			var customerId = 5;
			var customer = CustomerServiceFixture.MockCustomer();
			customer.CustomerId = customerId;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Update(customer);

			// Then
			Assert.False(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByEmailTakenByOtherCustomer()
		{
			// Given
			var customerId = 5;
			var customer = CustomerServiceFixture.MockCustomer();
			customer.CustomerId = customerId;
			var email = customer.Email;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(true);
			fixture.MockCustomerRepository.Setup(r => r.IsEmailTakenWithCustomerId(email))
				.Returns((true, 3241));

			var service = fixture.CreateService();

			// When
			var exception = Assert.Throws<EmailTakenException>(() => service.Update(customer));

			// Then
			Assert.Equal(email, exception.Message);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
			fixture.MockCustomerRepository.Verify(r => r.IsEmailTakenWithCustomerId(email),
				Times.Once);
		}

		[Fact]
		public void ShouldThrowOnUpdateByBadCustomer()
		{
			var service = new CustomerServiceFixture().CreateService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Customer()));
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDeleteCustomer()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(true);
			fixture.MockCustomerRepository.Setup(r => r.Delete(customerId));
			fixture.MockAddressService.Setup(s => s.DeleteByCustomer(customerId));

			var service = fixture.CreateService();

			// When
			var result = service.Delete(customerId);

			// Then
			Assert.True(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
			fixture.MockAddressService.Verify(s => s.DeleteByCustomer(customerId), Times.Once);
			fixture.MockCustomerRepository.Verify(r => r.Delete(customerId), Times.Once);
		}

		[Fact]
		public void ShouldNotDeleteNotFoundCustomer()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomerServiceFixture();
			fixture.MockCustomerRepository.Setup(r => r.Exists(customerId)).Returns(false);

			var service = fixture.CreateService();

			// When
			var result = service.Delete(customerId);

			// Then
			Assert.False(result);
			fixture.MockCustomerRepository.Verify(r => r.Exists(customerId), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Given
			var service = new CustomerServiceFixture().CreateService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Delete(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		#endregion
	}

	public class CustomerServiceFixture
	{
		public Mock<ICustomerRepository> MockCustomerRepository { get; set; }
		public Mock<IAddressService> MockAddressService { get; set; }
		public Mock<INoteService> MockNoteService { get; set; }
		public CustomerServiceFixture()
		{
			MockCustomerRepository = new(MockBehavior.Strict);
			MockAddressService = new(MockBehavior.Strict);
			MockNoteService = new(MockBehavior.Strict);
		}

		public CustomerService CreateService() => new(MockCustomerRepository.Object,
			MockAddressService.Object, MockNoteService.Object);

		/// <returns>The mocked customer with repo-relevant properties
		/// with valid addresses and notes. Optional properties not null.</returns>
		public static Customer MockCustomer(string email = "john@doe.com") => new()
		{
			FirstName = "a",
			LastName = "a",
			Addresses = new() { AddressServiceFixture.MockAddress() },
			PhoneNumber = "+123",
			Email = email,
			Notes = new() { NoteServiceFixture.MockNote() },
			TotalPurchasesAmount = 123,
		};

		/// <returns>The mocked customer with repo-relevant properties 
		/// with null addresses and notes. Optional properties not null.</returns>
		public static Customer MockRepoCustomer(string email = "john@doe.com") => new()
		{
			FirstName = "a",
			LastName = "a",
			Addresses = null,
			PhoneNumber = "+123",
			Email = email,
			Notes = null,
			TotalPurchasesAmount = 123,
		};
	}
}

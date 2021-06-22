using System;
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

		#region Save

		[Fact]
		public void ShouldSave()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var address = customer.Addresses[0];
			var note = customer.Notes[0];
			var email = customer.Email;

			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.Create(customer));
			customerRepoMock.Setup(r => r.IsEmailTaken(email)).Returns(false);

			var addressServiceMock = CustomerServiceFixture.MockAddressService();
			addressServiceMock.Setup(s => s.Save(address));

			var noteServiceMock = CustomerServiceFixture.MockNoteService();
			noteServiceMock.Setup(s => s.Save(note));

			var service = new CustomerService(customerRepoMock.Object, addressServiceMock.Object,
				noteServiceMock.Object);

			// When
			service.Save(customer);

			// Then
			customerRepoMock.Verify(r => r.Create(customer), Times.Once);
			customerRepoMock.Verify(r => r.IsEmailTaken(email), Times.Once);
			addressServiceMock.Verify(s => s.Save(address), Times.Once);
			noteServiceMock.Verify(s => s.Save(note), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnSaveByBadCustomer()
		{
			var service = CustomerServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Save(new Customer()));
		}

		[Fact]
		public void ShouldThrowOnSaveByEmailTaken()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var email = customer.Email;

			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.IsEmailTaken(email)).Returns(true);
			var service = new CustomerService(customerRepoMock.Object,
				CustomerServiceFixture.MockAddressService().Object,
				CustomerServiceFixture.MockNoteService().Object);

			// When
			var exception = Assert.Throws<EmailTakenException>(() => service.Save(customer));

			// Then
			Assert.Equal(email, exception.Message);
			customerRepoMock.Verify(r => r.IsEmailTaken(email), Times.Once);
		}

		[Fact]
		public void ShouldRethrowOnSave()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var email = customer.Email;
			var expectedException = new Exception("oops");

			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.IsEmailTaken(email)).Throws(expectedException);

			var service = new CustomerService(customerRepoMock.Object,
				CustomerServiceFixture.MockAddressService().Object,
				CustomerServiceFixture.MockNoteService().Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Save(customer));

			// Then
			Assert.Equal(expectedException, exception);
			customerRepoMock.Verify(r => r.IsEmailTaken(email), Times.Once);
		}

		#endregion

		#region Get by Id

		[Fact]
		public void ShouldGetCustomerById()
		{
			// Given
			var expectedCustomer = CustomerServiceFixture.MockCustomer();
			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.Read(1)).Returns(expectedCustomer);

			var service = new CustomerService(customerRepoMock.Object,
				CustomerServiceFixture.MockAddressService().Object,
				CustomerServiceFixture.MockNoteService().Object);

			// When
			var customer = service.Get(1);

			// Then
			Assert.Equal(expectedCustomer, customer);
			customerRepoMock.Verify(r => r.Read(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnGetCustomerByBadId()
		{
			var service = CustomerServiceFixture.MockService();

			var exception = Assert.Throws<ArgumentException>(() => service.Get(0));

			Assert.Equal("customerId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnGetCustomerById()
		{
			// Given
			var expectedException = new Exception("oops");
			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.Read(1)).Throws(expectedException);

			var service = new CustomerService(customerRepoMock.Object,
				CustomerServiceFixture.MockAddressService().Object,
				CustomerServiceFixture.MockNoteService().Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Get(1));

			// Then
			Assert.Equal(expectedException, exception);
			customerRepoMock.Verify(r => r.Read(1), Times.Once);
		}

		#endregion

		#region Update

		[Fact]
		public void ShouldUpdate()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var address = customer.Addresses[0];
			var note = customer.Notes[0];
			var email = customer.Email;

			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.Update(customer));
			customerRepoMock.Setup(r => r.IsEmailTakenWithCustomerId(email)).Returns((false, 0));

			var addressServiceMock = CustomerServiceFixture.MockAddressService();
			addressServiceMock.Setup(s => s.Update(address));

			var noteServiceMock = CustomerServiceFixture.MockNoteService();
			noteServiceMock.Setup(s => s.Update(note));

			var service = new CustomerService(customerRepoMock.Object, addressServiceMock.Object,
				noteServiceMock.Object);

			// When
			service.Update(customer);

			// Then
			customerRepoMock.Verify(r => r.Update(customer), Times.Once);
			customerRepoMock.Verify(r => r.IsEmailTakenWithCustomerId(email), Times.Once);
			addressServiceMock.Verify(s => s.Update(address), Times.Once);
			noteServiceMock.Verify(s => s.Update(note), Times.Once);
		}


		[Fact]
		public void ShouldThrowOnUpdateByEmailTakenByOtherCustomer()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var email = customer.Email;

			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.IsEmailTakenWithCustomerId(email)).Returns((true, 3241));
			var service = new CustomerService(customerRepoMock.Object,
				CustomerServiceFixture.MockAddressService().Object,
				CustomerServiceFixture.MockNoteService().Object);

			// When
			var exception = Assert.Throws<EmailTakenException>(() => service.Update(customer));

			// Then
			Assert.Equal(email, exception.Message);
			customerRepoMock.Verify(r => r.IsEmailTakenWithCustomerId(email), Times.Once);
		}


		[Fact]
		public void ShouldThrowOnUpdateByBadCustomer()
		{
			var service = CustomerServiceFixture.MockService();

			Assert.Throws<EntityValidationException>(() => service.Update(new Customer()));
		}

		[Fact]
		public void ShouldRethrowOnUpdate()
		{
			// Given
			var customer = CustomerServiceFixture.MockCustomer();
			var email = customer.Email;
			var expectedException = new Exception("oops");

			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.IsEmailTakenWithCustomerId(email)).Throws(expectedException);

			var service = new CustomerService(customerRepoMock.Object,
				CustomerServiceFixture.MockAddressService().Object,
				CustomerServiceFixture.MockNoteService().Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Update(customer));

			// Then
			Assert.Equal(expectedException, exception);
			customerRepoMock.Verify(r => r.IsEmailTakenWithCustomerId(email), Times.Once);
		}

		#endregion

		#region Delete

		[Fact]
		public void ShouldDelete()
		{
			// Given
			var customerRepoMock = CustomerServiceFixture.MockCustomerRepository();
			customerRepoMock.Setup(r => r.Delete(1));

			var addressServiceMock = CustomerServiceFixture.MockAddressService();
			addressServiceMock.Setup(s => s.DeleteByCustomer(1));

			var service = new CustomerService(customerRepoMock.Object, addressServiceMock.Object,
							CustomerServiceFixture.MockNoteService().Object);

			// When
			service.Delete(1);

			// Then
			addressServiceMock.Verify(s => s.DeleteByCustomer(1), Times.Once);
			customerRepoMock.Verify(r => r.Delete(1), Times.Once);
		}

		[Fact]
		public void ShouldThrowOnDeleteByBadId()
		{
			// Given
			var service = CustomerServiceFixture.MockService();

			// When
			var exception = Assert.Throws<ArgumentException>(() => service.Delete(0));

			// Then
			Assert.Equal("customerId", exception.ParamName);
		}

		[Fact]
		public void ShouldRethrowOnDelete()
		{
			// Given
			var expectedException = new Exception("oops");

			var addressServiceMock = CustomerServiceFixture.MockAddressService();
			addressServiceMock.Setup(s => s.DeleteByCustomer(1)).Throws(expectedException);

			var service = new CustomerService(
				CustomerServiceFixture.MockCustomerRepository().Object,
				addressServiceMock.Object, CustomerServiceFixture.MockNoteService().Object);

			// When
			var exception = Assert.Throws<Exception>(() => service.Delete(1));

			// Then
			Assert.Equal(expectedException, exception);
			addressServiceMock.Verify(s => s.DeleteByCustomer(1), Times.Once);
		}

		#endregion
	}

	public class CustomerServiceFixture
	{
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

		public static Mock<ICustomerRepository> MockCustomerRepository() =>
				new(MockBehavior.Strict);

		public static Mock<IAddressService> MockAddressService() => new(MockBehavior.Strict);
		public static Mock<INoteService> MockNoteService() => new(MockBehavior.Strict);

		/// <summary>
		/// Use when dependencies don't need to be mocked (are unused).
		/// </summary>
		public static CustomerService MockService() =>
				new(MockCustomerRepository().Object,
					AddressServiceFixture.MockService(),
					NoteServiceFixture.MockService());
	}
}

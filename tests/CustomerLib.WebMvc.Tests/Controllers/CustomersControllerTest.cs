using System.Collections.Generic;
using System.Web.Mvc;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Localization;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Controllers;
using CustomerLib.WebMvc.Models;
using CustomerLib.WebMvc.Models.Addresses;
using CustomerLib.WebMvc.Models.Customers;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Controllers
{
	public class CustomersControllerTest
	{
		[Fact]
		public void ShouldCreateCustomersControllerDefaultConstructor()
		{
			var controller = new CustomersController();

			Assert.NotNull(controller);
		}

		[Fact]
		public void ShouldCreateCustomersController()
		{
			var mockCustomerService = new StrictMock<ICustomerService>();
			var mockCustomerModelsMapper = new StrictMock<ICustomerModelsMapper>();

			var controller = new CustomersController(mockCustomerService.Object,
				mockCustomerModelsMapper.Object);

			Assert.NotNull(controller);
		}

		#region Index - GET

		[Fact]
		public void ShouldReturnIndexViewOnGetIndexByCustomersEmpty()
		{
			// Given
			var page = 3;
			var pageSize = 6;
			var customers = new List<Customer>();

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s =>
				s.GetPage(page, pageSize, false, false, false, 0)).Returns(customers);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index(page, pageSize);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomersModel);
			var customersModel = (CustomersModel)model;

			Assert.Equal(customers, customersModel.Customers);
			Assert.Equal(0, customersModel.CustomersCount);
			Assert.False(customersModel.HasCustomers);
			Assert.Equal(page, customersModel.Page);
			Assert.Equal(pageSize, customersModel.PageSize);
			Assert.Equal(0, customersModel.TotalPages);
		}

		[Fact]
		public void ShouldReturnIndexViewOnGetIndexByCustomersEmptyWithDefaultArguments()
		{
			// Given
			var defaultPage = 1;
			var defaultPageSize = 10;
			var customers = new List<Customer>();

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s =>
				s.GetPage(defaultPage, defaultPageSize, false, false, false, 0)).Returns(customers);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index();

			// Then	
			var model = result.Model;
			Assert.True(model is CustomersModel);
			var customersModel = (CustomersModel)model;

			Assert.Equal(customers, customersModel.Customers);
			Assert.Equal(0, customersModel.CustomersCount);
			Assert.False(customersModel.HasCustomers);
			Assert.Equal(defaultPage, customersModel.Page);
			Assert.Equal(defaultPageSize, customersModel.PageSize);
			Assert.Equal(0, customersModel.TotalPages);
		}

		[Fact]
		public void ShouldReturnIndexViewOnGetIndexByCustomersNotEmpty()
		{
			// Given
			var page = 3;
			var pageSize = 15;
			var customers = MockCustomers(64);

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s =>
			s.GetPage(page, pageSize, false, false, false, 0)).Returns(customers);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index(page, pageSize);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomersModel);
			var customersModel = (CustomersModel)model;

			Assert.Equal(customers, customersModel.Customers);
			Assert.Equal(64, customersModel.CustomersCount);
			Assert.True(customersModel.HasCustomers);
			Assert.Equal(page, customersModel.Page);
			Assert.Equal(pageSize, customersModel.PageSize);
			Assert.Equal(5, customersModel.TotalPages);
		}

		#endregion

		#region Create - GET

		[Fact]
		public void ShouldReturnCreateViewOnGetCreate()
		{
			// Given
			var controller = new CustomersControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Create();

			// Then	
			var model = result.Model;
			Assert.True(model is CustomerCreateModel);

			var createModel = (CustomerCreateModel)model;

			Assert.True(createModel.BasicDetails is CustomerBasicDetailsModel);

			Assert.Null(createModel.BasicDetails.FirstName);
			Assert.Null(createModel.BasicDetails.LastName);
			Assert.Null(createModel.BasicDetails.PhoneNumber);
			Assert.Null(createModel.BasicDetails.Email);
			Assert.Null(createModel.BasicDetails.TotalPurchasesAmount);

			Assert.True(createModel.AddressDetails is AddressDetailsModel);

			Assert.Null(createModel.AddressDetails.Address.AddressLine);
			Assert.Null(createModel.AddressDetails.Address.AddressLine2);
			Assert.Null(createModel.AddressDetails.Address.City);
			Assert.Null(createModel.AddressDetails.Address.PostalCode);
			Assert.Null(createModel.AddressDetails.Address.State);
			Assert.Null(createModel.AddressDetails.Address.Country);

			Assert.Null(createModel.Note.Content);
		}

		#endregion

		#region Create - POST

		[Fact]
		public void ShouldReturnCreateViewOnPostCreateByInvalidModel()
		{
			// Given
			var customer = MockCustomer();
			customer.FirstName = "  ";

			var address = MockAddress();
			var note = MockNote();

			var invalidModel = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var controller = new CustomersControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Create(invalidModel);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomerCreateModel);

			var createModel = (CustomerCreateModel)model;
			Assert.Equal(invalidModel, createModel);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("BasicDetails.FirstName", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal(ValidationRules.PERSON_FIRST_NAME_EMPTY_OR_WHITESPACE, error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnCreateViewOnPostCreateByEmailTaken()
		{
			// Given
			var email = "taken@email.com";

			var customer = MockCustomer();
			customer.Email = email;

			var address = MockAddress();
			var note = MockNote();

			var createModelWithEmailTaken = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerModelsMapper.Setup(m => m.ToEntity(createModelWithEmailTaken))
				.Returns(customer);
			fixture.MockCustomerService.Setup(s => s.Save(customer)).Throws<EmailTakenException>();

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(createModelWithEmailTaken);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomerCreateModel);

			var createModel = (CustomerCreateModel)model;
			Assert.Equal(createModelWithEmailTaken, createModel);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("BasicDetails.Email", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal("taken@email.com is already taken", error.ErrorMessage);
		}

		[Fact]
		public void ShouldRedirectToIndexOnPostCreateSuccess()
		{
			// Given
			var customer = MockCustomer();
			var address = MockAddress();
			var note = MockNote();

			var createModel = new CustomerCreateModel(customer, address)
			{
				Note = note
			};

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerModelsMapper.Setup(m => m.ToEntity(createModel))
				.Returns(customer);
			fixture.MockCustomerService.Setup(s => s.Save(customer));

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Create(createModel);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Edit - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetEditByCustomerNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnEditViewOnGetEdit()
		{
			// Given
			var customerId = 5;

			var customer = MockCustomer();
			customer.CustomerId = customerId;

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomerEditModel);
			var createModel = (CustomerEditModel)model;

			Assert.Equal(customer.CustomerId, createModel.CustomerId);
			Assert.Equal(customer.FirstName, createModel.FirstName);
			Assert.Equal(customer.LastName, createModel.LastName);
			Assert.Equal(customer.PhoneNumber, createModel.PhoneNumber);
			Assert.Equal(customer.Email, createModel.Email);
			Assert.Equal(customer.TotalPurchasesAmount.ToString(),
				createModel.TotalPurchasesAmount);
		}

		#endregion

		#region Edit - POST

		[Fact]
		public void ShouldReturnEditViewOnPostEditByInvalidModel()
		{
			// Given
			var customer = MockCustomer();
			customer.FirstName = "  ";

			var invalidModel = new CustomerEditModel(customer);

			var controller = new CustomersControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Edit(invalidModel);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomerEditModel);

			var editModel = (CustomerEditModel)model;
			Assert.Equal(invalidModel, editModel);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("FirstName", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal(ValidationRules.PERSON_FIRST_NAME_EMPTY_OR_WHITESPACE, error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnCreateViewOnPostEditByEmailTaken()
		{
			// Given
			var email = "taken@email.com";

			var customer = MockCustomer();
			customer.Email = email;

			var editModelWithEmailTaken = new CustomerEditModel(customer);

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerModelsMapper.Setup(m => m.ToEntity(editModelWithEmailTaken))
				.Returns(customer);
			fixture.MockCustomerService.Setup(s => s.Update(customer))
				.Throws<EmailTakenException>();

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(editModelWithEmailTaken);

			// Then	
			var model = result.Model;
			Assert.True(model is CustomerEditModel);

			var editModel = (CustomerEditModel)model;
			Assert.Equal(editModelWithEmailTaken, editModel);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("Email", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal("taken@email.com is already taken", error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnFailureViewOnPostEditByUpdateFail()
		{
			// Given
			var customer = MockCustomer();

			var editModel = new CustomerEditModel(customer);

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerModelsMapper.Setup(m => m.ToEntity(editModel))
				.Returns(customer);
			fixture.MockCustomerService.Setup(s => s.Update(customer)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(editModel);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Customer_Edit", failureModel.Title);
			Assert.Equal("Cannot edit the customer #4: it doesn't exist!", failureModel.Message);
			Assert.Equal("To customers list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Customers", failureModel.ControllerName);
			Assert.Null(failureModel.RouteValues);
		}

		[Fact]
		public void ShouldRedirectToIndexOnPostEditSuccess()
		{
			// Given
			var customer = MockCustomer();

			var editModel = new CustomerEditModel(customer);

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerModelsMapper.Setup(m => m.ToEntity(editModel))
				.Returns(customer);
			fixture.MockCustomerService.Setup(s => s.Update(customer)).Returns(true);

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Edit(editModel);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Delete - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetDeleteByCustomerNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Delete(customerId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnFailureViewOnGetDeleteFailNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(true);
			fixture.MockCustomerService.Setup(s => s.Delete(customerId)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Delete(customerId);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Customer_Delete", failureModel.Title);
			Assert.Equal("Cannot delete the customer #5: it doesn't exist!", failureModel.Message);
			Assert.Equal("To customers list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Customers", failureModel.ControllerName);
			Assert.Null(failureModel.RouteValues);
		}

		[Fact]
		public void ShouldRedirectToIndexViewOnGetDeleteSuccess()
		{
			// Given
			var customerId = 5;

			var fixture = new CustomersControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(true);
			fixture.MockCustomerService.Setup(s => s.Delete(customerId)).Returns(true);

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Delete(customerId);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Fixture, object mock helpers

		public class CustomersControllerFixture
		{
			public StrictMock<ICustomerService> MockCustomerService { get; set; }
			public StrictMock<ICustomerModelsMapper> MockCustomerModelsMapper { get; set; }

			public CustomersControllerFixture()
			{
				MockCustomerService = new();
				MockCustomerModelsMapper = new();
			}

			public CustomersController CreateController() => new(MockCustomerService.Object,
				MockCustomerModelsMapper.Object);
		}

		public static Customer MockCustomer() => new()
		{
			CustomerId = 4,
			FirstName = "One",
			LastName = "Two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = 666,
			Addresses = null,
			Notes = null
		};

		public static List<Customer> MockCustomers(int count)
		{
			var result = new List<Customer>();

			for (int i = 0; i < count; i++)
			{
				result.Add(MockCustomer());
			}

			return result;
		}

		private static Address MockAddress() => new()
		{
			AddressLine = "one",
			Type = AddressType.Billing,
			City = "city x",
			PostalCode = "666",
			State = "state x",
			Country = "Canada"
		};

		private static Note MockNote() => new()
		{
			Content = "text"
		};

		#endregion
	}
}

using System.Collections.Generic;
using System.Web.Mvc;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Localization;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Controllers;
using CustomerLib.WebMvc.Models;
using CustomerLib.WebMvc.Models.Addresses;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Controllers
{
	public class AddressesControllerTest
	{
		[Fact]
		public void ShouldCreateAddressesControllerDefaultConstructor()
		{
			var controller = new AddressesController();

			Assert.NotNull(controller);
		}

		[Fact]
		public void ShouldCreateAddressesController()
		{
			var mockCustomerService = new StrictMock<ICustomerService>();
			var mockAddressService = new StrictMock<IAddressService>();
			var mockAddressModelsMapper = new StrictMock<IAddressModelsMapper>();

			var controller = new AddressesController(mockCustomerService.Object,
				mockAddressService.Object, mockAddressModelsMapper.Object);

			Assert.NotNull(controller);
		}

		#region Index - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetIndexByCustomerNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, true, false))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index(customerId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnIndexViewOnIndexGet()
		{
			// Given
			var customerId = 5;
			var addresses = new List<Address>() { MockAddress() };

			var customer = MockCustomer();
			customer.CustomerId = 5;
			customer.Addresses = addresses;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, true, false))
				.Returns(customer);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Index(customerId);

			// Then	
			var model = result.Model;
			Assert.True(model is AddressesModel);
			var addressesModel = (AddressesModel)model;

			Assert.Equal("Addresses for the customer One Two (a@a.aa)", addressesModel.Title);
			Assert.True(addressesModel.HasAddresses);
			Assert.Equal(addresses, addressesModel.Addresses);
		}

		#endregion

		#region Create - GET

		[Fact]
		public void ShouldReturnNotFoundViewOnGetCreateByCustomerNotFound()
		{
			// Given
			var customerId = 5;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(customerId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnCreateViewOnGetCreate()
		{
			// Given
			var customerId = 5;
			var addresses = new List<Address>() { MockAddress() };

			var customer = MockCustomer();
			customer.CustomerId = 5;
			customer.Addresses = addresses;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(customerId);

			// Then	
			var model = result.Model;
			Assert.True(model is AddressEditModel);
			var createModel = (AddressEditModel)model;

			Assert.Equal("New address for the customer One Two (a@a.aa)", createModel.Title);
			Assert.Equal("Create", createModel.SubmitButtonText);

			Assert.Equal(customerId, createModel.AddressDetails.Address.CustomerId);
			Assert.Null(createModel.AddressDetails.Address.AddressLine);
			Assert.Null(createModel.AddressDetails.Address.AddressLine2);
			Assert.Null(createModel.AddressDetails.Address.City);
			Assert.Null(createModel.AddressDetails.Address.PostalCode);
			Assert.Null(createModel.AddressDetails.Address.State);
			Assert.Null(createModel.AddressDetails.Address.Country);
		}

		#endregion

		#region Create - POST

		[Fact]
		public void ShouldReturnCreateViewOnPostCreateByInvalidModel()
		{
			// Given
			var invalidAddress = MockAddress();
			invalidAddress.AddressLine = "  ";

			var title = "create";
			var submitButtonText = "submit";

			var invalidModel = new AddressEditModel(invalidAddress)
			{
				Title = title,
				SubmitButtonText = submitButtonText
			};

			var controller = new AddressesControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Create(invalidModel);

			// Then	
			var model = result.Model;
			Assert.True(model is AddressEditModel);

			var createModel = (AddressEditModel)model;
			Assert.Equal(title, createModel.Title);
			Assert.Equal(submitButtonText, createModel.SubmitButtonText);
			Assert.Equal(invalidAddress, createModel.AddressDetails.Address);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("AddressDetails.Address.AddressLine", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal(ValidationRules.ADDRESS_LINE_EMPTY_OR_WHITESPACE, error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnFailureViewOnPostCreateBySaveFail()
		{
			// Given
			var address = MockAddress();

			var createModel = new AddressEditModel(address);

			var fixture = new AddressesControllerFixture();
			fixture.MockAddressModelsMapper.Setup(m => m.ToEntity(createModel)).Returns(address);
			fixture.MockAddressService.Setup(s => s.Save(address)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Create(createModel);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Address_Create", failureModel.Title);
			Assert.Equal("Cannot create an address for the non-existing customer #5!",
				failureModel.Message);
			Assert.Equal("To customers list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Customers", failureModel.ControllerName);
			Assert.Null(failureModel.RouteValues);
		}

		[Fact]
		public void ShouldRedirectToIndexOnPostCreateSuccess()
		{
			// Given
			var address = MockAddress();

			var createModel = new AddressEditModel(address);

			var fixture = new AddressesControllerFixture();
			fixture.MockAddressModelsMapper.Setup(m => m.ToEntity(createModel)).Returns(address);
			fixture.MockAddressService.Setup(s => s.Save(address)).Returns(true);

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
			var addressId = 7;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns((Customer)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId, addressId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnNotFoundViewOnGetEditByAddressNotFound()
		{
			// Given
			var customerId = 5;
			var addressId = 7;

			var customer = MockCustomer();
			customer.CustomerId = customerId;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);
			fixture.MockAddressService.Setup(s => s.Get(addressId)).Returns((Address)null);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId, addressId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnNotFoundViewOnGetEditByCustomerIdMismatch()
		{
			// Given
			var customerIdCustomer = 5;
			var customerIdAddress = 8;
			var addressId = 7;

			var customer = MockCustomer();
			customer.CustomerId = customerIdCustomer;

			var address = MockAddress();
			address.CustomerId = customerIdAddress;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerIdCustomer, false, false))
				.Returns(customer);
			fixture.MockAddressService.Setup(s => s.Get(addressId)).Returns(address);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerIdCustomer, addressId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnEditViewOnGetEdit()
		{
			// Given
			var customerId = 5;
			var addressId = 7;

			var customer = MockCustomer();
			customer.CustomerId = customerId;

			var address = MockAddress();
			address.AddressId = addressId;
			address.CustomerId = customerId;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Get(customerId, false, false))
				.Returns(customer);
			fixture.MockAddressService.Setup(s => s.Get(addressId)).Returns(address);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(customerId, addressId);

			// Then	
			var model = result.Model;
			Assert.True(model is AddressEditModel);
			var createModel = (AddressEditModel)model;

			Assert.Equal("Edit the address for the customer One Two (a@a.aa)", createModel.Title);
			Assert.Equal("Save", createModel.SubmitButtonText);
			Assert.True(createModel.AddressDetails.Address.EqualsByValue(address));
		}

		#endregion

		#region Edit - POST

		[Fact]
		public void ShouldReturnEditViewOnPostEditByInvalidModel()
		{
			// Given
			var invalidAddress = MockAddress();
			invalidAddress.AddressLine = "  ";

			var title = "edit";
			var submitButtonText = "submit";

			var invalidModel = new AddressEditModel(invalidAddress)
			{
				Title = title,
				SubmitButtonText = submitButtonText
			};

			var controller = new AddressesControllerFixture().CreateController();

			// When
			var result = (ViewResult)controller.Edit(invalidModel);

			// Then	
			var model = result.Model;
			Assert.True(model is AddressEditModel);

			var createModel = (AddressEditModel)model;
			Assert.Equal(title, createModel.Title);
			Assert.Equal(submitButtonText, createModel.SubmitButtonText);
			Assert.Equal(invalidAddress, createModel.AddressDetails.Address);

			var modelState = Assert.Single(result.ViewData.ModelState);
			Assert.Equal("AddressDetails.Address.AddressLine", modelState.Key);

			var error = Assert.Single(modelState.Value.Errors);
			Assert.Equal(ValidationRules.ADDRESS_LINE_EMPTY_OR_WHITESPACE, error.ErrorMessage);
		}

		[Fact]
		public void ShouldReturnFailureViewOnPostEditByUpdateFail()
		{
			// Given
			var address = MockAddress();

			var editModel = new AddressEditModel(address);

			var fixture = new AddressesControllerFixture();
			fixture.MockAddressModelsMapper.Setup(m => m.ToEntity(editModel)).Returns(address);
			fixture.MockAddressService.Setup(s => s.Update(address)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Edit(editModel);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Address_Edit", failureModel.Title);
			Assert.Equal("Cannot edit the address #3: it doesn't exist!", failureModel.Message);
			Assert.Equal("To addresses list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Addresses", failureModel.ControllerName);

			var routeValue = Assert.Single(failureModel.RouteValues.GetType().GetProperties());
			Assert.Equal("customerId", routeValue.Name);
			Assert.Equal(5, routeValue.GetValue(failureModel.RouteValues));
		}

		[Fact]
		public void ShouldRedirectToIndexOnPostEditSuccess()
		{
			// Given
			var address = MockAddress();

			var editModel = new AddressEditModel(address);

			var fixture = new AddressesControllerFixture();
			fixture.MockAddressModelsMapper.Setup(m => m.ToEntity(editModel)).Returns(address);
			fixture.MockAddressService.Setup(s => s.Update(address)).Returns(true);

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
			var addressId = 7;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Delete(customerId, addressId);

			// Then	
			Assert.Equal("NotFound", result.ViewName);
		}

		[Fact]
		public void ShouldReturnFailureViewOnGetDeleteByAddressNotFound()
		{
			// Given
			var customerId = 5;
			var addressId = 7;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(true);
			fixture.MockAddressService.Setup(s => s.Delete(addressId)).Returns(false);

			var controller = fixture.CreateController();

			// When
			var result = (ViewResult)controller.Delete(customerId, addressId);

			// Then	
			Assert.Equal("Failure", result.ViewName);

			var model = result.Model;
			Assert.True(model is FailureModel);

			var failureModel = (FailureModel)model;
			Assert.Equal("Address_Delete", failureModel.Title);
			Assert.Equal("Cannot delete the address #7: it doesn't exist!", failureModel.Message);
			Assert.Equal("To addresses list", failureModel.LinkText);
			Assert.Equal("Index", failureModel.ActionName);
			Assert.Equal("Addresses", failureModel.ControllerName);
			Assert.Null(failureModel.RouteValues);
		}

		[Fact]
		public void ShouldRedirectToIndexViewOnGetDeleteSuccess()
		{
			// Given
			var customerId = 5;
			var addressId = 7;

			var fixture = new AddressesControllerFixture();
			fixture.MockCustomerService.Setup(s => s.Exists(customerId)).Returns(true);
			fixture.MockAddressService.Setup(s => s.Delete(addressId)).Returns(true);

			var controller = fixture.CreateController();

			// When
			var result = (RedirectToRouteResult)controller.Delete(customerId, addressId);

			// Then	
			var routeValue = Assert.Single(result.RouteValues);

			Assert.Equal("action", routeValue.Key);
			Assert.Equal("Index", routeValue.Value);
		}

		#endregion

		#region Fixture, object mock helpers

		public class AddressesControllerFixture
		{
			public StrictMock<ICustomerService> MockCustomerService { get; set; }
			public StrictMock<IAddressService> MockAddressService { get; set; }
			public StrictMock<IAddressModelsMapper> MockAddressModelsMapper { get; set; }

			public AddressesControllerFixture()
			{
				MockCustomerService = new();
				MockAddressService = new();
				MockAddressModelsMapper = new();
			}

			public AddressesController CreateController() =>
				new(MockCustomerService.Object, MockAddressService.Object,
					MockAddressModelsMapper.Object);
		}

		public static Customer MockCustomer() => new()
		{
			CustomerId = 5,
			FirstName = "One",
			LastName = "Two",
			PhoneNumber = "+123",
			Email = "a@a.aa",
			TotalPurchasesAmount = 666,
			Addresses = null,
			Notes = null
		};

		public static Address MockAddress() => new()
		{
			AddressId = 3,
			CustomerId = 5,
			AddressLine = "line",
			AddressLine2 = "line2",
			Type = AddressType.Shipping,
			City = "city",
			PostalCode = "code",
			State = "state",
			Country = "United States"
		};

		#endregion
	}
}

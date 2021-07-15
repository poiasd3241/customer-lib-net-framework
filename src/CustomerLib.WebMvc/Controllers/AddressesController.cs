using System.Web.Mvc;
using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebMvc.Models;
using CustomerLib.WebMvc.Models.Addresses;
using CustomerLib.WebMvc.ViewHelpers;

namespace CustomerLib.WebMvc.Controllers
{
	[Route("Customers/{customerId:int}/Addresses/{action=Index}/{addressId:int?}")]
	public class AddressesController : Controller
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly IAddressService _addressService;
		private readonly IAddressModelsMapper _addressModelsMapper;
		private readonly AddressModelsValidator _addressModelsValidator = new();

		#endregion

		#region Constructors

		public AddressesController()
		{
			_customerService = new CustomerService();
			_addressService = new AddressService();
		}

		public AddressesController(ICustomerService customerService, IAddressService addressService,
			IAddressModelsMapper addressModelsMapper)
		{
			_customerService = customerService;
			_addressService = addressService;
			_addressModelsMapper = addressModelsMapper;
		}

		#endregion

		#region Actions

		// GET: Customers/4/Addresses
		public ActionResult Index(int customerId)
		{
			var customer = _customerService.Get(customerId, true, false);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			var addressesModel = new AddressesModel(customer.Addresses)
			{
				Title = TitleHelper.GetTitleAddresses(customer)
			};

			return View(addressesModel);
		}

		// GET: Customers/4/Addresses/Create
		public ActionResult Create(int customerId)
		{
			var customer = _customerService.Get(customerId, false, false);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			var addressCreateModel = new AddressEditModel(new Address() { CustomerId = customerId })
			{
				Title = TitleHelper.GetTitleAddressCreate(customer),
				SubmitButtonText = "Create"
			};

			return View(addressCreateModel);
		}

		// POST: Customers/4/Addresses/Create
		[HttpPost]
		public ActionResult Create(AddressEditModel model)
		{
			var errors = _addressModelsValidator.ValidateEditModel(model);

			ModelStateHelper.AddErrors(ModelState, errors);

			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var address = _addressModelsMapper.ToEntity(model);

			if (_addressService.Save(address) == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Address_Create",
					Message =
						$"Cannot create an address for the non-existing customer #{address.CustomerId}!",
					LinkText = "To customers list",
					ActionName = "Index",
					ControllerName = "Customers"
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		// GET: Customers/4/Addresses/Edit/5
		public ActionResult Edit(int customerId, int addressId)
		{
			var customer = _customerService.Get(customerId, false, false);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			var address = _addressService.Get(addressId);

			if (IsAddressNotFound(address) ||
				address.CustomerId != customer.CustomerId)
			{
				return View("NotFound");
			}

			var addressEditModel = new AddressEditModel(address)
			{
				Title = TitleHelper.GetTitleAddressEdit(customer),
				SubmitButtonText = "Save"
			};

			return View(addressEditModel);
		}

		// POST: Customers/4/Addresses/Edit/5
		[HttpPost]
		public ActionResult Edit(AddressEditModel model)
		{
			var errors = _addressModelsValidator.ValidateEditModel(model);

			ModelStateHelper.AddErrors(ModelState, errors);

			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var address = _addressModelsMapper.ToEntity(model);

			if (_addressService.Update(address) == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Address_Edit",
					Message = $"Cannot edit the address #{address.AddressId}: it doesn't exist!",
					LinkText = "To addresses list",
					ActionName = "Index",
					ControllerName = "Addresses",
					RouteValues = new { customerId = address.CustomerId }
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		// GET: Customers/4/Addresses/Delete/5
		public ActionResult Delete(int customerId, int addressId)
		{
			if (IsCustomerNotFound(customerId))
			{
				return View("NotFound");
			}

			var foundAndDeleted = _addressService.Delete(addressId);

			if (foundAndDeleted == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Address_Delete",
					Message = $"Cannot delete the address #{addressId}: it doesn't exist!",
					LinkText = "To addresses list",
					ActionName = "Index",
					ControllerName = "Addresses"
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		#endregion

		#region Private Methods

		private bool IsCustomerNotFound(int customerId) =>
			_customerService.Exists(customerId) == false;
		private bool IsAddressNotFound(Address address) => address is null;
		private bool IsCustomerNotFound(Customer customer) => customer is null;

		#endregion
	}
}

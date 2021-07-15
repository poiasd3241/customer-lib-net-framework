using System.Web.Mvc;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebMvc.Models;
using CustomerLib.WebMvc.Models.Addresses;
using CustomerLib.WebMvc.Models.Customers;

namespace CustomerLib.WebMvc.Controllers
{
	[Route("Customers/{action:alpha=Index}/{customerId:int?}")]
	public class CustomersController : Controller
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly ICustomerModelsMapper _customerModelsMapper;
		private readonly CustomerModelsValidator _customerModelsValidator = new();

		#endregion

		#region Constructors

		public CustomersController()
		{
			_customerService = new CustomerService();
		}

		public CustomersController(ICustomerService customerService,
			ICustomerModelsMapper customerModelsMapper)
		{
			_customerService = customerService;
			_customerModelsMapper = customerModelsMapper;
		}

		#endregion

		#region Actions

		// GET: Customers
		public ActionResult Index(int page = 1, int pageSize = 10)
		{
			var customers = _customerService.GetPage(page, pageSize, false, false, false, 0);

			var customersModel = new CustomersModel(customers)
			{
				Page = page,
				PageSize = pageSize
			};

			return View(customersModel);
		}

		// GET: Customers/Create
		public ActionResult Create()
		{
			var customerCreateModel = new CustomerCreateModel()
			{
				BasicDetails = new CustomerBasicDetailsModel(),
				AddressDetails = new AddressDetailsModel()
				{
					Address = new()
				}
			};

			return View(customerCreateModel);
		}

		// POST: Customers/Create
		[HttpPost]
		public ActionResult Create(CustomerCreateModel model)
		{
			var errors = _customerModelsValidator.ValidateCreateModel(model);

			ModelStateHelper.AddErrors(ModelState, errors);

			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var customer = _customerModelsMapper.ToEntity(model);

			try
			{
				_customerService.Save(customer);

				return RedirectToAction("Index");
			}
			catch (EmailTakenException)
			{
				ModelState.AddModelError($"{nameof(CustomerCreateModel.BasicDetails)}." +
					$"{nameof(CustomerBasicDetailsModel.Email)}",
					$"{customer.Email} is already taken");
				return View(model);
			}
		}

		// GET: Customers/Edit/5
		public ActionResult Edit(int customerId)
		{
			var customer = _customerService.Get(customerId, false, false);

			if (IsCustomerNotFound(customer))
			{
				return View("NotFound");
			}

			var customerEditModel = new CustomerEditModel(customer);

			return View(customerEditModel);
		}

		// POST: Customers/Edit/5
		[HttpPost]
		public ActionResult Edit(CustomerEditModel model)
		{
			var errors = _customerModelsValidator.ValidateEditModel(model);

			ModelStateHelper.AddErrors(ModelState, errors);

			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var customer = _customerModelsMapper.ToEntity(model);

			bool foundAndUpdated;

			try
			{
				foundAndUpdated = _customerService.Update(customer);
			}
			catch (EmailTakenException)
			{
				ModelState.AddModelError(nameof(CustomerBasicDetailsModel.Email),
					$"{customer.Email} is already taken");
				return View(model);
			}

			if (foundAndUpdated == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Customer_Edit",
					Message = $"Cannot edit the customer #{customer.CustomerId}: it doesn't exist!",
					LinkText = "To customers list",
					ActionName = "Index",
					ControllerName = "Customers"
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		// GET: Customers/Delete/5
		public ActionResult Delete(int customerId)
		{
			if (IsCustomerNotFound(customerId))
			{
				return View("NotFound");
			}

			var foundAndDeleted = _customerService.Delete(customerId);

			if (foundAndDeleted == false)
			{
				var failureModel = new FailureModel()
				{
					Title = "Customer_Delete",
					Message = $"Cannot delete the customer #{customerId}: it doesn't exist!",
					LinkText = "To customers list",
					ActionName = "Index",
					ControllerName = "Customers"
				};

				return View("Failure", failureModel);
			}

			return RedirectToAction("Index");
		}

		#endregion

		#region Private Methods

		private bool IsCustomerNotFound(int customerId) =>
			_customerService.Exists(customerId) == false;

		private bool IsCustomerNotFound(Customer customer) => customer is null;

		#endregion
	}
}

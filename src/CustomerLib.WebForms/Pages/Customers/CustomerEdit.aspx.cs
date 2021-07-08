using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Validators;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;
using CustomerLib.WebForms.Validation;

namespace CustomerLib.WebForms.Pages.Customers
{
	public partial class CustomerEdit : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly IAddressService _addressService;
		private readonly INoteService _noteService;

		private readonly CustomerValidator _customerValidator = new();

		private readonly EntityInputValidator _entityInputValidator = new();
		private readonly CustomerInputValidator _customerInputValidator = new();

		private Dictionary<string, Label> _validationErrorLabelsByCustomerPropertyName;

		#endregion

		#region Public Properties

		public Customer Customer { get; set; }

		#endregion

		#region Constructors

		public CustomerEdit()
		{
			_customerService = new CustomerService();
			_addressService = new AddressService();
			_noteService = new NoteService();
		}

		public CustomerEdit(ICustomerService customerService, IAddressService addressService,
			INoteService noteService)
		{
			_customerService = customerService;
			_addressService = addressService;
			_noteService = noteService;
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack == false)
			{
				if (int.TryParse(Request.QueryString["customerId"], out int customerId) == false ||
					Request.QueryString.Count != 1)
				{
					throw new HttpException(400, "Bad Request");
				}

				LoadCustomer(customerId);
				InitUI();
			}

			PopulateValidationErrorLabelsByCustomerPropertyName();
		}

		public void LoadCustomer(int customerId)
		{
			Customer = _customerService.Get(customerId, true, true);

			if (Customer is null)
			{
				this.AlertRedirect("alertCustomerDoesNotExist",
					$"The customer #{customerId} doesn't exist!",
					"/Customers");
			}
		}

		public void InitUI()
		{
			inputFirstName.Text = Customer.FirstName;
			inputLastName.Text = Customer.LastName;
			inputPhoneNumber.Text = Customer.PhoneNumber;
			inputEmail.Text = Customer.Email;
			inputTotalPurchasesAmount.Text = Customer.TotalPurchasesAmount?.ToString();

			if (Customer.Addresses is null || Customer.Addresses.Count == 0)
			{
				tableHeaderAddresses.Visible = false;
				labelAddressesAbscent.Visible = true;
			}
			else
			{
				repeaterAddresses.DataSource = Customer.Addresses;
				repeaterAddresses.DataBind();
			}

			if (Customer.Notes is null || Customer.Notes.Count == 0)
			{
				tableHeaderNotes.Visible = false;
				labelNotesAbscent.Visible = true;
			}
			else
			{
				repeaterNotes.DataSource = Customer.Notes;
				repeaterNotes.DataBind();
			}

			linkButtonAddAddress.Attributes["href"] =
				$"/Addresses/Create?customerId={Customer.CustomerId}";
			linkButtonAddNote.Attributes["href"] =
				$"/Notes/Create?customerId={Customer.CustomerId}";

			// Decrease ViewState
			Customer.Addresses = null;
			Customer.Notes = null;
		}

		protected override object SaveViewState()
		{
			ViewState["Customer"] = Customer;

			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			Customer = (Customer)ViewState["Customer"];
		}

		public void PopulateValidationErrorLabelsByCustomerPropertyName()
		{
			if (_validationErrorLabelsByCustomerPropertyName is null)
			{
				_validationErrorLabelsByCustomerPropertyName = new();
			}

			_validationErrorLabelsByCustomerPropertyName.Add("FirstName",
				validationErrorFirstName);
			_validationErrorLabelsByCustomerPropertyName.Add("LastName", validationErrorLastName);
			_validationErrorLabelsByCustomerPropertyName.Add("PhoneNumber",
				validationErrorPhoneNumber);
			_validationErrorLabelsByCustomerPropertyName.Add("Email", validationErrorEmail);
			_validationErrorLabelsByCustomerPropertyName.Add("TotalPurchasesAmount",
				validationErrorTotalPurchasesAmount);
		}

		protected void OnSaveCommand(object sender, CommandEventArgs e)
		{
			if (ValidateAll() == false)
			{
				this.Alert("alertInputErrors", "Please correct the input errors.");
				return;
			}

			SaveCustomer();
		}

		public void SaveCustomer()
		{
			bool foundAndUpdated;

			try
			{
				foundAndUpdated = _customerService.Update(Customer);
			}
			catch (EmailTakenException)
			{
				validationErrorEmail.Visible = true;
				validationErrorEmail.Text = $"{Customer.Email} is already taken.";

				this.Alert("alertEmailTaken", "Email is already taken!");
				return;
			}

			var alertMessage = foundAndUpdated
				? $"Customer #{Customer.CustomerId} updated successfully!"
				: $"Cannot update the customer #{Customer.CustomerId}: it doesn't exist!";

			this.AlertRedirect("alertUpdateResult", alertMessage, "/Customers");
		}

		public void OnDeleteAddressCommand(object sender, CommandEventArgs e)
		{
			var addressId = int.Parse(e.CommandArgument.ToString());

			var alertMessage = DeleteAddress(addressId)
				? $"Address #{addressId} deleted successfully!"
				: $"Cannot delete the address #{addressId}: it doesn't exist!";

			// Refresh the page.
			this.AlertRedirect("alertDeleteAddressResult", alertMessage, $"{Request.Url}");
		}

		public bool DeleteAddress(int addressId) => _addressService.Delete(addressId);

		public void OnDeleteNoteCommand(object sender, CommandEventArgs e)
		{
			var noteId = int.Parse(e.CommandArgument.ToString());

			var alertMessage = DeleteNote(noteId)
				? $"Note #{noteId} deleted successfully!"
				: $"Cannot delete the note #{noteId}: it doesn't exist!";

			// Refresh the page.
			this.AlertRedirect("alertDeleteNoteResult", alertMessage, $"{Request.Url}");
		}

		public bool DeleteNote(int noteId) => _noteService.Delete(noteId);

		#endregion

		#region Validation

		/// <summary>
		/// Validates the customer according to <see cref="CustomerValidator"/>.
		/// Addresses and notes are excluded from validation.
		/// </summary>
		/// <returns>True if the customer is valid, otherwise false.</returns>
		public bool ValidateAll()
		{
			var allValid = true;

			foreach (var propertyName in _validationErrorLabelsByCustomerPropertyName.Keys)
			{
				allValid = _entityInputValidator.ValidatePropertyAndAdjustErrorLabel(
					Customer, propertyName,
					_validationErrorLabelsByCustomerPropertyName[propertyName]) &&
					allValid;
			}

			allValid = ValidateTotalPurchasesAmountAndAdjustErrorLabel(
				inputTotalPurchasesAmount.Text) && allValid;

			if (Customer.Addresses is not null || Customer.Notes is not null)
			{
				throw new Exception("Unexpected Customer value: Addresses and Notes must be null.");
			}

			var customerResult = _customerValidator.ValidateWithoutAddressesAndNotes(Customer);

			if (allValid && customerResult.IsValid == false)
			{
				throw new Exception("Mismatch in Customer validation.");
			}

			return allValid;
		}

		protected void OnInputTextChanged(object sender, EventArgs e)
		{
			string propertyName;
			var textBox = (TextBox)sender;
			var text = textBox.Text == "" ? null : textBox.Text;

			switch (textBox.ID)
			{
				case nameof(inputFirstName):
				{
					propertyName = nameof(Customer.FirstName);
					Customer.FirstName = text;
					break;
				}
				case nameof(inputLastName):
				{
					propertyName = nameof(Customer.LastName);
					Customer.LastName = text;
					break;
				}
				case nameof(inputPhoneNumber):
				{
					propertyName = nameof(Customer.PhoneNumber);
					Customer.PhoneNumber = text;
					break;
				}
				case nameof(inputEmail):
				{
					propertyName = nameof(Customer.Email);
					Customer.Email = text;
					break;
				}
				case nameof(inputTotalPurchasesAmount):
				{
					ValidateTotalPurchasesAmountAndAdjustErrorLabel(text);
					return;
				}
				default:
					throw new ArgumentException("Unknown sender", nameof(sender));
			};

			_entityInputValidator.ValidatePropertyAndAdjustErrorLabel(Customer, propertyName,
				_validationErrorLabelsByCustomerPropertyName[propertyName]);
		}

		public bool ValidateTotalPurchasesAmount(string input)
		{
			var isValid = _customerInputValidator.ValidateTotalPurchasesAmount(
				input, out decimal? validValue);

			Customer.TotalPurchasesAmount = validValue;

			return isValid;
		}

		public bool ValidateTotalPurchasesAmountAndAdjustErrorLabel(string input)
		{
			var isValid = ValidateTotalPurchasesAmount(input);
			_customerInputValidator.AdjustTotalPurchasesAmountErrorLabel(
				isValid, validationErrorTotalPurchasesAmount);

			return isValid;
		}

		#endregion
	}
}

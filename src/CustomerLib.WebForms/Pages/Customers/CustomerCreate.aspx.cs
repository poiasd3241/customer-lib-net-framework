using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Exceptions;
using CustomerLib.Business.Validators;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;
using CustomerLib.WebForms.Pages.PageHelpers.Addresses;
using CustomerLib.WebForms.Validation;
using FluentValidation.Results;

namespace CustomerLib.WebForms.Pages.Customers
{
	public partial class CustomerCreate : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;

		private readonly CustomerValidator _customerValidator = new();

		private readonly EntityInputValidator _entityInputValidator = new();
		private readonly CustomerInputValidator _customerInputValidator = new();

		private Dictionary<string, Label> _validationErrorLabelsByCustomerPropertyName;
		private Dictionary<string, Label> _validationErrorLabelsByAddressPropertyName;

		#endregion

		#region Public Properties

		public Customer Customer { get; set; }

		#endregion

		#region Constructors

		public CustomerCreate()
		{
			_customerService = new CustomerService();
		}

		public CustomerCreate(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			PopulateValidationErrorLabelsByCustomerPropertyName();
			PopulateValidationErrorLabelsByAddressPropertyName();

			PopulateAddressCountryDropDown();
			PopulateAddressTypeDropDown();

			if (IsPostBack)
			{
				return;
			}

			// Initialize customer with one Address and Note empty objects.
			Customer = new()
			{
				Notes = new() { new() },
				Addresses = new() { new() }
			};
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
		public void PopulateValidationErrorLabelsByAddressPropertyName()
		{
			if (_validationErrorLabelsByAddressPropertyName is null)
			{
				_validationErrorLabelsByAddressPropertyName = new();
			}

			_validationErrorLabelsByAddressPropertyName.Add("AddressLine",
				validationErrorAddressLine);
			_validationErrorLabelsByAddressPropertyName.Add("AddressLine2",
				validationErrorAddressLine2);
			_validationErrorLabelsByAddressPropertyName.Add("City", validationErrorCity);
			_validationErrorLabelsByAddressPropertyName.Add("PostalCode",
				validationErrorPostalCode);
			_validationErrorLabelsByAddressPropertyName.Add("State", validationErrorState);
		}

		public void PopulateAddressTypeDropDown()
		{
			dropDownInputType.DataSource = AddressUiHelper.GetAddressTypeDropDownItems();
			dropDownInputType.DataTextField = "Text";
			dropDownInputType.DataValueField = "Value";
			dropDownInputType.DataBind();
		}

		public void PopulateAddressCountryDropDown()
		{
			dropDownInputCountry.DataSource = AddressUiHelper.GetAddressCountryDropDownItems();
			dropDownInputCountry.DataBind();
		}

		protected void OnCreateCommand(object sender, CommandEventArgs e)
		{
			if (ValidateAll() == false)
			{
				this.Alert("alertInputErrors", "Please correct the input errors.");
				return;
			}

			CreateCustomer();
		}

		public void CreateCustomer()
		{
			try
			{
				_customerService.Save(Customer);
			}
			catch (EmailTakenException)
			{
				validationErrorEmail.Visible = true;
				validationErrorEmail.Text = $"{Customer.Email} is already taken.";

				this.Alert("alertEmailTaken", "Email is already taken!");
				return;
			}

			// Redirect to Customers.
			this.AlertRedirect("alertCreateSuccess", "Customer created successfully!",
				$"/Customers?page={int.MaxValue}");
		}

		public void ReadAddressDropDowns()
		{
			var addressType = (AddressType)int.Parse(dropDownInputType.SelectedValue);
			var country = dropDownInputCountry.SelectedValue;

			Customer.Addresses[0].Type = addressType;
			Customer.Addresses[0].Country = country;
		}

		#endregion

		#region Validation

		/// <summary>
		/// Validates the customer according to <see cref="CustomerValidator"/>.
		/// </summary>
		/// <returns>True if the customer is valid, otherwise false.</returns>
		public bool ValidateAll()
		{
			ReadAddressDropDowns();

			var allValid = true;

			foreach (var propertyName in _validationErrorLabelsByCustomerPropertyName.Keys)
			{
				allValid = _entityInputValidator.ValidatePropertyAndAdjustErrorLabel(
					Customer, propertyName,
					_validationErrorLabelsByCustomerPropertyName[propertyName]) && allValid;
			}

			foreach (var propertyName in _validationErrorLabelsByAddressPropertyName.Keys)
			{
				allValid = _entityInputValidator.ValidatePropertyAndAdjustErrorLabel(
					Customer.Addresses[0],
					propertyName, _validationErrorLabelsByAddressPropertyName[propertyName]) &&
					allValid;
			}

			allValid = ValidateTotalPurchasesAmountAndAdjustErrorLabel(
				inputTotalPurchasesAmount.Text) && allValid;
			allValid = ValidateNoteAndAdjustErrorLabel(inputNoteContent.Text) && allValid;

			if (Customer.Addresses.Count != 1 || Customer.Notes.Count != 1)
			{
				throw new Exception(
					"Unexpected Customer value: Addresses and Notes must each contain a single element."
					);
			}

			var customerResult = _customerValidator.Validate(Customer);

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

		protected void OnAddressInputTextChanged(object sender, EventArgs e)
		{
			string propertyName;
			var textBox = (TextBox)sender;
			var text = textBox.Text == "" ? null : textBox.Text;

			switch (textBox.ID)
			{
				case nameof(inputAddressLine):
				{
					propertyName = nameof(Address.AddressLine);
					Customer.Addresses[0].AddressLine = text;
					break;
				}
				case nameof(inputAddressLine2):
				{
					propertyName = nameof(Address.AddressLine2);
					Customer.Addresses[0].AddressLine2 = text;
					break;
				}
				case nameof(inputCity):
				{
					propertyName = nameof(Address.City);
					Customer.Addresses[0].City = text;
					break;
				}
				case nameof(inputPostalCode):
				{
					propertyName = nameof(Address.PostalCode);
					Customer.Addresses[0].PostalCode = text;
					break;
				}
				case nameof(inputState):
				{
					propertyName = nameof(Address.State);
					Customer.Addresses[0].State = text;
					break;
				}
				default:
					throw new ArgumentException("Unknown sender", nameof(sender));
			};

			_entityInputValidator.ValidatePropertyAndAdjustErrorLabel(Customer.Addresses[0],
				propertyName, _validationErrorLabelsByAddressPropertyName[propertyName]);
		}

		protected void OnNoteContentChanged(object sender, EventArgs e) =>
			ValidateNoteAndAdjustErrorLabel(inputNoteContent.Text);

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

		public ValidationResult ValidateNote(string content)
		{
			if (content == "")
			{
				content = null;
			}

			Customer.Notes[0].Content = content;

			return _entityInputValidator.ValidatePropertyResult(Customer.Notes[0], "Content");
		}

		public bool ValidateNoteAndAdjustErrorLabel(string content)
		{
			var result = ValidateNote(content);

			_entityInputValidator.AdjustErrorLabel(result, validationErrorNoteContent);

			return result.IsValid;
		}

		#endregion
	}
}
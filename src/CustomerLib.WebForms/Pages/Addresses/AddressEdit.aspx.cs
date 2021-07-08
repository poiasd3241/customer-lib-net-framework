using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Validators;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;
using CustomerLib.WebForms.Pages.PageHelpers.Addresses;
using CustomerLib.WebForms.Validation;

namespace CustomerLib.WebForms.Pages.Addresses
{
	public partial class AddressEdit : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly IAddressService _addressService;

		private readonly AddressValidator _addressValidator = new();

		private readonly EntityInputValidator _entityInputValidator = new();

		private Dictionary<string, Label> _validationErrorLabelsByAddressPropertyName;

		#endregion

		#region Public Properties

		public Address Address { get; set; }
		public int CustomerId { get; set; }
		public bool IsCreate { get; set; }

		#endregion

		#region Constructors

		public AddressEdit()
		{
			_customerService = new CustomerService();
			_addressService = new AddressService();
		}

		public AddressEdit(ICustomerService customerService, IAddressService addressService)
		{
			_customerService = customerService;
			_addressService = addressService;
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack == false)
			{
				// Create mode.
				var isCustomerIdSpecified = int.TryParse(Request.QueryString["customerId"],
					out int customerId);

				// Edit mode
				var isAddressIdSpecified = int.TryParse(Request.QueryString["addressId"],
					out int addressId);

				if (Request.QueryString.Count != 1 ||
					isAddressIdSpecified == false && isCustomerIdSpecified == false)
				{
					throw new HttpException(400, "Bad Request");
				}

				var type = (string)RouteData.Values["mode"];

				if (type != "Create" && type != "Edit" ||
					type == "Create" && isAddressIdSpecified ||
					type == "Edit" && isCustomerIdSpecified)
				{
					throw new HttpException(400, "Bad Request");
				}

				PopulateAddressCountryDropDown();
				PopulateAddressTypeDropDown();

				if (type == "Create")
				{
					if (CheckCustomerExists(customerId) == false)
					{
						return;
					}

					IsCreate = true;
					CustomerId = customerId;
					Address = new() { CustomerId = CustomerId };
				}
				else
				{
					if (LoadAddress(addressId) == false)
					{
						return;
					}

					IsCreate = false;
					PopulateUiWithAddressData();
				}

				InitCustomerDependentUI();
			}

			PopulateValidationErrorLabelsByAddressPropertyName();
		}

		public bool CheckCustomerExists(int customerId)
		{
			if (_customerService.Exists(customerId))
			{
				return true;
			}

			this.AlertRedirect("alertCustomerDoesNotExist",
				$"The customer #{customerId} doesn't exist!",
				"/Customers");
			return false;
		}

		public bool LoadAddress(int addressId)
		{
			Address = _addressService.Get(addressId);

			if (Address is not null)
			{
				return true;
			}

			this.AlertRedirect("alertAddressDoesNotExist",
				$"The address #{addressId} doesn't exist!",
				"/Customers");
			return false;
		}

		public void PopulateUiWithAddressData()
		{
			inputAddressLine.Text = Address.AddressLine;
			inputAddressLine2.Text = Address.AddressLine2;
			dropDownInputType.SelectedValue = ((int)Address.Type).ToString();
			inputCity.Text = Address.City;
			inputPostalCode.Text = Address.PostalCode;
			inputState.Text = Address.State;
			dropDownInputCountry.SelectedValue = Address.Country;
		}

		public Customer LoadCustomer(int customerId) =>
			_customerService.Get(customerId, false, false);

		public void InitCustomerDependentUI()
		{
			var customer = LoadCustomer(Address.CustomerId);

			labelTitle.Text = IsCreate
				? TitleHelper.GetTitleAddressCreate(customer)
				: TitleHelper.GetTitleAddressEdit(customer);

			btnSave.Text = IsCreate ? "Create" : "Save";
		}

		protected override object SaveViewState()
		{
			ViewState["Address"] = Address;
			ViewState["IsCreate"] = IsCreate;

			if (IsCreate)
			{
				ViewState["CustomerId"] = CustomerId;
			}

			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			Address = (Address)ViewState["Address"];
			IsCreate = (bool)ViewState["IsCreate"];

			if (IsCreate)
			{
				CustomerId = (int)ViewState["CustomerId"];
			}
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

		protected void OnSaveCommand(object sender, CommandEventArgs e)
		{
			if (ValidateAll() == false)
			{
				this.Alert("alertInputErrors", "Please correct the input errors.");
				return;
			}

			if (IsCreate)
			{
				CreateAddress();
			}
			else
			{
				SaveExistingAddress();
			}
		}

		public void CreateAddress()
		{
			_addressService.Save(Address);

			// Redirect to the customer's addresses.
			this.AlertRedirect("alertCreateSuccess",
				"Address created successfully!",
				$"/Addresses?customerId={CustomerId}");

		}

		public void SaveExistingAddress()
		{
			if (_addressService.Update(Address))
			{
				this.Alert("alertUpdateResult",
					$"Address #{Address.AddressId} updated successfully!");
				return;
			}

			// Redirect to Customers.
			this.AlertRedirect("alertUpdateResult",
				$"Cannot update the address #{Address.AddressId}: it doesn't exist!",
				"/Customers");
		}

		public void ReadAddressDropDowns()
		{
			var addressType = (AddressType)int.Parse(dropDownInputType.SelectedValue);
			var country = dropDownInputCountry.SelectedValue;

			Address.Type = addressType;
			Address.Country = country;
		}

		#endregion

		#region Validation

		/// <summary>
		/// Validates the address according to <see cref="AddressValidator"/>.
		/// </summary>
		/// <returns>True if the address is valid, otherwise false.</returns>
		public bool ValidateAll()
		{
			ReadAddressDropDowns();

			var allValid = true;

			foreach (var propertyName in _validationErrorLabelsByAddressPropertyName.Keys)
			{
				allValid = _entityInputValidator.ValidatePropertyAndAdjustErrorLabel(Address,
					propertyName, _validationErrorLabelsByAddressPropertyName[propertyName]) &&
					allValid;
			}

			var addressResult = _addressValidator.Validate(Address);

			if (allValid && addressResult.IsValid == false)
			{
				throw new Exception("Mismatch in Address validation.");
			}

			return allValid;
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
					Address.AddressLine = text;
					break;
				}
				case nameof(inputAddressLine2):
				{
					propertyName = nameof(Address.AddressLine2);
					Address.AddressLine2 = text;
					break;
				}
				case nameof(inputCity):
				{
					propertyName = nameof(Address.City);
					Address.City = text;
					break;
				}
				case nameof(inputPostalCode):
				{
					propertyName = nameof(Address.PostalCode);
					Address.PostalCode = text;
					break;
				}
				case nameof(inputState):
				{
					propertyName = nameof(Address.State);
					Address.State = text;
					break;
				}
				default:
					throw new ArgumentException("Unknown sender", nameof(sender));
			};

			_entityInputValidator.ValidatePropertyAndAdjustErrorLabel(Address, propertyName,
				_validationErrorLabelsByAddressPropertyName[propertyName]);
		}

		#endregion
	}
}
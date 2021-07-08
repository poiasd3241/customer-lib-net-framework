using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.ServiceLayer.Services;
using CustomerLib.ServiceLayer.Services.Implementations;
using CustomerLib.WebForms.Pages.PageHelpers;

namespace CustomerLib.WebForms.Pages.Addresses
{
	public partial class AddressList : Page
	{
		#region Private Members

		private readonly ICustomerService _customerService;
		private readonly IAddressService _addressService;

		#endregion

		#region Constructors

		public AddressList(ICustomerService customerService, IAddressService addressService)
		{
			_customerService = customerService;
			_addressService = addressService;
		}

		public AddressList()
		{
			_customerService = new CustomerService();
			_addressService = new AddressService();
		}

		#endregion

		#region Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			if (int.TryParse(Request.QueryString["customerId"], out int customerId) == false ||
				Request.QueryString.Count != 1)
			{
				throw new HttpException(400, "Bad Request");
			}

			InitUI(customerId);
		}

		public Customer LoadCustomerWithAddresses(int customerId) =>
			_customerService.Get(customerId, true, false);

		public void InitUI(int customerId)
		{
			var customer = LoadCustomerWithAddresses(customerId);

			if (customer is null)
			{
				labelCustomerDoesNotExist.Visible = true;
				tableHeaderAddresses.Visible = false;
				labelTitle.Text = "Addresses for the customer";
				return;
			}

			labelTitle.Text = TitleHelper.GetTitleAddresses(customer);

			var addresses = customer.Addresses;

			if (addresses is null || addresses.Count == 0)
			{
				tableHeaderAddresses.Visible = false;
				labelAddressesAbscent.Visible = true;
			}
			else
			{
				repeaterAddresses.DataSource = addresses;
				repeaterAddresses.DataBind();
			}

			linkButtonAddAnAddress.Attributes["href"] =
				$"Addresses/Create?customerId={customer.CustomerId}";
		}

		protected void OnDeleteAddressCommand(object sender, CommandEventArgs e)
		{
			var addressId = int.Parse(e.CommandArgument.ToString());

			var alertMessage = DeleteAddress(addressId)
				? $"Address #{addressId} deleted successfully!"
				: $"Cannot delete the address #{addressId}: it doesn't exist!";

			//this.RegisterClientScript("alertDeleteResult",
			//	alertMessage + $"window.location.href = '{Request.Url}';");

			// Refresh the page.
			this.AlertRedirect("alertDeleteResult", alertMessage, $"{Request.Url}");
		}

		public bool DeleteAddress(int addressId) => _addressService.Delete(addressId);

		#endregion
	}
}
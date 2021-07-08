using System.Web.UI.WebControls;
using CustomerLib.Business.Enums;

namespace CustomerLib.WebForms.Pages.PageHelpers.Addresses
{
	public class AddressUiHelper
	{
		public static ListItemCollection GetAddressTypeDropDownItems() => new()
		{
			new ListItem(
				AddressType.Shipping.ToString(),
				((int)AddressType.Shipping).ToString()),
			new ListItem(
				AddressType.Billing.ToString(),
				((int)AddressType.Billing).ToString()),
		};

		public static ListItemCollection GetAddressCountryDropDownItems() => new()
		{
			new ListItem("United States"),
			new ListItem("Canada")
		};
	}
}

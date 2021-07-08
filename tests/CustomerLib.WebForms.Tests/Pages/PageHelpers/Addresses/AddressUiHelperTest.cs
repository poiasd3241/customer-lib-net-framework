using CustomerLib.WebForms.Pages.PageHelpers.Addresses;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.PageHelpers.Addresses
{
	public class AddressUiHelperTest
	{
		[Fact]
		public void ShouldGetAddressTypeDropDownItems()
		{
			// Given
			var itemShippingText = "Shipping";
			var itemShippingValue = "1";
			var itemBillingText = "Billing";
			var itemBillingValue = "2";

			// When
			var actualItems = AddressUiHelper.GetAddressTypeDropDownItems();

			// Then
			Assert.Equal(itemShippingText, actualItems[0].Text);
			Assert.Equal(itemShippingValue, actualItems[0].Value);

			Assert.Equal(itemBillingText, actualItems[1].Text);
			Assert.Equal(itemBillingValue, actualItems[1].Value);
		}

		[Fact]
		public void ShouldGetAddressCountryDropDownItems()
		{
			// Given
			var itemUS = "United States";
			var itemCA = "Canada";

			// When
			var actualItems = AddressUiHelper.GetAddressCountryDropDownItems();

			// Then
			Assert.Equal(itemUS, actualItems[0].Text);
			Assert.Equal(itemCA, actualItems[1].Text);
		}
	}
}

using CustomerLib.Business.Entities;
using CustomerLib.TestHelpers;
using CustomerLib.WebMvc.Models.Addresses;
using Xunit;

namespace CustomerLib.WebMvc.Tests.Models.Addresses
{
	public class AddressEditModelTest
	{
		[Fact]
		public void ShouldCreateAddressEditModelDefaultConstructor()
		{
			var model = new AddressEditModel();

			Assert.Null(model.Title);
			Assert.Null(model.SubmitButtonText);
			Assert.Null(model.AddressDetails);
		}

		[Fact]
		public void ShouldCreateAddressEditModelFromAddress()
		{
			// Given
			var address = MockAddress();

			var model = new AddressEditModel(address);

			Assert.Null(model.Title);
			Assert.Null(model.SubmitButtonText);
			Assert.Equal(address, model.AddressDetails.Address);
		}

		[Fact]
		public void ShouldSetProperties()
		{
			// Given
			var title = "edit";
			var submitButtonText = "submit";
			var address = MockAddress();

			var addressDetailsModel = new AddressDetailsModel(address);

			var model = new AddressEditModel();

			Assert.NotEqual(title, model.Title);
			Assert.NotEqual(submitButtonText, model.SubmitButtonText);
			Assert.Null(model.AddressDetails);

			// When
			model.AddressDetails = addressDetailsModel;
			model.Title = title;
			model.SubmitButtonText = submitButtonText;

			// Then
			Assert.Equal(title, model.Title);
			Assert.Equal(submitButtonText, model.SubmitButtonText);
			Assert.Equal(addressDetailsModel, model.AddressDetails);
		}

		private Address MockAddress() => new()
		{
			AddressId = 5,
			CustomerId = 8,
			AddressLine = "1",
			AddressLine2 = "2",
			Type = Business.Enums.AddressType.Billing,
			City = "city x",
			PostalCode = "111",
			State = "state x",
			Country = "Canada"
		};
	}
}

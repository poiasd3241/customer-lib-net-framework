using CustomerLib.Business.Entities;
using CustomerLib.WebForms.Pages.PageHelpers;
using Xunit;

namespace CustomerLib.WebForms.Tests.Pages.PageHelpers
{
	public class TitleHelperTest
	{
		#region Get customer name and email text

		[Fact]
		public void ShouldGetCustomerLastNameWhenFirstNameAndEmailNull()
		{
			// Given
			var customer = new Customer() { LastName = "One" };

			// When
			var text = TitleHelper.GetCustomerNameAndEmailText(customer);

			// Then
			Assert.Equal(customer.LastName, text);
		}

		[Fact]
		public void ShouldGetCustomerLastNameWithFirstNameAndEmail()
		{
			// Given
			var customer = new Customer()
			{
				FirstName = "Zero",
				LastName = "One",
				Email = "a@a.com"
			};

			var expectedText = "Zero One (a@a.com)";

			// When
			var text = TitleHelper.GetCustomerNameAndEmailText(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		#endregion

		#region Note titles

		[Fact]
		public void ShouldGetTitleNoteCreate()
		{
			// Given
			var customer = new Customer() { LastName = "One" };
			var expectedText = "New note for the customer One";

			// When
			var text = TitleHelper.GetTitleNoteCreate(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		[Fact]
		public void ShouldGetTitleNoteEdit()
		{
			// Given
			var customer = new Customer() { LastName = "One" };
			var expectedText = "Edit the note for the customer One";

			// When
			var text = TitleHelper.GetTitleNoteEdit(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		[Fact]
		public void ShouldGetTitleNotes()
		{
			// Given
			var customer = new Customer() { LastName = "One" };
			var expectedText = "Notes for the customer One";

			// When
			var text = TitleHelper.GetTitleNotes(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		#endregion

		#region Address titles

		[Fact]
		public void ShouldGetTitleAddressCreate()
		{
			// Given
			var customer = new Customer() { LastName = "One" };
			var expectedText = "New address for the customer One";

			// When
			var text = TitleHelper.GetTitleAddressCreate(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		[Fact]
		public void ShouldGetTitleAddressEdit()
		{
			// Given
			var customer = new Customer() { LastName = "One" };
			var expectedText = "Edit the address for the customer One";

			// When
			var text = TitleHelper.GetTitleAddressEdit(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		[Fact]
		public void ShouldGetTitleAddresses()
		{
			// Given
			var customer = new Customer() { LastName = "One" };
			var expectedText = "Addresses for the customer One";

			// When
			var text = TitleHelper.GetTitleAddresses(customer);

			// Then
			Assert.Equal(expectedText, text);
		}

		#endregion
	}
}

using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;
using CustomerLib.WebMvc.Models.Addresses;

namespace CustomerLib.WebMvc.Models.Customers
{
	public class CustomerModelsValidator
	{
		private readonly CustomerValidator _customerValidator = new();
		private readonly AddressModelsValidator _addressModelsValidator = new();
		private readonly NoteValidator _noteValidator = new();

		public Dictionary<string, string> ValidateBasicDetailsModel(
			CustomerBasicDetailsModel model)
		{
			var result = new Dictionary<string, string>();

			if (ValidateTotalPurchasesAmount(model.TotalPurchasesAmount,
				out string totalPurchasesAmountError) == false)
			{
				result.Add(nameof(CustomerBasicDetailsModel.TotalPurchasesAmount),
					totalPurchasesAmountError);
			}

			var customerErrors = _customerValidator.ValidateWithoutAddressesAndNotes(new Customer()
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				PhoneNumber = model.PhoneNumber,
				Email = model.Email,
			}).Errors;

			foreach (var error in customerErrors)
			{
				result.Add(error.PropertyName, error.ErrorMessage);
			}

			return result;
		}

		public Dictionary<string, string> ValidateEditModel(CustomerEditModel model) =>
			ValidateBasicDetailsModel(model);

		public Dictionary<string, string> ValidateCreateModel(CustomerCreateModel model)
		{
			var result = new Dictionary<string, string>();

			var basicDetailsErrors = ValidateBasicDetailsModel(model.BasicDetails);
			var addressErrors = _addressModelsValidator.ValidateDetailsModel(model.AddressDetails);

			var noteErrors = _noteValidator.Validate(model.Note).Errors;

			foreach (var error in basicDetailsErrors)
			{
				result.Add($"{nameof(CustomerCreateModel.BasicDetails)}.{error.Key}", error.Value);
			}

			foreach (var error in addressErrors)
			{
				result.Add($"{nameof(CustomerCreateModel.AddressDetails)}.{error.Key}",
					error.Value);
			}

			foreach (var error in noteErrors)
			{
				result.Add($"{nameof(CustomerCreateModel.Note)}.{error.PropertyName}",
					error.ErrorMessage);
			}

			return result;
		}

		private bool ValidateTotalPurchasesAmount(string input, out string errorMessage)
		{
			if (decimal.TryParse(input, out _) || string.IsNullOrEmpty(input))
			{
				errorMessage = null;
				return true;
			}

			errorMessage =
				"Total purchases amount must be a decimal number." +
				"\n" +
				"*optional - clear the field if you don't have any value to enter.";
			return false;
		}
	}
}

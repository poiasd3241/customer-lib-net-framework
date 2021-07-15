using System;
using System.Collections.Generic;
using System.Linq;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;

namespace CustomerLib.WebMvc.Models.Addresses
{
	public class AddressModelsValidator
	{
		private readonly AddressValidator _addressValidator = new();

		public Dictionary<string, string> ValidateDetailsModel(AddressDetailsModel model)
		{
			var result = new Dictionary<string, string>();

			var errors = _addressValidator.Validate(model.Address).Errors;

			if (errors.Any(error =>
				error.PropertyName == nameof(Address.Type) ||
				error.PropertyName == nameof(Address.Country)))
			{
				throw new Exception("Unexpected model data.");
			}

			foreach (var error in errors)
			{
				result.Add($"{nameof(AddressDetailsModel.Address)}.{error.PropertyName}",
									error.ErrorMessage);
			}

			return result;
		}

		public Dictionary<string, string> ValidateEditModel(AddressEditModel model)
		{
			var result = new Dictionary<string, string>();

			var errors = ValidateDetailsModel(model.AddressDetails);

			foreach (var error in errors)
			{
				result.Add($"{nameof(AddressEditModel.AddressDetails)}.{error.Key}", error.Value);
			}

			return result;
		}
	}
}

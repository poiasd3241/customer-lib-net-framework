using System;
using System.Web.UI.WebControls;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace CustomerLib.WebForms.Validation
{
	public class EntityInputValidator
	{
		private readonly CustomerValidator _customerValidator = new();
		private readonly AddressValidator _addressValidator = new();
		private readonly NoteValidator _noteValidator = new();

		public ValidationResult ValidatePropertyResult(
			Entity propertyContainer, string propertyName)
		{
			ValidationResult result;

			switch (propertyContainer)
			{
				case Customer customer:
				{
					result = _customerValidator.Validate(customer,
						options => { options.IncludeProperties(propertyName); });
					break;
				}
				case Address address:
				{
					result = _addressValidator.Validate(address,
						options => { options.IncludeProperties(propertyName); });
					break;
				}
				case Note note:
				{
					result = _noteValidator.Validate(note,
						options => { options.IncludeProperties(propertyName); });
					break;
				}
				default:
					throw new ArgumentException("Unknown property container type",
						nameof(propertyContainer));
			}

			return result;
		}

		public bool ValidateProperty(Entity propertyContainer, string propertyName) =>
			ValidatePropertyResult(propertyContainer, propertyName).IsValid;

		public void AdjustErrorLabel(ValidationResult result, Label validationErrorLabel)
		{
			if (result.IsValid)
			{
				validationErrorLabel.Visible = false;
				return;
			}

			validationErrorLabel.Text = result.ToString();
			validationErrorLabel.Visible = true;
		}

		public bool ValidatePropertyAndAdjustErrorLabel(
			Entity propertyContainer, string propertyName, Label validationErrorLabel)
		{
			var result = ValidatePropertyResult(propertyContainer, propertyName);
			AdjustErrorLabel(result, validationErrorLabel);

			return result.IsValid;
		}
	}
}

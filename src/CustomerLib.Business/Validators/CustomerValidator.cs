using CustomerLib.Business.Entities;
using CustomerLib.Business.Localization;
using FluentValidation;
using FluentValidation.Results;

namespace CustomerLib.Business.Validators
{
	/// <summary>
	/// The fluent validator of <see cref="Customer"/> objects.
	/// </summary>
	public class CustomerValidator : AbstractValidator<Customer>
	{
		private static readonly int _name_max_length = 50;
		private static readonly string _phoneNumber_format = "E.164";

		public CustomerValidator()
		{
			RuleSet("Own", () =>
			{
				// FirstName - Optional
				RuleFor(customer => customer.FirstName).Cascade(CascadeMode.Stop)
					.NotEmptyNorWhitespace().WithMessage(
					ValidationRules.PERSON_FIRST_NAME_EMPTY_OR_WHITESPACE)
					.MaximumLength(_name_max_length).WithMessage(
					string.Format(ValidationRules.PERSON_FIRST_NAME_MAX_LENGTH, _name_max_length))
				.When(customer => customer.FirstName is not null);

				// LastName
				RuleFor(customer => customer.LastName).Cascade(CascadeMode.Stop)
					.NotNull().WithMessage(ValidationRules.PERSON_LAST_NAME_REQUIRED)
					.NotEmptyNorWhitespace().WithMessage(
						ValidationRules.PERSON_LAST_NAME_EMPTY_OR_WHITESPACE)
					.MaximumLength(_name_max_length).WithMessage(
						string.Format(ValidationRules.PERSON_LAST_NAME_MAX_LENGTH, _name_max_length));

				// PhoneNumber - Optional
				RuleFor(customer => customer.PhoneNumber).Cascade(CascadeMode.Stop)
					.NotEmptyNorWhitespace().WithMessage(
						ValidationRules.CUSTOMER_PHONE_NUMBER_EMPTY_OR_WHITESPACE)
					.PhoneNumberFormatE164().WithMessage(
						string.Format(ValidationRules.CUSTOMER_PHONE_NUMBER_FORMAT,
							_phoneNumber_format))
				.When(customer => customer.PhoneNumber is not null);

				// Email - Optional
				RuleFor(customer => customer.Email).Cascade(CascadeMode.Stop)
					.NotEmptyNorWhitespace().WithMessage(
						ValidationRules.CUSTOMER_EMAIL_EMPTY_OR_WHITESPACE)
					.Email().WithMessage(ValidationRules.CUSTOMER_EMAIL_FORMAT)
				.When(customer => customer.Email is not null);
			});

			// Addresses
			RuleFor(customer => customer.Addresses)
				.NotEmpty().WithMessage(ValidationRules.CUSTOMER_ADDRESSES_COUNT_MIN)
			.ForEach(address => address.SetValidator(new AddressValidator()));

			// Notes
			RuleFor(customer => customer.Notes).Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(ValidationRules.CUSTOMER_NOTES_COUNT_MIN)
			.ForEach(note => note.SetValidator(new NoteValidator()));
		}

		public ValidationResult ValidateWithoutAddressesAndNotes(Customer customer) =>
			((IValidator<Customer>)this).Validate(customer,
				options => options.IncludeRuleSets("Own"));

		public ValidationResult ValidateFull(Customer customer) =>
			((IValidator<Customer>)this).Validate(customer,
				options => options.IncludeAllRuleSets());
	}
}

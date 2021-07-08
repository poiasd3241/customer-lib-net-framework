using System;
using System.Collections.Generic;
using CustomerLib.Business.Entities;
using CustomerLib.Business.Enums;
using CustomerLib.Business.Localization;
using FluentValidation;
using static FluentValidation.DefaultValidatorExtensions;

namespace CustomerLib.Business.Validators
{
	/// <summary>
	/// The fluent validator of <see cref="Address"/> objects.
	/// </summary>
	public class AddressValidator : AbstractValidator<Address>
	{
		private static readonly int _line_max_length = 100;
		private static readonly int _city_max_length = 50;
		private static readonly int _postalCode_max_length = 6;
		private static readonly int _state_max_length = 20;
		private static readonly List<string> _country_allowed =
			new() { "United States", "Canada" };

		public AddressValidator()
		{
			// AddressLine
			RuleFor(address => address.AddressLine).Cascade(CascadeMode.Stop)
				.NotNull().WithMessage(ValidationRules.ADDRESS_LINE_REQUIRED)
				.NotEmptyNorWhitespace().WithMessage(
					ValidationRules.ADDRESS_LINE_EMPTY_OR_WHITESPACE)
				.MaximumLength(_line_max_length).WithMessage(
					string.Format(ValidationRules.ADDRESS_LINE_MAX_LENGTH, _line_max_length));

			// AddressLine2 - Optional
			RuleFor(address => address.AddressLine2).Cascade(CascadeMode.Stop)
				.NotEmptyNorWhitespace().WithMessage(
					ValidationRules.ADDRESS_LINE2_EMPTY_OR_WHITESPACE)
				.MaximumLength(_line_max_length).WithMessage(
					string.Format(ValidationRules.ADDRESS_LINE2_MAX_LENGTH, _line_max_length))
			.When(address => address.AddressLine2 is not null);

			// Type
			RuleFor(address => address.Type).Cascade(CascadeMode.Stop)
				.Must(type => Enum.IsDefined(typeof(AddressType), type)).WithMessage(
					ValidationRules.ADDRESS_TYPE_UNKNOWN);

			// City
			RuleFor(address => address.City).Cascade(CascadeMode.Stop)
				.NotNull().WithMessage(ValidationRules.ADDRESS_CITY_REQUIRED)
				.NotEmptyNorWhitespace().WithMessage(
					ValidationRules.ADDRESS_CITY_EMPTY_OR_WHITESPACE)
				.MaximumLength(_city_max_length).WithMessage(
					string.Format(ValidationRules.ADDRESS_CITY_MAX_LENGTH, _city_max_length));

			// PostalCode
			RuleFor(address => address.PostalCode).Cascade(CascadeMode.Stop)
				.NotNull().WithMessage(ValidationRules.ADDRESS_POSTAL_CODE_REQUIRED)
				.NotEmptyNorWhitespace().WithMessage(
					ValidationRules.ADDRESS_POSTAL_CODE_EMPTY_OR_WHITESPACE)
				.MaximumLength(_postalCode_max_length).WithMessage(
					string.Format(ValidationRules.ADDRESS_POSTAL_CODE_MAX_LENGTH,
						_postalCode_max_length));

			// State
			RuleFor(address => address.State).Cascade(CascadeMode.Stop)
				.NotNull().WithMessage(ValidationRules.ADDRESS_STATE_REQUIRED)
				.NotEmptyNorWhitespace().WithMessage(
					ValidationRules.ADDRESS_STATE_EMPTY_OR_WHITESPACE)
				.MaximumLength(_state_max_length).WithMessage(
					string.Format(ValidationRules.ADDRESS_STATE_MAX_LENGTH, _state_max_length));

			// Country
			RuleFor(address => address.Country).Cascade(CascadeMode.Stop)
				.NotNull().WithMessage(ValidationRules.ADDRESS_COUNTRY_REQUIRED)
				.NotEmptyNorWhitespace().WithMessage(
				ValidationRules.ADDRESS_COUNTRY_EMPTY_OR_WHITESPACE)
				.Must(country => _country_allowed.Contains(country)).WithMessage(
					string.Format(ValidationRules.ADDRESS_COUNTRY_ALLOWED_LIST,
						string.Join(", ", _country_allowed)));
		}
	}
}

namespace CustomerLib.Business.Localization
{
	public static class ValidationRules
	{
		#region Note

		public const string NOTE_EMPTY_OR_WHITESPACE = "Note cannot be empty or whitespace.";
		public const string NOTE_MAX_LENGTH = "Note: max {0} characters.";

		#endregion

		#region Person

		public const string PERSON_FIRST_NAME_EMPTY_OR_WHITESPACE =
			"First name cannot be empty or whitespace.";
		public const string PERSON_FIRST_NAME_MAX_LENGTH = "First name: max {0} characters.";

		public const string PERSON_LAST_NAME_REQUIRED = "Last name is required.";
		public const string PERSON_LAST_NAME_EMPTY_OR_WHITESPACE =
			"Last name cannot be empty or whitespace.";
		public const string PERSON_LAST_NAME_MAX_LENGTH = "Last name: max {0} characters.";

		#endregion

		#region Customer

		public const string CUSTOMER_ADDRESSES_COUNT_MIN = "At least one address is required.";

		public const string CUSTOMER_PHONE_NUMBER_EMPTY_OR_WHITESPACE =
			"Phone number cannot be empty or whitespace.";
		public const string CUSTOMER_PHONE_NUMBER_FORMAT = "Phone number: must be in {0} format.";

		public const string CUSTOMER_EMAIL_EMPTY_OR_WHITESPACE =
			"Email cannot be empty or whitespace.";
		public const string CUSTOMER_EMAIL_FORMAT = "Invalid email.";

		public const string CUSTOMER_NOTES_COUNT_MIN = "At least one note is required.";
		public const string CUSTOMER_NOTES_TEXT_NULL_EMPTY_OR_EMPTY_OR_WHITESPACE =
			"Notes cannot be empty or consist of whitespace characters.";

		#endregion

		#region Address

		public const string ADDRESS_LINE_REQUIRED = "Address line is required.";
		public const string ADDRESS_LINE_EMPTY_OR_WHITESPACE =
			"Address line cannot be empty or whitespace.";
		public const string ADDRESS_LINE_MAX_LENGTH = "Address line: max {0} characters.";

		public const string ADDRESS_LINE2_EMPTY_OR_WHITESPACE =
			"Address line2 cannot be empty or whitespace.";
		public const string ADDRESS_LINE2_MAX_LENGTH = "Address line2: max {0} characters.";

		public const string ADDRESS_TYPE_UNKNOWN = "Unknown type.";

		public const string ADDRESS_CITY_REQUIRED = "City is required.";
		public const string ADDRESS_CITY_EMPTY_OR_WHITESPACE =
			"City cannot be empty or whitespace.";
		public const string ADDRESS_CITY_MAX_LENGTH = "City: max {0} characters.";

		public const string ADDRESS_POSTAL_CODE_REQUIRED = "Postal code is required.";
		public const string ADDRESS_POSTAL_CODE_EMPTY_OR_WHITESPACE =
			"Postal code cannot be empty or whitespace.";
		public const string ADDRESS_POSTAL_CODE_MAX_LENGTH = "Postal code: max {0} characters.";

		public const string ADDRESS_STATE_REQUIRED = "State is required.";
		public const string ADDRESS_STATE_EMPTY_OR_WHITESPACE =
			"State cannot be empty or whitespace.";
		public const string ADDRESS_STATE_MAX_LENGTH = "State: max {0} characters.";

		public const string ADDRESS_COUNTRY_REQUIRED = "Country is required.";
		public const string ADDRESS_COUNTRY_EMPTY_OR_WHITESPACE =
			"Country cannot be empty or whitespace.";
		public const string ADDRESS_COUNTRY_ALLOWED_LIST = "Country: allowed only {0}.";

		#endregion
	}
}

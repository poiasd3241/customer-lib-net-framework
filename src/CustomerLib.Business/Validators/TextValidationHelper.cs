namespace CustomerLib.Business.Validators
{
	public class TextValidationHelper
	{
		/// <param name="text">The string to check.</param>
		/// <returns>True if the string is empty "" or whitespace; otherwise, false.</returns>
		public static bool IsEmptyOrWhitespace(string text) =>
			text?.Length == 0 ||
			text?.Trim().Length == 0;
	}
}

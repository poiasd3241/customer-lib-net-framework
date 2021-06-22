using FluentValidation;

namespace CustomerLib.Business.Validators
{
	/// <summary>
	/// Extensions for <see cref="IRuleBuilder{T, TProperty}"/>.
	/// </summary>
	public static class RuleBuilderExtensions
	{
		public static IRuleBuilderOptions<T, string> NotEmptyNorWhitespace<T>(
			this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder.Must(
				text => TextValidationHelper.IsEmptyOrWhitespace(text) == false);

		public static IRuleBuilderOptions<T, string> PhoneNumberFormatE164<T>(
			this IRuleBuilder<T, string> ruleBuilder) =>
				ruleBuilder.Matches(@"^\+?[1-9]\d{1,14}$");

		public static IRuleBuilderOptions<T, string> Email<T>(
			this IRuleBuilder<T, string> ruleBuilder) =>
				ruleBuilder.Matches(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
	}
}

using CustomerLib.WebForms.Validation;
using Xunit;

namespace CustomerLib.WebForms.Tests.Validation
{
	public class CustomerInputValidatorTest
	{
		[Theory]
		[InlineData("123")]
		[InlineData("1,2")]
		[InlineData("1.2")]
		[InlineData("")]
		[InlineData(null)]
		public void ShouldValidateTotalPurchasesAmount(string input)
		{
			// Given
			decimal? value = string.IsNullOrEmpty(input)
				? null
				: decimal.Parse(input);

			var customerInputValidator = new CustomerInputValidator();

			// When
			var isValid = customerInputValidator.ValidateTotalPurchasesAmount(
				input, out decimal? resultValue);

			// Then
			Assert.True(isValid);
			Assert.Equal(value, resultValue);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("a")]
		[InlineData("1.1.1")]
		public void ShouldInvalidateTotalPurchasesAmount(string input)
		{
			// Given
			var customerInputValidator = new CustomerInputValidator();

			// When
			var isValid = customerInputValidator.ValidateTotalPurchasesAmount(
				input, out decimal? resultValue);

			// Then
			Assert.False(isValid);
			Assert.Null(resultValue);
		}
	}
}

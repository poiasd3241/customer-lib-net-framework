using CustomerLib.Business.Validators;
using Xunit;

namespace CustomerLib.Business.Tests.Validators
{
	public class TextValidationHelperTest
	{
		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		public void ShouldConfirmEmptyOrWhitespaceText(string text)
		{
			Assert.True(TextValidationHelper.IsEmptyOrWhitespace(text));
		}

		[Theory]
		[InlineData(null)]
		[InlineData(" a")]
		[InlineData("a ")]
		[InlineData(" a ")]
		[InlineData("a")]
		public void ShouldConfirmNotEmptyNorWhitespaceText(string text)
		{
			Assert.False(TextValidationHelper.IsEmptyOrWhitespace(text));
		}
	}
}

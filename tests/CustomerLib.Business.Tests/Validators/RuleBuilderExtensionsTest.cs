using CustomerLib.Business.Validators;
using FluentValidation;
using Xunit;

namespace CustomerLib.Business.Tests.Validators
{
	public class RuleBuilderExtensionsTest
	{
		#region Private Members

		private static readonly TestModelValidator _testModelValidator = new();
		private class TestModelValidator : AbstractValidator<TestModel>
		{
			public TestModelValidator()
			{
				RuleFor(model => model.NotEmptyNorWhitespace)
					.NotEmptyNorWhitespace().WithMessage("bad NotEmptyNorWhitespace text");
				RuleFor(model => model.PhoneNumberFormatE164Text)
					.PhoneNumberFormatE164().WithMessage("bad PhoneNumberFormatE164Text");
				RuleFor(model => model.EmailText)
					.Email().WithMessage("bad EmailText");
			}
		}

		private class TestModel
		{
			public string NotEmptyNorWhitespace { get; set; }
			public string PhoneNumberFormatE164Text { get; set; }
			public string EmailText { get; set; }
		}

		private static TestModel GetValidTestModel()
		{
			return new TestModel()
			{
				NotEmptyNorWhitespace = "a",
				PhoneNumberFormatE164Text = "+123",
				EmailText = "my@email.com",
			};
		}

		#endregion

		#region Test model

		[Fact]
		public void ShouldCreateTestModel()
		{
			TestModel model = new();

			Assert.Null(model.NotEmptyNorWhitespace);
			Assert.Null(model.PhoneNumberFormatE164Text);
			Assert.Null(model.EmailText);
		}

		[Fact]
		public void ShouldSetTestModelProperties()
		{
			var text = "a";

			TestModel model = new();

			model.NotEmptyNorWhitespace = text;
			model.PhoneNumberFormatE164Text = text;
			model.EmailText = text;

			Assert.Equal(text, model.NotEmptyNorWhitespace);
			Assert.Equal(text, model.PhoneNumberFormatE164Text);
			Assert.Equal(text, model.EmailText);
		}

		#endregion

		#region RuleBuilder Extensions

		[Fact]
		public void ShouldValidateTestModel()
		{
			// Given
			var validModel = GetValidTestModel();

			// When
			var result = _testModelValidator.Validate(validModel);

			// Then
			Assert.True(result.IsValid);
		}

		[Fact]
		public void ShouldInvalidateTestModelByFormat()
		{
			// Given
			var invalidModel = GetValidTestModel();
			invalidModel.PhoneNumberFormatE164Text = "1-23";
			invalidModel.EmailText = "@email";

			// When
			var errors = _testModelValidator.Validate(invalidModel).Errors;

			// Then
			Assert.Equal(2, errors.Count);
			Assert.Equal("bad PhoneNumberFormatE164Text", errors[0].ErrorMessage);
			Assert.Equal("bad EmailText", errors[1].ErrorMessage);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		public void ShouldInvalidateTestModelByEmptyOrWhitespace(string text)
		{
			// Given
			var invalidModel = GetValidTestModel();
			invalidModel.NotEmptyNorWhitespace = text;

			// When
			var errors = _testModelValidator.Validate(invalidModel).Errors;

			// Then
			Assert.Single(errors);
			Assert.Equal("bad NotEmptyNorWhitespace text", errors[0].ErrorMessage);
		}

		#endregion
	}
}

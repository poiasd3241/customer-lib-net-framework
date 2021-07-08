using System;
using CustomerLib.Business.Entities;
using CustomerLib.WebForms.Validation;
using Xunit;

namespace CustomerLib.WebForms.Tests.Validation
{
	public class EntityInputValidatorTest
	{
		private class ValidateEntityPropertyData : TheoryData<Entity, string>
		{
			public ValidateEntityPropertyData()
			{
				Add(new Customer() { FirstName = "Correct" }, "FirstName");
				Add(new Address() { State = "Correct" }, "State");
				Add(new Note() { Content = "Correct" }, "Content");
			}
		}

		private class InvalidateEntityPropertyData : TheoryData<Entity, string>
		{
			public InvalidateEntityPropertyData()
			{
				Add(new Customer() { FirstName = " " }, "FirstName");
				Add(new Address() { State = " " }, "State");
				Add(new Note() { Content = " " }, "Content");
			}
		}

		private class UnknownEntity : Entity { }

		#region Validate property (result)

		[Theory]
		[ClassData(typeof(ValidateEntityPropertyData))]
		public void ShouldValidatePropertyResult(Entity entity, string propertyName)
		{
			// Given
			var entityInputValidator = new EntityInputValidator();

			// When
			var result = entityInputValidator.ValidatePropertyResult(entity, propertyName);

			// Then
			Assert.True(result.IsValid);
		}

		[Theory]
		[ClassData(typeof(InvalidateEntityPropertyData))]
		public void ShouldInvalidatePropertyResult(Entity entity, string propertyName)
		{
			// Given
			var entityInputValidator = new EntityInputValidator();

			// When
			var result = entityInputValidator.ValidatePropertyResult(entity, propertyName);

			// Then
			Assert.False(result.IsValid);
			var error = Assert.Single(result.Errors);
			Assert.Equal(propertyName, error.PropertyName);
		}

		[Fact]
		public void ShouldThrowOnValidatePropertyResultByUnknownEntity()
		{
			// Given
			var unknownEntity = new UnknownEntity();
			var propertyName = "whatever";

			var entityInputValidator = new EntityInputValidator();

			// When
			var exception = Assert.Throws<ArgumentException>(() =>
				entityInputValidator.ValidatePropertyResult(unknownEntity, propertyName));

			// Then
			Assert.Equal("propertyContainer", exception.ParamName);
		}

		#endregion

		#region Validate property (bool)

		[Theory]
		[ClassData(typeof(ValidateEntityPropertyData))]
		public void ShouldValidateProperty(Entity entity, string propertyName)
		{
			// Given
			var entityInputValidator = new EntityInputValidator();

			// When
			var result = entityInputValidator.ValidateProperty(entity, propertyName);

			// Then
			Assert.True(result);
		}

		[Theory]
		[ClassData(typeof(InvalidateEntityPropertyData))]
		public void ShouldInvalidateProperty(Entity entity, string propertyName)
		{
			// Given
			var entityInputValidator = new EntityInputValidator();

			// When
			var result = entityInputValidator.ValidateProperty(entity, propertyName);

			// Then
			Assert.False(result);
		}

		[Fact]
		public void ShouldThrowOnValidatePropertyByUnknownEntity()
		{
			// Given
			var unknownEntity = new UnknownEntity();
			var propertyName = "whatever";

			var entityInputValidator = new EntityInputValidator();

			// When
			var exception = Assert.Throws<ArgumentException>(() =>
				entityInputValidator.ValidateProperty(unknownEntity, propertyName));

			// Then
			Assert.Equal("propertyContainer", exception.ParamName);
		}

		#endregion
	}
}

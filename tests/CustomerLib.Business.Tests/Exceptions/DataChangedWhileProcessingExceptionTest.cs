using System;
using CustomerLib.Business.Exceptions;
using Xunit;

namespace CustomerLib.Business.Tests.Exceptions
{
	public class DataChangedWhileProcessingExceptionTest
	{
		[Fact]
		public void ShouldCreateDataChangedWhileProcessingException()
		{
			var message = "oops";
			var inner = new Exception();

			var defaultConstructor = new DataChangedWhileProcessingException();
			var messageConstructor = new DataChangedWhileProcessingException(message);

			var messageWithInnerConstructor = new DataChangedWhileProcessingException(
				message, inner);

			Assert.NotNull(defaultConstructor);

			Assert.Equal(message, messageConstructor.Message);

			Assert.Equal(message, messageWithInnerConstructor.Message);
			Assert.Equal(inner, messageWithInnerConstructor.InnerException);
		}

		// TODO: test Serialization...
	}
}

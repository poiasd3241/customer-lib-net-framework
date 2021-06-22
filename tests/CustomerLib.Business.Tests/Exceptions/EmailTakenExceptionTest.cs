using System;
using CustomerLib.Business.Exceptions;
using Xunit;

namespace CustomerLib.Business.Tests.Exceptions
{
	public class EmailTakenExceptionTest
	{
		[Fact]
		public void ShouldCreateEmailTakenException()
		{
			var message = "oops";
			var inner = new Exception();

			var defaultConstructor = new EmailTakenException();
			var messageConstructor = new EmailTakenException(message);

			var messageWithInnerConstructor = new EmailTakenException(message, inner);

			Assert.NotNull(defaultConstructor);

			Assert.Equal(message, messageConstructor.Message);

			Assert.Equal(message, messageWithInnerConstructor.Message);
			Assert.Equal(inner, messageWithInnerConstructor.InnerException);
		}

		// TODO: test Serialization...
	}
}

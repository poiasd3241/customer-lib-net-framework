using System;

namespace CustomerLib.Business.Exceptions
{
	/// <summary>
	/// Throw when the email is already taken.
	/// </summary>
	[Serializable]
	public class EmailTakenException : Exception
	{
		public EmailTakenException() { }
		public EmailTakenException(string message) : base(message) { }
		public EmailTakenException(string message, Exception inner) : base(message, inner) { }
		protected EmailTakenException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}

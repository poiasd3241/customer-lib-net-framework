using System;
using System.Runtime.Serialization;

namespace CustomerLib.Business.Exceptions
{
	/// <summary>
	/// Throw when the entity fails validation.
	/// </summary>
	[Serializable]
	public class EntityValidationException : Exception
	{
		public EntityValidationException() { }
		public EntityValidationException(string message) : base(message) { }
		public EntityValidationException(string message, Exception inner) : base(message, inner) { }
		protected EntityValidationException(
		  SerializationInfo info,
		  StreamingContext context) : base(info, context) { }
	}
}

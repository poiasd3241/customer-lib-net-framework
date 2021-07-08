using System;
using System.Runtime.Serialization;

namespace CustomerLib.Business.Exceptions
{
	[Serializable]
	public class DataChangedWhileProcessingException : Exception
	{
		public DataChangedWhileProcessingException() { }
		public DataChangedWhileProcessingException(string message) : base(message) { }
		public DataChangedWhileProcessingException(string message, Exception inner)
			: base(message, inner) { }
		protected DataChangedWhileProcessingException(
		  SerializationInfo info,
		  StreamingContext context) : base(info, context) { }
	}
}

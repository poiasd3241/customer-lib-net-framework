using Moq;

namespace CustomerLib.TestHelpers
{
	/// <summary>
	/// The <see cref="Mock"/> with <see cref="MockBehavior.Strict"/>.
	/// </summary>
	/// <typeparam name="T">The type to create the mock implementation for.</typeparam>
	public class StrictMock<T> : Mock<T> where T : class
	{
		/// <summary>
		/// The default constructor.
		/// Sets the <see cref="Behavior"/> to <see cref="MockBehavior.Strict"/>.
		/// </summary>
		public StrictMock() : base(MockBehavior.Strict) { }
	}
}

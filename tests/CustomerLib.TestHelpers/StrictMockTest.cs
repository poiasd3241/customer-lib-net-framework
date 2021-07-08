using Moq;
using Xunit;

namespace CustomerLib.TestHelpers
{
	public class StrictMockTest
	{
		private class Whatever { }

		[Fact]
		public void ShouldCreateMockWithStrictBehavior()
		{
			var mock = new StrictMock<Whatever>();

			Assert.Equal(MockBehavior.Strict, mock.Behavior);
		}
	}
}

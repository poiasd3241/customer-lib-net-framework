using CustomerLib.Data.Repositories.EF;
using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories.EF
{
	public class CustomerLibDataContextTest
	{
		[Fact]
		public void ShouldCreateDataContext()
		{
			var context = new CustomerLibDataContext();

			Assert.NotNull(context.Customers);
			Assert.NotNull(context.Addresses);
			Assert.NotNull(context.Notes);
		}
	}
}

using CustomerLib.Data.Repositories.Implementations;
using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories.TestHelpers
{
	[Collection(nameof(NotDbSafeResourceCollection))]
	public class AddressTypeHelperRepositoryTest
	{
		[Fact]
		public void ShouldUnsafeRepopulateAddressTypes()
		{
			// Free up FK.
			AddressRepository.DeleteAll();

			AddressTypeHelperRepository.UnsafeRepopulateAddressTypes();
		}
	}
}

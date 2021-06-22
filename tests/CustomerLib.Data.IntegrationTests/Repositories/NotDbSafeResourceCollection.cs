using Xunit;

namespace CustomerLib.Data.IntegrationTests.Repositories
{
	[CollectionDefinition(nameof(NotDbSafeResourceCollection), DisableParallelization = true)]
	public class NotDbSafeResourceCollection
	{ }
}

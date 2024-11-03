using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Core.MockProviders;

namespace SnowboardShop.Api.Tests.Integration.Tests.TestCollections;

[CollectionDefinition(ApiSeedTestCollectionName)]
public class ApiSeedTestCollection : ICollectionFixture<SnowboardsApiFactory<EmptyMocksProvider>>
{
    public const string ApiSeedTestCollectionName = "Api Seed Test Collection";
}
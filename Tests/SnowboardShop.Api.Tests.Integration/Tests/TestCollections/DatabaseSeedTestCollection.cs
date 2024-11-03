using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Core.MockProviders;

namespace SnowboardShop.Api.Tests.Integration.Tests.TestCollections;

[CollectionDefinition(DatabaseSeedTestCollectionName)]
public class DatabaseSeedTestCollection : ICollectionFixture<SnowboardsApiFactory<MocksProvider>>
{
   public const string DatabaseSeedTestCollectionName = "Database Seed Test Collection";
}



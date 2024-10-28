using SnowboardShop.Api.Tests.Integration.Core.Factories;

namespace SnowboardShop.Api.Tests.Integration.Tests;

[CollectionDefinition(ApiFactoryTestCollectionName)]
public class ApiFactoryTestCollection : ICollectionFixture<TestContainersSnowboardsApiFactory>
{
    public const string ApiFactoryTestCollectionName = "ApiFactory TestCollection";
}
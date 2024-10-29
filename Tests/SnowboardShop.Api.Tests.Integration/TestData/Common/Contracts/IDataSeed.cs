namespace SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;

public interface IDataSeed
{
    public string Name { get; }

    Task SeedAsync();

    Task ClearAsync();
}

public interface IDataSeed<TModel> : IDataSeed
{
    IReadOnlyCollection<TModel> GetAllData();
}
namespace SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;

public interface IDataSeed
{
    public string Name { get; }

    Task SeedAsync(CancellationToken token = default);

    Task ClearAsync(CancellationToken token = default);
}

public interface IDataSeed<TModel> : IDataSeed
{
    IReadOnlyCollection<TModel> GetAllData();
}
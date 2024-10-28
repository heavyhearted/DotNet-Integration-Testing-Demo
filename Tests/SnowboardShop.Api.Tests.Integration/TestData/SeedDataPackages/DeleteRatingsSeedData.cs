using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Application.Models;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages;

public class DeleteRatingsSeedData : IDataSeed<Snowboard>
{
    public IReadOnlyCollection<Snowboard> GetAllData()
    {
        return new List<Snowboard>();
    }

    public string Name => nameof(DeleteRatingsSeedData);
    
    public Task SeedAsync(CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken token = default)
    {
        return Task.CompletedTask;
    }
}
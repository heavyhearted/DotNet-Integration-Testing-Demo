using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;

namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public class DataSeedFactory
{
    private readonly IEnumerable<IDataSeed> _dataSeedPackages;

    public DataSeedFactory(IEnumerable<IDataSeed> dataSeedPackages)
    {
        _dataSeedPackages = dataSeedPackages;
    }
    
    // This generic method allows to get a specific data seed package by its name.
    public TDataSeed GetDataSeed<TDataSeed>(string name) where TDataSeed : IDataSeed
    {
        var dataSeed = _dataSeedPackages
            .OfType<TDataSeed>()
            .First(x => x.Name == name);

        return dataSeed;
    }
    
    // New List is required to avoid modifying the original list which is shared between tests.
    public List<IDataSeed> GetAllDataSeeds()
    {
        return new List<IDataSeed>(_dataSeedPackages); 
    }
}
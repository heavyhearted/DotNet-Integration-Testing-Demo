using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;

namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public class DataSeedFactory
{
    private readonly IEnumerable<IDataSeed> _dataSeedPackages;

    public DataSeedFactory(IEnumerable<IDataSeed> dataSeedPackages)
    {
        _dataSeedPackages = dataSeedPackages;
    }
    
    public IDataSeed<TModel> GetDataSeed<TModel>(string name) where TModel : class
    {
        var dataSeed = _dataSeedPackages
            .OfType<IDataSeed<TModel>>()
            .First(x => x.Name == name);

        return dataSeed;
    }
}
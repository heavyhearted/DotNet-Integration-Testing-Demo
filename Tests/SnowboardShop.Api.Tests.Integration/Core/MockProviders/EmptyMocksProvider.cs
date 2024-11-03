using Microsoft.Extensions.DependencyInjection;

namespace SnowboardShop.Api.Tests.Integration.Core.MockProviders;

public class EmptyMocksProvider : IMocksProvider
{
    public void RegisterMocks(IServiceCollection serviceCollection)
    {
    }

    public void ResetAllMocks()
    {
    }
}
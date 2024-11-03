using Microsoft.Extensions.DependencyInjection;

namespace SnowboardShop.Api.Tests.Integration.Core.MockProviders;

public interface IMocksProvider
{
    void RegisterMocks(IServiceCollection serviceCollection);

    void ResetAllMocks();
}
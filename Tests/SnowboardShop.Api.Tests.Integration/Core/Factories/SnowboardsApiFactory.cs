using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public class SnowboardsApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<ITokenService, TokenService>();
        });
    }

    public RestClient CreateRestClient(ITestOutputHelper? testOutputHelper = default)
    {
        RestClient client;
        HttpClient baseClient;
        
        if (testOutputHelper != null)
        {
            var handler = RequestLoggingDelegatingHandlerFactory.Create(testOutputHelper);
            baseClient = CreateDefaultClient(handler);
            client = new RestClient(baseClient);
            return client;
        }
        
        baseClient = CreateDefaultClient();
        client = new RestClient(baseClient);

        return client;
    }
    
    public RestClient CreateRestClient(string overrideAccessToken, ITestOutputHelper? testOutputHelper = default)
    {
        var client = CreateRestClient(testOutputHelper);
        client.AddDefaultHeader("Authorization", $"Bearer {overrideAccessToken}");

        return client;
    }
    
    public async Task<RestClient> CreateAuthenticatedRestClientAsync(ITestOutputHelper? testOutputHelper = default)
    {
        var client = CreateRestClient(testOutputHelper);
        var tokenService = Services.GetRequiredService<ITokenService>();
        var token = await tokenService.GetTokenAsync();
        client.AddDefaultHeader("Authorization", $"Bearer {token}");

        return client;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
        
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}


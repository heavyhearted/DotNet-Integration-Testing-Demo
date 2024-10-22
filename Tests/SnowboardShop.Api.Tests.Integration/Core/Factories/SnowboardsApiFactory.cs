using System.ComponentModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Identity.Api.FakeVault.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Application.Database;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;
using IContainer = DotNet.Testcontainers.Containers.IContainer;


namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public class SnowboardsApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private IContainer _identityApiContainer;

    public SnowboardsApiFactory()
    {
        DotNetEnv.Env.Load("Helpers/Identity.Api/FakeVault/DemoSecrets/demo.env");
        var certificatePassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");

        // Initialize PostgreSQL container using credentials from FakeVault
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase(FakeVault.DatabaseName)
            .WithUsername(FakeVault.DatabaseUsername)
            .WithPassword(FakeVault.DatabasePassword)
            .Build();

        // Build the Identity API Docker image and create the container
        var identityApiImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "Helpers/Identity.Api")
            .WithDockerfile("Dockerfile")
            .Build();
        
        identityApiImage.CreateAsync().GetAwaiter().GetResult();

        // Ensure the image is created asynchronously before being used
        _identityApiContainer = new ContainerBuilder()
            .WithImage(identityApiImage)  // Use the built Identity API image
            .WithPortBinding(5002, 5002)  // HTTP port
            .WithPortBinding(5003, 5003)  // HTTPS port
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            .WithEnvironment("CERTIFICATE_PASSWORD", certificatePassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5003))
            .Build();
    }


    public async Task InitializeAsync()
    {
        await _identityApiContainer.StartAsync();
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _identityApiContainer.StopAsync();
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IAccessTokenService, AccessTokenService>();
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(_dbContainer.GetConnectionString()));
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
        var tokenService = Services.GetRequiredService<IAccessTokenService>();
        var token = await tokenService.GetTokenAsync();
        client.AddDefaultHeader("Authorization", $"Bearer {token}");

        return client;
    }
}
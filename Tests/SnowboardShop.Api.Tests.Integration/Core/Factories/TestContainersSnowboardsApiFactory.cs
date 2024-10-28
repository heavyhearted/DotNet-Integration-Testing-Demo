using System.Reflection;
using DotNet.Testcontainers.Builders;
using Identity.Api.FakeVault.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Application.Database;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;
using IContainer = DotNet.Testcontainers.Containers.IContainer;


namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public class TestContainersSnowboardsApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime, IApiFactory
{
    private PostgreSqlContainer _dbContainer = default!;
    private IContainer _identityApiContainer = default!;

    public MocksProvider MocksProvider { get; } = new();

    public async Task InitializeAsync()
    {
        await InitializeContainersAsync();
        await _identityApiContainer.StartAsync();
        await _dbContainer.StartAsync();

        var dataSeedPackages = Services.GetServices<IDataSeed>();

        foreach (var dataSeedPackage in dataSeedPackages)
        {
            await dataSeedPackage.SeedAsync();
        }
    }

    public new async Task DisposeAsync()
    {
        await _identityApiContainer.StopAsync();
        await _dbContainer.StopAsync();

        var dataSeedPackages = Services.GetServices<IDataSeed>();

        foreach (var dataSeedPackage in dataSeedPackages)
        {
            await dataSeedPackage.ClearAsync();
        }
    }

    private async Task InitializeContainersAsync()
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

        await identityApiImage.CreateAsync();

        // Ensure the image is created asynchronously before being used
        _identityApiContainer = new ContainerBuilder()
            .WithImage(identityApiImage) // Use the built Identity API image
            .WithPortBinding(5002, true) // HTTP port
            .WithPortBinding(5003, true) // HTTPS port
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            .WithEnvironment("CERTIFICATE_PASSWORD", certificatePassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5003))
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var identityApiSettings = new IdentityApiSettings
            {
                HttpPort = _identityApiContainer.GetMappedPublicPort(5003)
            };

            services.AddSingleton(identityApiSettings);
            services.AddSingleton<IAccessTokenService, AccessTokenService>();
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(_dbContainer.GetConnectionString()));


            // Register all data seed packages
            var dataSeedPackages = Assembly.GetAssembly(typeof(IDataSeed))!
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(IDataSeed)))
                .ToList();

            foreach (var dataSeedPackage in dataSeedPackages)
            {
                services.AddSingleton(dataSeedPackage);
            }

            services.AddSingleton<DataSeedFactory>();

            MocksProvider.RegisterMocks(services);
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

    public async Task<RestClient> CreateAuthenticatedRestClientAsync(
        UserRoles roles = UserRoles.Admin | UserRoles.TrustedMember,
        ITestOutputHelper? testOutputHelper = default)
    {
        var client = CreateRestClient(testOutputHelper);

        var externalPort = _identityApiContainer.GetMappedPublicPort(5003);
        var tokenService = Services.GetRequiredService<IAccessTokenService>();
        var token = await tokenService.GetTokenAsync(roles);

        client.AddDefaultHeader("Authorization", $"Bearer {token}");

        return client;
    }

    public async Task<RestClient> CreateAuthenticatedRestClientAsync(ITestOutputHelper? testOutputHelper = default)
    {
        return await CreateAuthenticatedRestClientAsync(UserRoles.Admin | UserRoles.TrustedMember, testOutputHelper);
    }
}
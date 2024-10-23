using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class DeleteSnowboardTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string DeleteSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Delete;
    private const string CreateSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Create;
    private const string GetSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Get;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly CreateSnowboardFaker _snowboardFaker = new();
    private readonly HashSet<Guid> _createdIds = new();

    public DeleteSnowboardTests(ITestOutputHelper output, SnowboardsApiFactory apiFactory)
    {
        _output = output;
        _apiFactory = apiFactory;
    }

    public Task InitializeAsync() => Task.CompletedTask;


    public async Task DisposeAsync()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        foreach (var id in _createdIds)
        {
            var request = new RestRequest(DeleteSnowboardEndpoint, Method.Delete);
            request.AddUrlSegment("id", id);

            await restClient.DeleteAsync(request);
        }
    }
}
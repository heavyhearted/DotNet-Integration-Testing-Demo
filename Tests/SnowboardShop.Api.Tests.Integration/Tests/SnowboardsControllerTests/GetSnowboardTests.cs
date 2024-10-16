using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestData;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class GetSnowboardTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string GetSnowboardEndpoint = ApiEndpoints.Snowboards.Get;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;

    public GetSnowboardTests(SnowboardsApiFactory apiFactory, ITestOutputHelper output)
    {
        _apiFactory = apiFactory;
        _output = output;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }


    [Theory]
    [ClassData(typeof(InvalidSnowboardGuidTheoryData))]
    public async Task Get_ReturnsNotFound_WhenSnowboardDoesNotExist(string guidAsText)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var request = new RestRequest(GetSnowboardEndpoint);
        request.AddUrlSegment("idOrSlug", guidAsText);
        
        var response = await restClient.ExecuteGetAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
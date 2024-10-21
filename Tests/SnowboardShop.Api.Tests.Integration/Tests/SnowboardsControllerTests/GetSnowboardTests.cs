using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestData;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardController;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class GetSnowboardTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string GetSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Get;
    private const string CreateSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Create;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly CreateSnowboardFaker _snowboardFaker = new();

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

    [Fact]
    public async Task Get_ReturnsSnowboard_WhenSnowboardExists()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        
        var postRequest = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        postRequest.AddJsonBody(_snowboardFaker.Generate());
        var postResponse = await restClient.ExecutePostAsync<SnowboardResponse>(postRequest);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var postResponseContent = JsonSerializer.Deserialize<SnowboardResponse>(postResponse.Content!);
        var snowboardId = postResponseContent!.Id;
        
        var request = new RestRequest(GetSnowboardEndpoint);
        request.AddUrlSegment("idOrSlug", snowboardId);

        var response = await restClient.ExecuteGetAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(NonExistingSnowboardGuidTheoryData))]
    public async Task Get_ReturnsNotFound_WhenSnowboardDoesNotExist(string guidAsText)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var request = new RestRequest(GetSnowboardEndpoint);
        request.AddUrlSegment("idOrSlug", guidAsText);
        
        var response = await restClient.ExecuteGetAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SnowboardShop.Api.Controllers;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.SnowboardsControllerTestData;
using SnowboardShop.Api.Tests.Integration.TestUtilities.Logging;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class CreateSnowboardTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string CreateSnowboardEndpoint = ApiEndpoints.Snowboards.Create;
    
    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly CreateSnowboardFaker _snowboardFaker = new();

    public CreateSnowboardTests(SnowboardsApiFactory apiFactory, ITestOutputHelper output)
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
    [ClassData(typeof(SnowboardsValidTestData))]
    public async Task CreateSnowboardRequest_ShouldSucceed(CreateSnowboardRequest snowboardRequest)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(snowboardRequest);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // TestLogger.LogRequestResponse(_output, request, response);
    }
    
    [Fact(DisplayName = $"{nameof(SnowboardsController.Create)} Invalid authentication should return HTTP 401 Unauthorized")]
    public async Task InvalidAuthentication_ShouldReturnUnauthorized()
    {
        var restClient = _apiFactory.CreateRestClient("invalid_token");
        
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(_snowboardFaker.Generate());

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        TestLogger.LogRequestResponse(_output, request, response);
    }
}
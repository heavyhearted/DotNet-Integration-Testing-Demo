using System.ComponentModel;
using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

[Collection(ApiFactoryTestCollection.ApiFactoryTestCollectionName)]
public class GetSnowboardTests : IAsyncLifetime
{
    private const string GetSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Get;
    private const string DeleteSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Delete;

    private readonly ITestOutputHelper _output;
    private readonly TestContainersSnowboardsApiFactory _apiFactory;
    private readonly HashSet<Guid> _createdIds = new();
    
    public GetSnowboardTests(TestContainersSnowboardsApiFactory apiFactory, ITestOutputHelper output)
    {
        _apiFactory = apiFactory;
        _apiFactory.MocksProvider.SetupUserContextService(Guid.NewGuid());
        
        _output = output;
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
        
        _apiFactory.MocksProvider.ResetAllMocks();
    }
    
    [Fact]
    [DisplayName("Get Snowboard With Authentication Should Succeed")]
    public async Task GetSnowboard_With_Authentication_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync
            (_apiFactory, _output, _createdIds);
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var getRequest = new RestRequest(GetSnowboardEndpoint);
        getRequest.AddUrlSegment("idOrSlug", createdSnowboard.Id);
      
        var getResponse = await restClient.ExecuteGetAsync<SnowboardResponse>(getRequest);
        
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    [DisplayName("Get Snowboard Without Authentication Should Succeed")]
    public async Task GetSnowboard_Without_Authentication_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync
            (_apiFactory, _output, _createdIds);
        var unauthenticatedRestClient = _apiFactory.CreateRestClient(_output);
        var getRequest = new RestRequest(GetSnowboardEndpoint);
        getRequest.AddUrlSegment("idOrSlug", createdSnowboard.Id);
        
        var getResponse = await unauthenticatedRestClient.ExecuteGetAsync<SnowboardResponse>(getRequest);
       
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    [DisplayName("Get Snowboard By Valid Slug Should Succeed")]
    public async Task GetSnowboard_ByValidSlug_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync
            (_apiFactory, _output, _createdIds);
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var validSlug = createdSnowboard.Slug;
        var getRequest = new RestRequest(GetSnowboardEndpoint);
        getRequest.AddUrlSegment("idOrSlug", validSlug);
        
        var getResponse = await restClient.ExecuteGetAsync<SnowboardResponse>(getRequest);
        
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.Data!.Slug.Should().Be(validSlug);
    }


    [Theory]
    [DisplayName("Get Snowboard With Non-Existing Id Should Return NotFound")]
    [ClassData(typeof(NonExistingSnowboardGuidTheoryData))]
    public async Task GetSnowboard_WithNonExistingId_ShouldReturnNotFound(string guidAsText)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(GetSnowboardEndpoint);
        request.AddUrlSegment("idOrSlug", guidAsText);
        
        var response = await restClient.ExecuteGetAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [DisplayName("Get Snowboard With Non-Existing Slug Should Return NotFound")]
    public async Task GetSnowboard_WithNonExistingSlug_ShouldReturnNotFound()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var invalidId = "non-existing-slug-2025";
        var request = new RestRequest(GetSnowboardEndpoint);
        request.AddUrlSegment("id", invalidId);

        var response = await restClient.ExecuteGetAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [DisplayName("Get Snowboard Without Id or Slug Should Return NotFound")]
    public async Task GetSnowboard_Without_IdOrSlug_ShouldReturnNotFound()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(GetSnowboardEndpoint);

        var response = await restClient.ExecuteGetAsync<SnowboardResponse>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [DisplayName("Get Snowboard With SQL Injection Attempt Should Return NotFound")]
    public async Task GetSnowboard_WithSqlInjectionAttempt_ShouldReturnNotFound()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var sqlInjectionString = "'; DROP TABLE Snowboards;--";
        var request = new RestRequest(GetSnowboardEndpoint);
        request.AddUrlSegment("idOrSlug", sqlInjectionString);

        var response = await restClient.ExecuteGetAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}
using System.ComponentModel;
using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestUtilities.AssertionHelpers;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class GetAllSnowboardsTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string GetAllSnowboardsEndpoint = Core.ApiEndpoints.Snowboards.GetAll;
    private const string DeleteSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Delete;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly HashSet<Guid> _createdIds = new();
    
    public GetAllSnowboardsTests(SnowboardsApiFactory apiFactory, ITestOutputHelper output)
    {
        _apiFactory = apiFactory;
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
    }
    
    [Fact]
    [DisplayName("Get All Snowboards With Authentication Should Succeed")]
    public async Task GetAllSnowboards_WithAuthentication_ShouldSucceed()
    {
        var firstSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var secondSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var thirdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        
        var getAllRequest = new RestRequest(GetAllSnowboardsEndpoint);
        var getAllResponse = await restClient.ExecuteGetAsync<SnowboardsResponse>(getAllRequest);
        
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var returnedSnowboards = getAllResponse.Data!.Items;
        var expectedSnowboards = new[] { firstSnowboard, secondSnowboard, thirdSnowboard };

        SnowboardAssertions.AssertMultipleSnowboardResponses(returnedSnowboards, expectedSnowboards);
    }
    
    [Fact]
    [DisplayName("Get All Snowboards Without Authentication Should Succeed")]
    public async Task GetAllSnowboards_WithoutAuthentication_ShouldSucceed()
    {
        var firstSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var secondSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var thirdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        
        var restClient = _apiFactory.CreateRestClient(_output);
        
        var getAllRequest = new RestRequest(GetAllSnowboardsEndpoint);
        var getAllResponse = await restClient.ExecuteGetAsync<SnowboardsResponse>(getAllRequest);
        
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var returnedSnowboards = getAllResponse.Data!.Items;
        var expectedIds = new[] { firstSnowboard.Id, secondSnowboard.Id, thirdSnowboard.Id };
        var returnedIds = returnedSnowboards.Select(snowboard => snowboard.Id);
        returnedIds.Should().BeEquivalentTo(expectedIds);
    }
    
    [Fact]
    [DisplayName("Get All Snowboards When No Snowboards Exist Should Return Empty List")]
    public async Task GetAllSnowboards_WhenNoSnowboardsExist_ShouldReturnEmptyList()
    {
        var restClient = _apiFactory.CreateRestClient(_output);
    
        var getAllRequest = new RestRequest(GetAllSnowboardsEndpoint);
        var getAllResponse = await restClient.ExecuteGetAsync<SnowboardsResponse>(getAllRequest);
    
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getAllResponse.Data.Should().NotBeNull();
        getAllResponse.Data!.Items.Should().BeEmpty();
    }
}
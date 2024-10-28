using System.ComponentModel;
using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.RatingsController;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.RatingsControllerTests;

[Collection(ApiFactoryTestCollection.ApiFactoryTestCollectionName)]
public class GetAllRatingsTests : IAsyncLifetime
{
    private const string GetAllRatingsEndpoint = Core.ApiEndpoints.Ratings.GetUserRatings;
    private const string DeleteRatingEndpoint = Core.ApiEndpoints.Ratings.DeleteRating;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly HashSet<Guid> _createdIds = new();
    
    public GetAllRatingsTests(SnowboardsApiFactory apiFactory, ITestOutputHelper output)
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
            // Delete ratings for the snowboard due to foreign key constraint
            var deleteRatingsRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
            deleteRatingsRequest.AddUrlSegment("id", id);

            await restClient.ExecuteAsync(deleteRatingsRequest);

            var deleteSnowboardRequest = new RestRequest(Core.ApiEndpoints.Snowboards.Delete, Method.Delete);
            deleteSnowboardRequest.AddUrlSegment("id", id);
            await restClient.ExecuteAsync(deleteSnowboardRequest);
        }
        
        _apiFactory.MocksProvider.ResetAllMocks();
    }

    
    [Theory]
    [ClassData(typeof(SnowboardRatingsTheoryData))]
    [DisplayName("Get All Ratings With Multiple Ratings Should Return All Items")]
    public async Task GetAllRatings_WithMultipleRatings_ShouldReturnAllItems(
        Dictionary<CreateSnowboardRequest, RateSnowboardRequest> dataSeed)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        foreach (var createAndRateRequest in dataSeed)
        {
            await SnowboardTestUtilities.CreateAndRateSnowboard(restClient, createAndRateRequest.Key, createAndRateRequest.Value, _createdIds);
        }
        
        var request = new RestRequest(GetAllRatingsEndpoint);
        var response = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull().And.HaveCount(dataSeed.Count);
    
        // Verify each snowboard has the correct rating
        for (int i = 0; i < dataSeed.Count; i++)
        {
            response.Data.Should().ContainSingle(r => r.Rating == (i + 1)); // Each snowboard has rating 1 to 5
        }
    }
}
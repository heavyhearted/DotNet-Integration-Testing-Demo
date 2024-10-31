using System.ComponentModel;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Core.MockProviders;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData;
using SnowboardShop.Api.Tests.Integration.Tests.TestCollections;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData.DeleteRatingConstants;

namespace SnowboardShop.Api.Tests.Integration.Tests.RatingsControllerTests;

[Collection(DatabaseSeedTestCollection.DatabaseSeedTestCollectionName)]
public class DeleteRatingTests : IAsyncLifetime
{
    private const string DeleteRatingEndpoint = Core.ApiEndpoints.Ratings.DeleteRating;
    private const string GetUserRatingsEndpoint = Core.ApiEndpoints.Ratings.GetUserRatings;
    
    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory<MocksProvider> _apiFactory;
    private readonly List<IDataSeed> _dataSeedPackages;
    private readonly DataSeedFactory _dataSeedFactory;

    public DeleteRatingTests(SnowboardsApiFactory<MocksProvider> apiFactory, ITestOutputHelper output)
    {
        _apiFactory = apiFactory;
        _apiFactory.MocksProvider.SetupUserContextService(Guid.NewGuid());

        _dataSeedFactory = _apiFactory.Services.GetRequiredService<DataSeedFactory>();
        _dataSeedPackages = _dataSeedFactory.GetAllDataSeeds();

        _output = output;
    }

    public async Task InitializeAsync()
    {
        foreach (var package in _dataSeedPackages)
        {
            await package.SeedAsync();
        }
    }

    public async Task DisposeAsync()
    {
        foreach (var package in _dataSeedPackages)
        {
            await package.ClearAsync();
        }

        _apiFactory.MocksProvider.ResetAllMocks();
    }
    
    [Fact]
    [DisplayName("Delete Rating When Rating Exists Should Succeed")]
    public async Task DeleteRating_WhenRatingExists_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(DeleteSingleRatingUserId);
        
        var dataSeedPackage = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var ratingToDelete = dataSeedPackage.GetAllDataForUserId(DeleteSingleRatingUserId).First();

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", ratingToDelete.SnowboardId.ToString());
        
        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getRequest = new RestRequest(GetUserRatingsEndpoint);
        var getResponse = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(getRequest);
        
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.Data.Should().NotContain(r => r.SnowboardId == ratingToDelete.SnowboardId);
    }
    
    
    
    

    
    
    
}
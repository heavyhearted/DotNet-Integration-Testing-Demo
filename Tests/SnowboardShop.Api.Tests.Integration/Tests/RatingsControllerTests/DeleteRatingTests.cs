using System.ComponentModel;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Core.MockProviders;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData;
using SnowboardShop.Api.Tests.Integration.Tests.TestCollections;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData.
    DeleteRatingConstants;

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

    [Fact]
    [DisplayName("Delete Single Rating From List Should Succeed")]
    public async Task DeleteSingleRating_FromList_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(DeleteSingleRatingFromListUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var ratingToDelete = dataSeedPackage.GetAllDataForUserId(DeleteSingleRatingFromListUserId).First();

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

    [Fact]
    [DisplayName("Delete Rating When Snowboard Id Does Not Exist Should Fail")]
    public async Task DeleteRating_WhenSnowboardIdDoesNotExist_ShouldFail()
    {
        _apiFactory.MocksProvider.SetupUserContextService(MissingSnowboardId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", Guid.NewGuid().ToString());

        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [DisplayName("Delete Rating When User Does Not Have Rating Should Fail")]
    public async Task DeleteRating_WhenUserDoesNotHaveRating_ShouldFail()
    {
        _apiFactory.MocksProvider.SetupUserContextService(MissingRatingUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", Guid.NewGuid().ToString());

        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [DisplayName("Delete Rating When Snowboard Has No Rating Should Return Not Found")]
    public async Task DeleteRating_WhenSnowboardHasNoRating_ShouldReturnNotFound()
    {
        _apiFactory.MocksProvider.SetupUserContextService(MissingRatingUserId);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        
        var deleteRatingSeedData = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var snowboardId = await deleteRatingSeedData.SeedSnowboardWithoutRatingAsync(MissingRatingUserId);
        
        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", snowboardId.ToString());
        
        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound); 
    }
    
    [Fact]
    [DisplayName("Delete Rating Without Authentication Should Return Unauthorized")]
    public async Task DeleteRating_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        _apiFactory.MocksProvider.SetupUserContextService(DeleteSingleRatingUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var ratingToDelete = dataSeedPackage.GetAllDataForUserId(DeleteSingleRatingUserId).First();

        var restClient = _apiFactory.CreateRestClient(_output);

        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", ratingToDelete.SnowboardId.ToString());

        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [DisplayName("Delete Rating With Invalid Authentication Should Return Unauthorized")]
    public async Task DeleteRating_WithInvalidAuthentication_ShouldReturnUnauthorized()
    {
        _apiFactory.MocksProvider.SetupUserContextService(DeleteSingleRatingUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var ratingToDelete = dataSeedPackage.GetAllDataForUserId(DeleteSingleRatingUserId).First();

        var restClient = _apiFactory.CreateRestClient("invalid_token", _output);

        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", ratingToDelete.SnowboardId.ToString());

        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    [DisplayName("Delete Rating As Regular User When Rating Exists Should Succeed")]
    public async Task DeleteRating_AsRegularUser_WhenRatingExists_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(DeleteSingleRatingUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var ratingToDelete = dataSeedPackage.GetAllDataForUserId(DeleteSingleRatingUserId).First();

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.RegularUser, _output);

        var deleteRequest = new RestRequest(DeleteRatingEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", ratingToDelete.SnowboardId.ToString());

        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getRequest = new RestRequest(GetUserRatingsEndpoint);
        var getResponse = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(getRequest);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.Data.Should().NotContain(r => r.SnowboardId == ratingToDelete.SnowboardId);
    }
    
    [Fact]
    [DisplayName("Delete Rating As Trusted Member When Rating Exists Should Succeed")]
    public async Task DeleteRating_AsTrustedMember_WhenRatingExists_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(DeleteSingleRatingUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<DeleteRatingSeedData>(nameof(DeleteRatingSeedData));
        var ratingToDelete = dataSeedPackage.GetAllDataForUserId(DeleteSingleRatingUserId).First();

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);

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
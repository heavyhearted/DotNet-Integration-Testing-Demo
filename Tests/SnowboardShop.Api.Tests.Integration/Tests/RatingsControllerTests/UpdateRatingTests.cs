using System.ComponentModel;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Core.MockProviders;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.CommonSeedData;
using SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.UpdateRatingData;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.RatingsController;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;
using SnowboardShop.Api.Tests.Integration.Tests.TestCollections;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.CommonSeedData.CommonSnowboardConstants;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.UpdateRatingData.
    UpdateRatingConstants;

namespace SnowboardShop.Api.Tests.Integration.Tests.RatingsControllerTests;

[Collection(DatabaseSeedTestCollection.DatabaseSeedTestCollectionName)]
public class UpdateRatingTests : IAsyncLifetime
{
    private const string UpdateRatingEndpoint = Core.ApiEndpoints.Ratings.Rate;
    private const string GetUserRatingsEndpoint = Core.ApiEndpoints.Ratings.GetUserRatings;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory<MocksProvider> _apiFactory;
    private readonly List<IDataSeed> _dataSeedPackages;
    private readonly DataSeedFactory _dataSeedFactory;

    public UpdateRatingTests(SnowboardsApiFactory<MocksProvider> apiFactory, ITestOutputHelper output)
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


    [Theory]
    [DisplayName("Update Rating of Non-Rated Snowboard Should Succeed")]
    [ClassData(typeof(ValidRatingRangeTheoryData))]
    public async Task UpdateRating_OfNonRatedSnowboard_ShouldSucceed(RateSnowboardRequest rateRequest)
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(rateRequest);

        var updateResponse = await restClient.ExecuteAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [DisplayName("Update Rating of Previously Rated Snowboard Should Succeed")]
    [ClassData(typeof(ValidRatingRangeTheoryData))]
    public async Task UpdateRating_OfPreviouslyRatedSnowboard_ShouldSucceed(RateSnowboardRequest rateRequest)
    {
        _apiFactory.MocksProvider.SetupUserContextService(UpdateSingleRatingOfRatedSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<UpdateRatingSeedData>(nameof(UpdateRatingSeedData));
        var snowboardId = dataSeedPackage.GetAllDataForUserId(UpdateSingleRatingOfRatedSnowboardUserId).Single()
            .SnowboardId;

        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(rateRequest);

        var updateResponse = await restClient.ExecuteAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [DisplayName("Update Multiple Ratings for User One by One and Verify Updated Ratings")]
    public async Task UpdateMultipleRatings_ForUserOneByOne_ShouldVerifyUpdatedRatings()
    {
        _apiFactory.MocksProvider.SetupUserContextService(UpdateMultipleRatingsUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<UpdateRatingSeedData>(nameof(UpdateRatingSeedData));

        var ratings = dataSeedPackage.GetAllDataForUserId(UpdateMultipleRatingsUserId);

        ratings.Should().HaveCount(5).And.OnlyContain(r => r.Rating == 1);

        foreach (var rating in ratings.Take(3))
        {
            var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put)
                .AddUrlSegment("id", rating.SnowboardId.ToString())
                .AddJsonBody(new RateSnowboardRequest { Rating = 5 });

            var updateResponse = await restClient.ExecuteAsync(updateRequest);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var getRequest = new RestRequest(GetUserRatingsEndpoint);
        var getResponse = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(getRequest);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.Data.Should().NotBeNull().And.HaveCount(5);

        var expectedRatings = new List<int> { 5, 5, 5, 1, 1 };
        var actualRatings = getResponse.Data!
            .OrderByDescending(r => r.Rating)
            .Select(r => r.Rating)
            .ToList();

        actualRatings.Should().BeEquivalentTo(expectedRatings);
    }


    [Theory]
    [DisplayName("Update Rating With Invalid Range Should Return BadRequest")]
    [ClassData(typeof(InvalidRatingRangeTheoryData))]
    public async Task UpdateRating_WithInvalidRating_ShouldReturnBadRequest(RateSnowboardRequest invalidRateRequest)
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(invalidRateRequest);

        var updateResponse = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorResponse = JsonSerializer.Deserialize<ValidationFailureResponse>(updateResponse.Content!);
        errorResponse!.Errors.Should().ContainSingle(e =>
            e.PropertyName == "Rating" && e.Message == "Rating must be between 1 and 5");
    }


    [Theory]
    [DisplayName("Update Rating With Invalid Properties Should Return BadRequest")]
    [ClassData(typeof(InvalidRatingPropertyTheoryData))]
    public async Task UpdateRating_WithInvalidProperties_ShouldReturnBadRequest(string invalidJsonPayload)
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(invalidJsonPayload);

        var updateResponse = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    [DisplayName("Update Rating Without Authentication Should Return Unauthorized")]
    public async Task UpdateRating_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = _apiFactory.CreateRestClient(_output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequestBody = new RateSnowboardRequest { Rating = 3 };
        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(updateRequestBody);

        var updateResponse = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    
    [Fact]
    [DisplayName("Update Rating With Invalid Authentication Should Return Unauthorized")]
    public async Task UpdateRating_WithInvalidAuthentication_ShouldReturnUnauthorized()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = _apiFactory.CreateRestClient("invalid_token", _output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequestBody = new RateSnowboardRequest { Rating = 3 };
        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(updateRequestBody);

        var updateResponse = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    [DisplayName("Update Rating With Empty Payload Should Return BadRequest")]
    public async Task UpdateRating_WithEmptyPayload_ShouldReturnBadRequest()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(new { });

        var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    [DisplayName("Update Rating With Missing Payload Should Return BadRequest")]
    public async Task UpdateRating_WithMissingPayload_ShouldReturnBadRequest()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddHeader("Content-Type", "application/json");

        var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [DisplayName("Update Rating As Regular User Should Succeed")]
    public async Task UpdateRating_AsRegularUser_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.RegularUser, _output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequestBody = new RateSnowboardRequest { Rating = 4 };
        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(updateRequestBody);
        
        var updateResponse = await restClient.ExecuteAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    
    [Fact]
    [DisplayName("Update Rating As Trusted Member Should Succeed")]
    public async Task UpdateRating_AsTrustedMember_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidCommonSnowboardUserId);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);

        var snowboardSeedData = _dataSeedFactory.GetDataSeed<CommonSnowboardSeedData>(nameof(CommonSnowboardSeedData));
        var snowboardId = snowboardSeedData.GetAllDataForUserId(ValidCommonSnowboardUserId).Single().Id;

        var updateRequestBody = new RateSnowboardRequest { Rating = 4 };
        var updateRequest = new RestRequest(UpdateRatingEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", snowboardId.ToString());
        updateRequest.AddJsonBody(updateRequestBody);
        
        var updateResponse = await restClient.ExecuteAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    
    [Theory]
    [DisplayName("Update Rating With Non-Existent Snowboard ID Should Return NotFound")]
    [ClassData(typeof(NonExistingSnowboardGuidTheoryData))]
    public async Task UpdateRating_WithNonExistentSnowboardId_ShouldReturnNotFound(string nonExistentSnowboardId)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var rateRequest = new RateSnowboardRequest { Rating = 1 };
        var request = new RestRequest(UpdateRatingEndpoint, Method.Put);
        request.AddUrlSegment("id", nonExistentSnowboardId);
        request.AddJsonBody(rateRequest);

        var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
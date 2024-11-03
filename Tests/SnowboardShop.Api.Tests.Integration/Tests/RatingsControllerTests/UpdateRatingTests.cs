using System.ComponentModel;
using System.Net;
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
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.CommonSeedData.CommonSnowboardConstants;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.UpdateRatingData.UpdateRatingConstants;

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


    // [Theory]
    // [DisplayName("Update Rating Should Succeed With Valid Rating")]
    // [ClassData(typeof(ValidRatingRangeTheoryData))]
    // public async Task UpdateRating_WithValidRating_ShouldSucceed(RateSnowboardRequest rateRequest)
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(rateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //
    //     var expectedResponse = new SnowboardRatingResponse
    //     {
    //         Message = "Rating submitted successfully",
    //         SnowboardId = createdSnowboard.Id,
    //         Rating = rateRequest.Rating
    //     };
    //
    //     response.Data.Should().BeEquivalentTo(expectedResponse);
    // }


    // [Theory]
    // [DisplayName("Update Rating With Invalid Range Should Return BadRequest")]
    // [ClassData(typeof(InvalidRatingRangeTheoryData))]
    // public async Task UpdateRating_WithInvalidRating_ShouldReturnBadRequest(RateSnowboardRequest invalidRateRequest)
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(invalidRateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    // }

    // [Theory]
    // [DisplayName("Update Rating With Invalid Properties Should Return BadRequest")]
    // [ClassData(typeof(InvalidRatingPropertyTheoryData))]
    // public async Task UpdateRating_WithInvalidProperties_ShouldReturnBadRequest(string invalidJsonPayload)
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(invalidJsonPayload);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    // }

    // [Fact]
    // [DisplayName("Update Rating Without Authentication Should Return Unauthorized")]
    // public async Task UpdateRating_WithoutAuthentication_ShouldReturnUnauthorized()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = _apiFactory.CreateRestClient(_output);
    //
    //     var rateRequest = new RateSnowboardRequest { Rating = 3 };
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(rateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    // }

    // [Fact]
    // [DisplayName("Update Rating With Invalid Authentication Should Return Unauthorized")]
    // public async Task UpdateRating_WithInvalidAuthentication_ShouldReturnUnauthorized()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = _apiFactory.CreateRestClient("invalid_token", _output);
    //
    //     var rateRequest = new RateSnowboardRequest { Rating = 3 };
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(rateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    // }

    // [Fact]
    // [DisplayName("Update Rating With Empty Payload Should Return BadRequest")]
    // public async Task UpdateRating_WithEmptyPayload_ShouldReturnBadRequest()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(new { });
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    // }

    // [Fact]
    // [DisplayName("Update Rating With Missing Payload Should Return BadRequest")]
    // public async Task UpdateRating_WithMissingPayload_ShouldReturnBadRequest()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request); 
    //     response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    // }

    // [Fact]
    // [DisplayName("Update Rating As Regular User Should Succeed")]
    // public async Task UpdateRating_AsRegularUser_ShouldSucceed()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.RegularUser, _output);
    //
    //     var rateRequest = new RateSnowboardRequest { Rating = 3 };
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(rateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //     response.Data!.Rating.Should().Be(rateRequest.Rating);
    // }

    // [Fact]
    // [DisplayName("Update Rating As Trusted Member Should Succeed")]
    // public async Task UpdateRating_AsTrustedMember_ShouldSucceed()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);
    //
    //     var rateRequest = new RateSnowboardRequest { Rating = 2 };
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", createdSnowboard.Id);
    //     request.AddJsonBody(rateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //     response.Data!.Rating.Should().Be(rateRequest.Rating);
    // }

    // [Fact]
    // [DisplayName("Update Rating With Already Rated Snowboard Should Succeed")]
    // public async Task UpdateRating_WithAlreadyRatedSnowboard_ShouldSucceed()
    // {
    //     var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var initialRateRequest = new RateSnowboardRequest { Rating = 4 };
    //     var initialRequest = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     initialRequest.AddUrlSegment("id", createdSnowboard.Id);
    //     initialRequest.AddJsonBody(initialRateRequest);
    //
    //     var initialRateResponse = await restClient.ExecuteAsync<SnowboardRatingResponse>(initialRequest);
    //     initialRateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    //     initialRateResponse.Data!.Rating.Should().Be(initialRateRequest.Rating);
    //
    //     var updateRateRequest = new RateSnowboardRequest { Rating = 5 };
    //     var updateRequest = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     updateRequest.AddUrlSegment("id", createdSnowboard.Id);
    //     updateRequest.AddJsonBody(updateRateRequest);
    //
    //     var updateResponse = await restClient.ExecuteAsync<SnowboardRatingResponse>(updateRequest);
    //
    //     updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    //     updateResponse.Data!.Rating.Should().Be(updateRateRequest.Rating);
    // }

    // [Theory]
    // [DisplayName("Update Rating With Non-Existent Snowboard ID Should Return NotFound")]
    // [ClassData(typeof(NonExistingSnowboardGuidTheoryData))]
    // public async Task UpdateRating_WithNonExistentSnowboardId_ShouldReturnNotFound(string nonExistentSnowboardId)
    // {
    //     var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
    //
    //     var rateRequest = new RateSnowboardRequest { Rating = 1 };
    //     var request = new RestRequest(RateSnowboardEndpoint, Method.Put);
    //     request.AddUrlSegment("id", nonExistentSnowboardId);
    //     request.AddJsonBody(rateRequest);
    //
    //     var response = await restClient.ExecuteAsync<SnowboardRatingResponse>(request);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    // }
}
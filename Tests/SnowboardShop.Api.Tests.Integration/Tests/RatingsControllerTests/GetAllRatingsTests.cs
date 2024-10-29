using System.ComponentModel;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Core.MockProviders;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.GetAllRatingsData;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.RatingsController;
using SnowboardShop.Api.Tests.Integration.Tests.TestCollections;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Application.Models;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.GetAllRatingsData.GetAllRatingsConstants;

namespace SnowboardShop.Api.Tests.Integration.Tests.RatingsControllerTests;

[Collection(DatabaseSeedTestCollection.DatabaseSeedTestCollectionName)]
public class GetAllRatingsTests : IAsyncLifetime
{
    private const string GetAllRatingsEndpoint = Core.ApiEndpoints.Ratings.GetUserRatings;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory<MocksProvider> _apiFactory;
    private readonly List<IDataSeed> _dataSeedPackages;
    private readonly DataSeedFactory _dataSeedFactory;

    public GetAllRatingsTests(SnowboardsApiFactory<MocksProvider> apiFactory, ITestOutputHelper output)
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
    [DisplayName("Get All Ratings With Multiple Ratings Should Return All Items")]
    public async Task GetAllRatings_WithMultipleRatings_ShouldReturnAllItems()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidRatingsUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<GetAllRatingsSeedData>(nameof(GetAllRatingsSeedData));
        var expectedData = dataSeedPackage.GetAllDataForUserId(ValidRatingsUserId);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        
        var request = new RestRequest(GetAllRatingsEndpoint);
        var response = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull().And.HaveCount(expectedData.Count);

        // Verify each snowboard has the correct rating
        for (int i = 0; i < expectedData.Count; i++)
        {
            response.Data.Should().ContainSingle(r
                => r.Rating == (i + 1)); // Each snowboard has rating 1 to 5
        }
    }
    
    [Fact]
    [DisplayName("Get All Ratings Without Authentication Should Return Unauthorized")]
    public async Task GetAllRatings_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var restClient = _apiFactory.CreateRestClient(_output);
        
        var request = new RestRequest(GetAllRatingsEndpoint);
        var response = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    [DisplayName("Get All Ratings When No Ratings Exist Should Return Empty List")]
    public async Task GetAllRatings_WhenNoRatingsExist_ShouldReturnEmptyList()
    {
        _apiFactory.MocksProvider.SetupUserContextService(MissingRatingsUserId);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var request = new RestRequest(GetAllRatingsEndpoint);
        var response = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull().And.BeEmpty();
    }
    
    [Fact]
    [DisplayName("Get All Ratings As Regular User Should Succeed")]
    public async Task GetAllRatings_AsRegularUser_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidRatingsUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<GetAllRatingsSeedData>(nameof(GetAllRatingsSeedData));
        var expectedData = dataSeedPackage.GetAllDataForUserId(ValidRatingsUserId);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.RegularUser, _output);
        
        var request = new RestRequest(GetAllRatingsEndpoint);
        var response = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull().And.HaveCount(expectedData.Count);

        // Verify each snowboard has the correct rating
        for (int i = 0; i < expectedData.Count; i++)
        {
            response.Data.Should().ContainSingle(r
                => r.Rating == (i + 1)); // Each snowboard has rating 1 to 5
        }
    }
    
    [Fact]
    [DisplayName("Get All Ratings As Trusted Member Should Succeed")]
    public async Task GetAllRatings_AsTrustedMember_ShouldSucceed()
    {
        _apiFactory.MocksProvider.SetupUserContextService(ValidRatingsUserId);

        var dataSeedPackage = _dataSeedFactory.GetDataSeed<GetAllRatingsSeedData>(nameof(GetAllRatingsSeedData));
        var expectedData = dataSeedPackage.GetAllDataForUserId(ValidRatingsUserId);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);
        
        var request = new RestRequest(GetAllRatingsEndpoint);
        var response = await restClient.ExecuteAsync<List<SnowboardRatingsResponse>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull().And.HaveCount(expectedData.Count);

        // Verify each snowboard has the correct rating
        for (int i = 0; i < expectedData.Count; i++)
        {
            response.Data.Should().ContainSingle(r
                => r.Rating == (i + 1)); // Each snowboard has rating 1 to 5
        }
    }
}
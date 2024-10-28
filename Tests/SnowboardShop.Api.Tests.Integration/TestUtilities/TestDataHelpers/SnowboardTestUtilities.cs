using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

public static class SnowboardTestUtilities
{
    public static async Task<SnowboardResponse> CreateSnowboardAsync(
        SnowboardsApiFactory apiFactory,
        ITestOutputHelper output,
        HashSet<Guid> createdIds,
        UserRoles roles = UserRoles.Admin | UserRoles.TrustedMember)
    {
        var snowboardFaker = new CreateSnowboardFaker();

        var restClient = await apiFactory.CreateAuthenticatedRestClientAsync(roles, output);

        var snowboardRequest = snowboardFaker.Generate();
        var createRequest = new RestRequest(Core.ApiEndpoints.Snowboards.Create, Method.Post);
        createRequest.AddJsonBody(snowboardRequest);

        var createResponse = await restClient.ExecutePostAsync<SnowboardResponse>(createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdSnowboard = createResponse.Data!;
        createdIds.Add(createdSnowboard.Id);

        return createdSnowboard;
    }

    public static async Task CreateAndRateSnowboard(
        RestClient restClient, CreateSnowboardRequest createRequest,
        RateSnowboardRequest rateRequest, HashSet<Guid> createdIds)
    {
        var createSnowboardRequest = new RestRequest(Core.ApiEndpoints.Snowboards.Create, Method.Post);
        createSnowboardRequest.AddJsonBody(createRequest);

        var createSnowboardResponse = await restClient.ExecuteAsync<SnowboardResponse>(createSnowboardRequest);
        createSnowboardResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdSnowboard = createSnowboardResponse.Data!;
        createdIds.Add(createdSnowboard.Id);

        var rateSnowboardRequest = new RestRequest(Core.ApiEndpoints.Ratings.Rate, Method.Put);
        rateSnowboardRequest.AddUrlSegment("id", createdSnowboard.Id);
        rateSnowboardRequest.AddJsonBody(rateRequest);

        var rateSnowboardResponse = await restClient.ExecuteAsync(rateSnowboardRequest);
        rateSnowboardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
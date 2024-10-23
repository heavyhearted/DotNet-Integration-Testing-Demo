using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

public static class SnowboardTestUtilities
{
    /// <summary>
    /// Creates a snowboard using the API and returns its ID.
    /// Adds the ID to the createdIds HashSet only after successful creation.
    /// </summary>
    /// <param name="apiFactory">Factory to create REST clients.</param>
    /// <param name="output">Test output helper for logging.</param>
    /// <param name="createdIds">A collection to track created snowboard IDs for cleanup.</param>
    /// <returns>The GUID of the created snowboard.</returns>
    public static async Task<SnowboardResponse> CreateSnowboardAsync(
        SnowboardsApiFactory apiFactory,
        ITestOutputHelper output,
        HashSet<Guid> createdIds)
    {
        var snowboardFaker = new CreateSnowboardFaker();
        var restClient = await apiFactory.CreateAuthenticatedRestClientAsync(output);
        var snowboardRequest = snowboardFaker.Generate();
        var createRequest = new RestRequest(Core.ApiEndpoints.Snowboards.Create, Method.Post);
        createRequest.AddJsonBody(snowboardRequest);


        var createResponse = await restClient.ExecutePostAsync<SnowboardResponse>(createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdSnowboard = createResponse.Data!;
        createdIds.Add(createdSnowboard.Id);
        return createdSnowboard;
    }
}
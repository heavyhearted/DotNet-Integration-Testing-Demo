using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
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
}
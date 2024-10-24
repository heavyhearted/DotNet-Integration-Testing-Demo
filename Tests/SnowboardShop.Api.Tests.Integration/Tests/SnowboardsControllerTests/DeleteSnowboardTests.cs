using System.ComponentModel;
using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class DeleteSnowboardTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string DeleteSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Delete;
    private const string CreateSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Create;
    private const string GetSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Get;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly HashSet<Guid> _createdIds = new();

    public DeleteSnowboardTests(ITestOutputHelper output, SnowboardsApiFactory apiFactory)
    {
        _output = output;
        _apiFactory = apiFactory;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => Task.CompletedTask;
    
    [Fact]
    [DisplayName("Delete Snowboard Should Succeed")]
    public async Task DeleteSnowboard_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var deleteRequest = new RestRequest(DeleteSnowboardEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", createdSnowboard.Id);

        var deleteResponse = await restClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getRequest = new RestRequest(GetSnowboardEndpoint);
        getRequest.AddUrlSegment("idOrSlug", createdSnowboard.Id);
        
        var getResponse = await restClient.ExecuteGetAsync<SnowboardResponse>(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [DisplayName("Delete Snowboard Without Authentication Should Return Unauthorized")]
    public async Task DeleteSnowboardWithoutAuthentication_ShouldReturnUnauthorized()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var unauthenticatedRestClient = _apiFactory.CreateRestClient(_output);
        var deleteRequest = new RestRequest(DeleteSnowboardEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", createdSnowboard.Id);

        var deleteResponse = await unauthenticatedRestClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    [DisplayName("Delete Snowboard As Trusted Member Should Return Forbidden")]
    public async Task DeleteSnowboardAsTrustedMember_ShouldReturnForbidden()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        
        var trustedMemberRestClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);
        var deleteRequest = new RestRequest(DeleteSnowboardEndpoint, Method.Delete);
        deleteRequest.AddUrlSegment("id", createdSnowboard.Id);

        var deleteResponse = await trustedMemberRestClient.ExecuteAsync(deleteRequest);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

}
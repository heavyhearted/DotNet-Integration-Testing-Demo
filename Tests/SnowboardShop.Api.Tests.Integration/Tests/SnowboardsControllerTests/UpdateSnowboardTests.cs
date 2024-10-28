using System.ComponentModel;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;
using SnowboardShop.Api.Tests.Integration.TestUtilities.AssertionHelpers;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

[Collection(ApiFactoryTestCollection.ApiFactoryTestCollectionName)]
public class UpdateSnowboardTests : IAsyncLifetime
{
    private const string UpdateSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Update;
    private const string DeleteSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Delete;

    private readonly ITestOutputHelper _output;
    private readonly TestContainersSnowboardsApiFactory _apiFactory;
    private readonly CreateSnowboardFaker _snowboardFaker = new();
    private readonly HashSet<Guid> _createdIds = new();

    public UpdateSnowboardTests(TestContainersSnowboardsApiFactory apiFactory, ITestOutputHelper output)
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
            var request = new RestRequest(DeleteSnowboardEndpoint, Method.Delete);
            request.AddUrlSegment("id", id);

            await restClient.DeleteAsync(request);
        }
        
        _apiFactory.MocksProvider.ResetAllMocks();
    }

    [Fact]
    [DisplayName("Update Snowboard Should Succeed")]
    public async Task UpdateSnowboard_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var updateRequest = _snowboardFaker.Generate();

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var updateRequestApi = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        updateRequestApi.AddUrlSegment("id", createdSnowboard.Id);
        updateRequestApi.AddJsonBody(updateRequest);

        var updateResponse = await restClient.ExecuteAsync<SnowboardResponse>(updateRequestApi);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        SnowboardAssertions.AssertSnowboardResponseBody(updateResponse.Data, updateRequest);
    }

    [Theory]
    [DisplayName("Update Snowboard By Modifying Individual Properties Should Succeed")]
    [ClassData(typeof(UpdateSnowboardTheoryData))]
    public async Task UpdateSnowboard_ByModifyingIndividualProperties_ShouldSucceed(
        Func<CreateSnowboardRequest, UpdateSnowboardRequest> modifyRequest)
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var updateRequest = modifyRequest(new CreateSnowboardFaker().Generate());

        var updateRequestWithId = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        updateRequestWithId.AddUrlSegment("id", createdSnowboard.Id);
        updateRequestWithId.AddJsonBody(updateRequest);

        var updateResponse = await restClient.ExecuteAsync<SnowboardResponse>(updateRequestWithId);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        SnowboardAssertions.AssertSnowboardResponseBody(updateResponse.Data, updateRequest);
    }

    [Fact]
    [DisplayName("Update Snowboard With Same Body Should Succeed")]
    public async Task UpdateSnowboard_WithSameBody_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var updateRequestBody = new UpdateSnowboardRequest
        {
            SnowboardBrand = createdSnowboard.SnowboardBrand,
            YearOfRelease = createdSnowboard.YearOfRelease,
            SnowboardLineup = createdSnowboard.SnowboardLineup
        };

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var updateRequest = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", createdSnowboard.Id);
        updateRequest.AddJsonBody(updateRequestBody);

        var updateResponse = await restClient.ExecuteAsync<SnowboardResponse>(updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        SnowboardAssertions.AssertSnowboardResponseBody(updateResponse.Data, updateRequestBody);
    }

    [Fact]
    [DisplayName("Update Snowboard Without Authentication Should Return Unauthorized")]
    public async Task UpdateSnowboard_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var restClient = _apiFactory.CreateRestClient(_output);
        var updateRequestBody = new CreateSnowboardFaker().Generate();

        var updateRequest = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", createdSnowboard.Id);
        updateRequest.AddJsonBody(updateRequestBody);

        var updateResponse = await restClient.ExecuteAsync<SnowboardResponse>(updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [DisplayName("Update Snowboard With Invalid Authentication Should Return Unauthorized")]
    public async Task UpdateSnowboard_WithInvalidAuthentication_ShouldReturnUnauthorized()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);
        var restClient = _apiFactory.CreateRestClient("invalid_token", _output);
        var updateRequestBody = new CreateSnowboardFaker().Generate();

        var updateRequest = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        updateRequest.AddUrlSegment("id", createdSnowboard.Id);
        updateRequest.AddJsonBody(updateRequestBody);

        var updateResponse = await restClient.ExecuteAsync<SnowboardResponse>(updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [DisplayName("Update Snowboard With Empty Payload Should Return BadRequest")]
    public async Task UpdateSnowboard_WithEmptyPayload_ShouldReturnBadRequest()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(new { });

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [DisplayName("Update Snowboard With Missing Payload Should Return BadRequest")]
    public async Task UpdateSnowboard_WithMissingPayload_ShouldReturnBadRequest()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Theory]
    [DisplayName("Update Snowboard With Invalid Year Of Release Should Return BadRequest")]
    [ClassData(typeof(InvalidSnowboardYearOfReleaseTheoryData))]
    public async Task UpdateSnowboard_InvalidYearOfRelease_ShouldReturnBadRequest(object invalidRequest)
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(invalidRequest);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [DisplayName("Update Snowboard With Empty String Properties Should Return BadRequest")]
    [ClassData(typeof(EmptyStringSnowboardTheoryData))]
    public async Task UpdateSnowboard_WithEmptyStringProperties_ShouldReturnBadRequest(
        CreateSnowboardRequest invalidRequest)
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(invalidRequest);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [DisplayName("Update Snowboard With Null Properties Should Return BadRequest")]
    [ClassData(typeof(NullSnowboardPropertiesTheoryData))]
    public async Task UpdateSnowboard_WithNullProperties_ShouldReturnBadRequest(string jsonPayload)
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(jsonPayload);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [DisplayName("Update Snowboard With Invalid Properties Should Return BadRequest")]
    [ClassData(typeof(InvalidSnowboardTheoryData))]
    public async Task UpdateSnowboard_WithInvalidProperties_ShouldReturnBadRequest(string jsonPayload)
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(jsonPayload);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [DisplayName("Update Snowboard With SQL Injection Attempt on Properties Should Return BadRequest")]
    [ClassData(typeof(SqlInjectionTheoryData))]
    public async Task UpdateSnowboard_WithSqlInjectionAttempt_ShouldReturnBadRequest(string sqlInjectionPayload)
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(sqlInjectionPayload); 

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [DisplayName("Update Snowboard As Regular User Should Return Forbidden")]
    public async Task UpdateSnowboard_AsRegularUser_ShouldReturnForbidden()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.RegularUser, _output);
        var snowboardRequest = _snowboardFaker.Generate();
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(snowboardRequest);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [DisplayName("Update Snowboard As Trusted Member Should Succeed")]
    public async Task UpdateSnowboard_AsTrustedMember_ShouldSucceed()
    {
        var createdSnowboard = await SnowboardTestUtilities.CreateSnowboardAsync(_apiFactory, _output, _createdIds);

        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);
        var snowboardRequest = _snowboardFaker.Generate();
        var request = new RestRequest(UpdateSnowboardEndpoint, Method.Put);
        request.AddUrlSegment("id", createdSnowboard.Id);
        request.AddJsonBody(snowboardRequest);

        var response = await restClient.ExecuteAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
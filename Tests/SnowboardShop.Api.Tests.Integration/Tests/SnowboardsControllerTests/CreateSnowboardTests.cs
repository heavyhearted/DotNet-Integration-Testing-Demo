using System.ComponentModel;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardController;
using SnowboardShop.Api.Tests.Integration.TestUtilities.AssertionHelpers;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class CreateSnowboardTests : IClassFixture<SnowboardsApiFactory>, IAsyncLifetime
{
    private const string CreateSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Create;
    private const string DeleteSnowboardEndpoint = Core.ApiEndpoints.Snowboards.Delete;

    private readonly ITestOutputHelper _output;
    private readonly SnowboardsApiFactory _apiFactory;
    private readonly CreateSnowboardFaker _snowboardFaker = new();
    private readonly HashSet<Guid> _createdIds = new();

    public CreateSnowboardTests(SnowboardsApiFactory apiFactory, ITestOutputHelper output)
    {
        _apiFactory = apiFactory;
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
    }

    [Theory]
    [DisplayName("Create Snowboard With Valid Body Should Succeed")]
    [ClassData(typeof(CreateSnowboardTheoryData))]
    public async Task CreateSnowboard_ValidBody_ShouldSucceed(CreateSnowboardRequest snowboardRequestBody)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(snowboardRequestBody);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        SnowboardAssertions.AssertCreateSnowboardResponseBody(response.Data, snowboardRequestBody);

        _createdIds.Add(response.Data!.Id);
    }

    [Fact]
    [DisplayName("Create Snowboard Without Authentication Should Return Unauthorized")]
    public async Task CreateSnowboard_MissingAuthentication_ShouldReturnUnauthorized()
    {
        var restClient = _apiFactory.CreateRestClient(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(_snowboardFaker.Generate());

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    [DisplayName("Create Snowboard With Invalid Authentication Should Return Unauthorized")]
    public async Task CreateSnowboard_InvalidAuthentication_ShouldReturnUnauthorized()
    {
        var restClient = _apiFactory.CreateRestClient("invalid_token");
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(_snowboardFaker.Generate());

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    [DisplayName("Create Duplicate Snowboard Attempt Should Return BadRequest")]
    public async Task CreateDuplicateSnowboardAttempt_SecondRequestSubmission_ShouldReturnBadRequest()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var snowboardRequest = _snowboardFaker.Generate();
        var firstRequest = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        firstRequest.AddJsonBody(snowboardRequest);

        var firstResponse = await restClient.ExecutePostAsync<SnowboardResponse>(firstRequest);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);


        var secondRequest = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        secondRequest.AddJsonBody(snowboardRequest);

        var secondResponse = await restClient.ExecutePostAsync<SnowboardResponse>(secondRequest);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = JsonSerializer.Deserialize<ValidationFailureResponse>(secondResponse.Content!);
        errorResponse.Should().NotBeNull();
        errorResponse!.Errors.Should().Contain(e => e.PropertyName == "Slug"
                                                    && e.Message ==
                                                    "This Snowboard Collection already exists in the system.");
    }


    [Theory]
    [DisplayName("Create Snowboard With Missing Required Fields Should Return BadRequest")]
    [ClassData(typeof(MissingSnowboardPropertiesTheoryData))]
    public async Task CreateSnowboard_MissingRequiredFields_ShouldReturnBadRequest(string jsonPayload)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddStringBody(jsonPayload, DataFormat.Json);

        var response = await restClient.ExecutePostAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [DisplayName("Create Snowboard With Empty Payload Should Return BadRequest")]
    public async Task CreateSnowboard_EmptyPayload_ShouldReturnBadRequest()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(new { });

        var response = await restClient.ExecutePostAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [DisplayName("Create Snowboard With Invalid Year Of Release Should Return BadRequest")]
    [ClassData(typeof(InvalidSnowboardYearOfReleaseTheoryData))]
    public async Task CreateSnowboard_InvalidYearOfRelease_ShouldReturnBadRequest(CreateSnowboardRequest invalidRequest)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(invalidRequest);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [DisplayName("Create Snowboard With Empty String Properties Should Return BadRequest")]
    [ClassData(typeof(EmptyStringSnowboardTheoryData))]
    public async Task CreateSnowboard_WithEmptyStringProperties_ShouldReturnBadRequest(CreateSnowboardRequest invalidRequest)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(invalidRequest);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [DisplayName("Create Snowboard With Null Properties Should Return BadRequest")]
    [ClassData(typeof(NullSnowboardPropertiesTheoryData))]
    public async Task CreateSnowboard_WithNullProperties_ShouldReturnBadRequest(string jsonPayload)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(jsonPayload);
        
        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [DisplayName("Create Snowboard With Invalid Properties Should Return BadRequest")]
    [ClassData(typeof(InvalidSnowboardTheoryData))]
    public async Task CreateSnowboard_WithInvalidProperties_ShouldReturnBadRequest(string jsonPayload)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(jsonPayload);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [DisplayName("Create Snowboard As Regular User Should Return Forbidden")]
    public async Task CreateSnowboard_AsRegularUser_ShouldReturnForbidden()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.RegularUser, _output);
        var snowboardRequest = _snowboardFaker.Generate();
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(snowboardRequest);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    [DisplayName("Create Snowboard As Trusted Member Should Succeed")]
    public async Task CreateSnowboard_AsTrustedMember_ShouldSucceed()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(UserRoles.TrustedMember, _output);
        var snowboardRequest = _snowboardFaker.Generate();
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(snowboardRequest);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        SnowboardAssertions.AssertCreateSnowboardResponseBody(response.Data, snowboardRequest);
        
        _createdIds.Add(response.Data!.Id);
    }
}
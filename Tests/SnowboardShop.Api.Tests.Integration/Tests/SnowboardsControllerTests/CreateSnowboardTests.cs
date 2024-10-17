using System.ComponentModel;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;
using SnowboardShop.Api.Tests.Integration.TestData;
using SnowboardShop.Api.Tests.Integration.TestUtilities.AssertionHelpers;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;
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

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);

        foreach (var id in _createdIds)
        {
            var request = new RestRequest(DeleteSnowboardEndpoint, Method.Delete);
            request.AddUrlSegment("id", id);

            var response = await restClient.DeleteAsync(request);

            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                _output.WriteLine($"Failed to delete snowboard with ID: {id}. Status: {response.StatusCode}");
            }
            else
            {
                _output.WriteLine($"Successfully deleted snowboard with ID: {id}");
            }
        }
    }

    [Theory]
    [ClassData(typeof(SnowboardsTestTheories))]
    public async Task CreateSnowboard_ShouldSucceed(CreateSnowboardRequest snowboardRequest)
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(snowboardRequest);

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        SnowboardAssertions.AssertCreateSnowboardResponseBody(response.Data, snowboardRequest);

        _createdIds.Add(response.Data!.Id);
    }
    
    [Fact]
    public async Task CreateSnowboard_MissingAuthentication_ShouldReturnUnauthorized()
    {
        var restClient = _apiFactory.CreateRestClient(_output);
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(_snowboardFaker.Generate());

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    

    [Fact]
    public async Task CreateSnowboard_InvalidAuthentication_ShouldReturnUnauthorized()
    {
        var restClient = _apiFactory.CreateRestClient("invalid_token");
        var request = new RestRequest(CreateSnowboardEndpoint, Method.Post);
        request.AddJsonBody(_snowboardFaker.Generate());

        var response = await restClient.ExecutePostAsync<SnowboardResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    [DisplayName("Create Duplicate Snowboard Attempt Should Fail On Second Request Submission")]
    public async Task CreateDuplicateSnowboardAttempt_ShouldFail_OnSecondRequestSubmission()
    {
        var restClient = await _apiFactory.CreateAuthenticatedRestClientAsync(_output);
        var faker = new CreateSnowboardFaker();
        var snowboardRequest = faker.Generate();

        
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
                      && e.Message == "This Snowboard Collection already exists in the system.");
    }
}
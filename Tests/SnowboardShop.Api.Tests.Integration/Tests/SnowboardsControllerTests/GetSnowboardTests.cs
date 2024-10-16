using System.Net;
using FluentAssertions;
using SnowboardShop.Api.Tests.Integration.TestData.SnowboardsControllerTestData;

namespace SnowboardShop.Api.Tests.Integration.Tests.SnowboardsControllerTests;

public class GetSnowboardTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7001/api/")
    };
    
    public GetSnowboardTests()
    {

    }

    public async Task InitializeAsync()
    {
        await Task.Delay(500);
    }

    
    [Theory]
    [ClassData(typeof(InvalidSnowboardGuidTheoryData))]
    public async Task Get_ReturnsNotFound_WhenSnowboardDoesNotExist(string guidAsText)
    {
        var snowboardId = Guid.Parse(guidAsText);

        var response = await _httpClient.GetAsync($"snowboards/{snowboardId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }



    public async Task DisposeAsync()
    {
        await Task.Delay(500);
    }
}
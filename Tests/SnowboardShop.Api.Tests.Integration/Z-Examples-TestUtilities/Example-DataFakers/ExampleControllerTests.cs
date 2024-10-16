using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SnowboardShop.Api.Tests.Integration.TestData.CommonTestData;
using SnowboardShop.Api.Tests.Integration.TestUtilities.DataFakers;

namespace SnowboardShop.Api.Tests.Integration.SnowboardsControllerTests;

public class ExampleControllerTests
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7001")
    };

    private readonly ExampleSnowboardDTO_DeleteMe _currentExampleSnowboard;

    //TODO: Add tests
    public ExampleControllerTests()
    {
        var snowboardFaker = new ExampleSnowboardFaker();
        _currentExampleSnowboard = snowboardFaker.Generate();
    }
    
    [Theory]
    [ClassData(typeof(InvalidNameTestTheories))]
    public async Task InvalidSnowboardName_ShouldFail(string snowboardName)
    {
        // Arrange
        _currentExampleSnowboard.Name = snowboardName;
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("snowboards", _currentExampleSnowboard);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
    
    [Theory]
    [ClassData(typeof(InvalidNameTestTheories))]
    public async Task InvalidSnowboardBrand_ShouldFail(string snowboardName)
    {
        // Arrange
        _currentExampleSnowboard.Brand = snowboardName;
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("snowboards", _currentExampleSnowboard);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
}
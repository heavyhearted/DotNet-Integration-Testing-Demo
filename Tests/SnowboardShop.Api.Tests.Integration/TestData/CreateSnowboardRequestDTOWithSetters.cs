namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;

/// <summary>
/// This class is a test-specific version of the CreateSnowboardRequest model.
/// It uses mutable properties with setters to facilitate easy modification in test scenarios.
/// The original CreateSnowboardRequest uses 'init' properties to enforce immutability, 
/// but this version allows dynamic changes during tests.
/// </summary>
public class TestCreateSnowboardRequest
{
    public string SnowboardBrand { get; set; }
    public int YearOfRelease { get; set; }
    public IEnumerable<string> SnowboardLineup { get; set; } = Enumerable.Empty<string>();
}

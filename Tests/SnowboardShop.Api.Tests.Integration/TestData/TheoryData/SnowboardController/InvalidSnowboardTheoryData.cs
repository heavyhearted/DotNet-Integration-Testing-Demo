using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardController;

public class InvalidSnowboardTheoryData : TheoryData<string>
{
    public InvalidSnowboardTheoryData()
    {
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData",
            "JsonData", "invalid_snowboard_properties.json");

        foreach (var jsonTestCase in JsonDataHelper.LoadJsonTestCases(jsonFilePath))
        {
            Add(jsonTestCase);
        }
    }
}
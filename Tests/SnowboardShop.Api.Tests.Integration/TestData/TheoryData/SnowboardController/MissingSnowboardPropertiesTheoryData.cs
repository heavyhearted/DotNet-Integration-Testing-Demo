using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardController;

public class MissingSnowboardPropertiesTheoryData : TheoryData<string>
{
    public MissingSnowboardPropertiesTheoryData()
    {
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData",
            "JsonData", "missing_snowboard_properties.json");

        foreach (var jsonTestCase in JsonDataHelper.LoadJsonTestCases(jsonFilePath))
        {
            Add(jsonTestCase);
        }
    }
}
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;

public class NullSnowboardPropertiesTheoryData : TheoryData<string>
{
    public NullSnowboardPropertiesTheoryData()
    {
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData",
            "JsonData", "null_snowboard_properties.json");

        foreach (var jsonTestCase in JsonDataHelper.LoadJsonTestCases(jsonFilePath))
        {
            Add(jsonTestCase);
        }
    }
}
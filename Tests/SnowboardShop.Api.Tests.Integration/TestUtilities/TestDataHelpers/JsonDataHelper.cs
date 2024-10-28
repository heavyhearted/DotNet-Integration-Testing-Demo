using System.Text.Json;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

public static class JsonDataHelper
{
    public static IEnumerable<string> LoadJsonTestCases(string filePath)
    {
        var jsonData = File.ReadAllText(filePath);

        var testCases = JsonSerializer.Deserialize<object[]>(jsonData);

        foreach (var testCase in testCases!)
        {
            yield return JsonSerializer.Serialize(testCase, new JsonSerializerOptions
            {
                WriteIndented = true 
            });
        }
    }
}

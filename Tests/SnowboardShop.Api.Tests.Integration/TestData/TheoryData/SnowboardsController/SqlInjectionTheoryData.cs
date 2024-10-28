namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;

public class SqlInjectionTheoryData : TheoryData<string>
{
    public SqlInjectionTheoryData()
    {
        var sqlInjectionString = "'; DROP TABLE Snowboards;--";
        
        Add($@"
        {{
            ""SnowboardBrand"": ""Burton"",
            ""YearOfRelease"": ""{sqlInjectionString}"",
            ""SnowboardLineup"": [""Custom""]
        }}");

        
        Add($@"
        {{
            ""SnowboardBrand"": ""{sqlInjectionString}"",
            ""YearOfRelease"": ""{sqlInjectionString}"",
            ""SnowboardLineup"": [""{sqlInjectionString}""]
        }}");
    }
}
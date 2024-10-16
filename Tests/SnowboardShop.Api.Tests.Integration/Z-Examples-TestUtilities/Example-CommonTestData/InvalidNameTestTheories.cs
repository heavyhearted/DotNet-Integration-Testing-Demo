namespace SnowboardShop.Api.Tests.Integration.TestData.CommonTestData;

public class InvalidNameTestTheories : TheoryData<string>
{
    public InvalidNameTestTheories()
    {
        Add(null);
        Add("");
        Add(")");
    }
}
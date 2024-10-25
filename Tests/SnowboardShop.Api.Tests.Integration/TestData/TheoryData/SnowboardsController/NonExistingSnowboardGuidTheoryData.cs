namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;

public class NonExistingSnowboardGuidTheoryData : TheoryData<string>
{
    private const int NumberOfGuidsToGenerate = 3;

    public NonExistingSnowboardGuidTheoryData()
    {
        for (int i = 0; i < NumberOfGuidsToGenerate; i++)
        {
            Add(Guid.NewGuid().ToString());
        }
    }
}
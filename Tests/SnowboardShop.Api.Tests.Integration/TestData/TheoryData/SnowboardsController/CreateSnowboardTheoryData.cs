using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController
{
    public class CreateSnowboardTheoryData : TheoryData<CreateSnowboardRequest>
    {
        private readonly CreateSnowboardFaker _snowboardFaker = new();

        public CreateSnowboardTheoryData()
        {
            for (int i = 0; i < 5; i++) // Generate 5 unique snowboard requests
            {
                Add(_snowboardFaker.Generate());
            }
        }
    }
}
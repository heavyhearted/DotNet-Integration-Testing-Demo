using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.RatingsController;

public class ValidRatingRangeTheoryData : TheoryData<RateSnowboardRequest>
{
    public ValidRatingRangeTheoryData()
    {
        // Adding valid ratings between 1 to 5
        for (int rating = 1; rating <= 5; rating++)
        {
            Add(new RateSnowboardRequest { Rating = rating });
        }
    }
}
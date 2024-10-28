using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.RatingsController;

public class InvalidRatingRangeTheoryData : TheoryData<RateSnowboardRequest>
{
    public InvalidRatingRangeTheoryData()
    {
        // Values outside the valid rating range of 1 to 5 
        Add(new RateSnowboardRequest { Rating = 0 }); // Boundary case: zero is below minimum valid rating
        Add(new RateSnowboardRequest { Rating = 6 }); // Boundary case: six is above maximum valid rating
        Add(new RateSnowboardRequest { Rating = -1 }); // Negative value: not allowed
        Add(new RateSnowboardRequest { Rating = 10 }); // large positive value beyond valid range
        Add(new RateSnowboardRequest { Rating = 100 }); // very large value
    }
}
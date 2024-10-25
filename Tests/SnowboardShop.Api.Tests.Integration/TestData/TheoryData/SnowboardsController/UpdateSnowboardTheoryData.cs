using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;

public class UpdateSnowboardTheoryData : TheoryData<Func<CreateSnowboardRequest, UpdateSnowboardRequest>>
{
    public UpdateSnowboardTheoryData()
    {
        Add(originalRequest => new UpdateSnowboardRequest
        {
            SnowboardBrand = "New Updated Brand", 
            YearOfRelease = originalRequest.YearOfRelease,
            SnowboardLineup = originalRequest.SnowboardLineup
        });
        
        Add(originalRequest => new UpdateSnowboardRequest
        {
            SnowboardBrand = originalRequest.SnowboardBrand,
            YearOfRelease = 1999,
            SnowboardLineup = originalRequest.SnowboardLineup
        });
        
        Add(originalRequest => new UpdateSnowboardRequest
        {
            SnowboardBrand = originalRequest.SnowboardBrand,
            YearOfRelease = originalRequest.YearOfRelease,
            SnowboardLineup = new[]
            {
                "First Item Updated Lineup", 
                "Second Item Updated Lineup"
            } 
        });
    }
}

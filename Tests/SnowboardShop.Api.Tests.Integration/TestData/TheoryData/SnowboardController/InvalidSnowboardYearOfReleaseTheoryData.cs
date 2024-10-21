using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardController;

public class InvalidSnowboardYearOfReleaseTheoryData : TheoryData<CreateSnowboardRequest>
{
    public InvalidSnowboardYearOfReleaseTheoryData()
    {
        var snowboardFaker = new CreateSnowboardFaker();
        var currentYear = DateTime.UtcNow.Year;

        CreateSnowboardRequest CreateRequest(int yearOfRelease)
        {
            var request = snowboardFaker.Generate();

            return new CreateSnowboardRequest
            {
                SnowboardBrand = request.SnowboardBrand,
                YearOfRelease = yearOfRelease,
                SnowboardLineup = request.SnowboardLineup
            };
        }

        Add(CreateRequest(1964)); // Invalid: before 1965
        Add(CreateRequest(currentYear + 2)); // Invalid: beyond current year + 1
        Add(CreateRequest(1950)); // Invalid: far past year
        Add(CreateRequest(2050)); // Invalid: far future year
        Add(CreateRequest(-1998)); // Invalid: negative year
    }
}
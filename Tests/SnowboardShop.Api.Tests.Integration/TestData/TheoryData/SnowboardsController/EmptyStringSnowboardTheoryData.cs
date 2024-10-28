using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestData.TheoryData.SnowboardsController;

public class EmptyStringSnowboardTheoryData : TheoryData<CreateSnowboardRequest>
{
    private readonly CreateSnowboardFaker _snowboardFaker;

    public EmptyStringSnowboardTheoryData()
    {
        _snowboardFaker = new CreateSnowboardFaker(); 
        var currentYear = DateTime.UtcNow.Year;

        CreateSnowboardRequest CreateRequest(string? snowboardBrand = null, int? yearOfRelease = null,
            List<string>? snowboardLineup = null)
        {
            var request = _snowboardFaker.Generate();

            return new CreateSnowboardRequest
            {
                SnowboardBrand = snowboardBrand ?? request.SnowboardBrand, 
                YearOfRelease = yearOfRelease ?? request.YearOfRelease, 
                SnowboardLineup = snowboardLineup ?? request.SnowboardLineup.ToList() 
            };
        }
            
        var validSnowboardBrand = _snowboardFaker.Generate().SnowboardBrand;
        var validSnowboardLineup = _snowboardFaker.Generate().SnowboardLineup; 
            
        Add(CreateRequest("", currentYear, validSnowboardLineup.ToList()));
        Add(CreateRequest("", currentYear, snowboardLineup: new List<string> { "" }));
        Add(CreateRequest("", currentYear, new List<string>()));
        Add(CreateRequest(validSnowboardBrand, currentYear, new List<string>()));
        Add(CreateRequest(validSnowboardBrand, currentYear, new List<string> { "" }));
    }
}
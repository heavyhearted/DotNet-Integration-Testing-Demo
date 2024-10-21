using Bogus;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;

public class CreateSnowboardFaker : Faker<CreateSnowboardRequest>
{
    private readonly Faker<CreateSnowboardRequest> _snowboardRequestGenerator;
    
    private readonly HashSet<(string Brand, int Year)> _existingPairs = new();

    public CreateSnowboardFaker()
    {
        _snowboardRequestGenerator = new Faker<CreateSnowboardRequest>()
            .CustomInstantiator(f =>
            {
                string brand;
                int year;

                // Ensure unique SnowboardBrand and YearOfRelease pairs
                do
                {
                    // Generate a new SnowboardBrand and YearOfRelease
                    brand = f.PickRandom(SnowboardGenerationConstants.ValidSnowboardBrands);
                    year = f.Random.Int(1965, DateTime.Now.Year + 1);
                } 
                // Continue generating until a unique pair is found
                while (_existingPairs.Contains((brand, year)));

                // Add the new unique pair to the HashSet
                _existingPairs.Add((brand, year));

                // Return the generated snowboard request
                return new CreateSnowboardRequest
                {
                    SnowboardBrand = brand,
                    YearOfRelease = year,
                    SnowboardLineup = f.Make(f.Random.Int(1, 5), () =>
                        f.PickRandom(SnowboardGenerationConstants.SnowboardLineupList)).Distinct()
                };
            });
    }

    public CreateSnowboardRequest Generate() => _snowboardRequestGenerator.Generate();
}

public static class SnowboardGenerationConstants
{
    public static readonly string[] ValidSnowboardBrands =
    {
        "Burton", "Never Summer", "CAPiTA", "GNU", "Salomon", "Lib Tech", "Roxy", "Bataleon", "Nitro", "Rossignol",
        "Nidecker", "K2", "Whitespace", "Jones", "Rome", "Flow"
    };

    public static readonly string[] SnowboardLineupList =
    {
        "Custom", "Process", "Deep Thinker", "Family Tree Hometown Hero", "Kilroy Twin", "Flight Attendant",
        "Free Thinker", "Name Dropper", "Proto Slinger", "West Bound", "Harpoon", "Maverix", "Swift",
        "Shaper Twin", "Big Gun", "Defenders of Awesome", "Super DOA",
        "Mercury", "Paradise", "Birds of a Feather", "The Outsiders", "Riders Choice", "Gremlin",
        "Head Space", "Mullair", "Money", "Ravish", "Barrett", "Assassin", "Huck Knife", "Dancehaul",
        "Super 8", "Craft", "Pulse", "Sleepwalker", "T.Rice Pro", "Skate Banana", "Orca", "Cold Brew",
        "Box Scratcher", "Lost Retro Ripper", "Dynamiss", "Shredder", "Charger", "Ripper", "Powder Slayer"
    };
}
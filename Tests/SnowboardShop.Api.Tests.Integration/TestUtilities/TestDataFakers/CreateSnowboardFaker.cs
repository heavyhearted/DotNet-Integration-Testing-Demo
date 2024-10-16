using Bogus;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataFakers;

public class CreateSnowboardFaker : Faker<CreateSnowboardRequest>
{
    private readonly Faker<CreateSnowboardRequest> _snowboardRequestGenerator = new Faker<CreateSnowboardRequest>()
        .RuleFor(x => x.SnowboardBrand, f => f.PickRandom(ValidSnowboardBrands))
        .RuleFor(x => x.YearOfRelease, f => f.Random.Int(1965, DateTime.Now.Year + 1)) 
        .RuleFor(x => x.SnowboardLineup, f 
            => f.Make(f.Random.Int(1, 5), ()
            => f.PickRandom(SnowboardLineupList) + " " +
               f.PickRandom("Edition", "Series", "Shredder", "Charger", "Ripper", "Powder Slayer")));

    public CreateSnowboardRequest Generate() => _snowboardRequestGenerator.Generate();

    private static readonly string[] ValidSnowboardBrands =
    {
        "Burton", "Never Summer", "CAPiTA", "GNU", "Salomon", "Lib Tech", "Roxy", "Bataleon", "Nitro", "Rossignol",
        "Nidecker", "K2", "Whitespace", "Jones", "Rome", "Flow"
    };

    private static readonly string[] SnowboardLineupList =
    {
        "Custom", "Process", "Deep Thinker", "Family Tree Hometown Hero", "Kilroy Twin", "Flight Attendant",
        "Free Thinker", "Name Dropper", "Proto Slinger", "West Bound", "Harpoon", "Maverix", "Swift",
        "Shaper Twin", "Big Gun", "Defenders of Awesome", "Black Snowboard of Death", "Super DOA",
        "Mercury", "Paradise", "Birds of a Feather", "The Outsiders", "Riders Choice", "Gremlin",
        "Head Space", "Mullair", "Money", "Ravish", "Barrett", "Assassin", "Huck Knife", "Dancehaul",
        "Super 8", "Craft", "Pulse", "Sleepwalker", "T.Rice Pro", "Skate Banana", "Orca", "Cold Brew",
        "Box Scratcher", "Lost Retro Ripper", "Dynamiss"
    };
}
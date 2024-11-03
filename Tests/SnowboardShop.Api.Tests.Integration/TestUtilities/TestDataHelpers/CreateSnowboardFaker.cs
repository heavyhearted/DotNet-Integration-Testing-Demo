using Bogus;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;

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
                    brand = f.PickRandom(SnowboardGenerationConstants.ValidSnowboardBrands);
                    year = f.Random.Int(1965, DateTime.Now.Year + 1);
                } 
                // Continue generating until a unique pair is found
                while (_existingPairs.Contains((brand, year)));

                _existingPairs.Add((brand, year));

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
using Bogus;
using SnowboardShop.Api.Tests.Integration.TestUtilities;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.CommonSeedData.CommonSnowboardConstants;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.CommonSeedData;

public class CommonSnowboardSeedData : SnowboardsDataSeedBase
{
    private readonly Faker _snowboardFaker = new();

    public CommonSnowboardSeedData(ISnowboardRepository snowboardRepository)
        : base(snowboardRepository)
    {
    }

    public override string Name => nameof(CommonSnowboardSeedData);

    protected override Dictionary<Guid, List<Snowboard>> CreateData()
    {
        var snowboardsPerUser = new Dictionary<Guid, List<Snowboard>>();

        snowboardsPerUser.Add(ValidCommonSnowboardUserId, new List<Snowboard>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SnowboardBrand = _snowboardFaker.Company.CompanyName(),
                YearOfRelease = _snowboardFaker.Random.Int(1965, 2025),
                SnowboardLineup = _snowboardFaker.PickRandom(SnowboardGenerationConstants.SnowboardLineupList,
                    _snowboardFaker.Random.Int(1, 3)).ToList(),
                Rating = null,
                UserRating = null
            }
        });
        
        return snowboardsPerUser;
    }
}
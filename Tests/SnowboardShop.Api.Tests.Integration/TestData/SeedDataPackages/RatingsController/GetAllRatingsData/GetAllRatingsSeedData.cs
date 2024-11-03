using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.GetAllRatingsData.
    GetAllRatingsConstants;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.GetAllRatingsData;

public class GetAllRatingsSeedData : RatingsDataSeedBase
{
    public GetAllRatingsSeedData(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository)
        : base(ratingRepository, snowboardRepository)
    {
    }

    public override string Name => nameof(GetAllRatingsSeedData);

    protected override Dictionary<Guid, List<SnowboardRating>> CreateData()
    {
        var snowboardRatingsPerUser = new Dictionary<Guid, List<SnowboardRating>>();

        snowboardRatingsPerUser.Add(ValidRatingsUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 1 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 2 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 3 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 4 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 5 }
        });

        return snowboardRatingsPerUser;
    }
}
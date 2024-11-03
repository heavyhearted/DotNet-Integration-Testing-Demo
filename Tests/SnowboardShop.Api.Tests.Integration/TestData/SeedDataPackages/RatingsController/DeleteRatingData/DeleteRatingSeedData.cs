using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData.
    DeleteRatingConstants;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData;

public class DeleteRatingSeedData : RatingsDataSeedBase
{
    public DeleteRatingSeedData(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository)
        : base(ratingRepository, snowboardRepository)
    {
    }

    public override string Name => nameof(DeleteRatingSeedData);

    protected override Dictionary<Guid, List<SnowboardRating>> CreateData()
    {
        var snowboardRatingsPerUser = new Dictionary<Guid, List<SnowboardRating>>();

        snowboardRatingsPerUser.Add(DeleteSingleRatingUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 1 }
        });


        snowboardRatingsPerUser.Add(DeleteSingleRatingFromListUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 1 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 2 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 3 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 4 },
            new() { SnowboardId = Guid.NewGuid(), Slug = AutoGen, Rating = 5 }
        });


        snowboardRatingsPerUser.Add(MissingRatingUserId, new List<SnowboardRating>());
        
        snowboardRatingsPerUser.Add(DeleteMultipleRatingsUserId, new List<SnowboardRating>
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
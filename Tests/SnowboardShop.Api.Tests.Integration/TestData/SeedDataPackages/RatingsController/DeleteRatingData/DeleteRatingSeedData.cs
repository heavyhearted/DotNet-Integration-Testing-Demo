using Bogus;
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
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2003", Rating = 1 }
        });


        snowboardRatingsPerUser.Add(DeleteSingleRatingFromListUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2003", Rating = 1 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2004", Rating = 2 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2005", Rating = 3 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2006", Rating = 4 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2007", Rating = 5 }
        });


        snowboardRatingsPerUser.Add(MissingRatingUserId, new List<SnowboardRating>());


        return snowboardRatingsPerUser;
    }
}
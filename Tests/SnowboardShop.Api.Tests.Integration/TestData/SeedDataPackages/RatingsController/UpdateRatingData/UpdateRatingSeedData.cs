using Bogus;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.UpdateRatingData.UpdateRatingConstants;


namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.UpdateRatingData;

public class UpdateRatingSeedData : RatingsDataSeedBase
{
    public UpdateRatingSeedData(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository) 
        : base(ratingRepository, snowboardRepository)
    {
    }

    public override string Name => nameof(UpdateRatingSeedData);
    protected override Dictionary<Guid, List<SnowboardRating>> CreateData()
    {
        var snowboardRatingsPerUser = new Dictionary<Guid, List<SnowboardRating>>();

        snowboardRatingsPerUser.Add(UpdateSingleRatingOfRatedSnowboardUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2003", Rating = 1 }
        });

        snowboardRatingsPerUser.Add(UpdateSingleRatingFromListUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2003", Rating = 1 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2004", Rating = 2 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2005", Rating = 3 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2006", Rating = 4 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2007", Rating = 5 }
        });
        
        snowboardRatingsPerUser.Add(UpdateMultipleRatingsUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2021", Rating = 1 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2020", Rating = 2 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2022", Rating = 3 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2024", Rating = 4 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2023", Rating = 5 }
        });

        return snowboardRatingsPerUser;
    }
}

using Bogus;
using SnowboardShop.Api.Tests.Integration.TestUtilities;
using SnowboardShop.Api.Tests.Integration.TestUtilities.TestDataHelpers;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData.DeleteRatingConstants;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.DeleteRatingData;

public class DeleteRatingSeedData : RatingsDataSeedBase
{
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly Faker _faker = new();
    private readonly List<Guid> _snowboardIdsToClear = new();
    public DeleteRatingSeedData(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository) 
        : base(ratingRepository, snowboardRepository)
    {
        _snowboardRepository = snowboardRepository;
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
        
        return snowboardRatingsPerUser;
    }
    
    public virtual async Task<Guid> SeedSnowboardWithoutRatingAsync(Guid userId)
    {
        var snowboard = new Snowboard
        {
            Id = Guid.NewGuid(),
            SnowboardBrand = _faker.Company.CompanyName(),
            YearOfRelease = _faker.Random.Int(1965, 2025),
            SnowboardLineup = _faker.PickRandom(SnowboardGenerationConstants.SnowboardLineupList, _faker.Random.Int(1, 3)).ToList(),
            Rating = null,
            UserRating = null
            // Rating is defaultly set to null by the API
        };

        await _snowboardRepository.CreateAsync(snowboard);
        _snowboardIdsToClear.Add(snowboard.Id);
        
        return snowboard.Id; 
    }
}
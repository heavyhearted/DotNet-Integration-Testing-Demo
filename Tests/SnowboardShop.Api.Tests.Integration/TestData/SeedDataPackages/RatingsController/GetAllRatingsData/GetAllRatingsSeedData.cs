using Bogus;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;
using static SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.GetAllRatingsData.GetAllRatingsConstants;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages.RatingsController.GetAllRatingsData;

public class GetAllRatingsSeedData : IDataSeed<SnowboardRating>
{
    private readonly IRatingRepository _ratingRepository;
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly Dictionary<Guid, List<SnowboardRating>> _allData;
    private readonly Faker _faker = new Faker();
    private readonly List<Guid> _snowboardIdsToClear = new();

    public GetAllRatingsSeedData(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository)
    {
        _ratingRepository = ratingRepository;
        _snowboardRepository = snowboardRepository;
        _allData = CreateData();
    }
    
    public string Name => nameof(GetAllRatingsSeedData);

    private Dictionary<Guid, List<SnowboardRating>> CreateData()
    {
        var snowboardRatingsPerUser = new Dictionary<Guid, List<SnowboardRating>>();
        
        snowboardRatingsPerUser.Add(ValidRatingsUserId, new List<SnowboardRating>
        {
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2003", Rating = 1 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2004", Rating = 2 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2005", Rating = 3 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2006", Rating = 4 },
            new() { SnowboardId = Guid.NewGuid(), Slug = "slug-2007", Rating = 5 }
        });

        return snowboardRatingsPerUser;
    }

    public IReadOnlyCollection<SnowboardRating> GetAllData()
    {
        return _allData.SelectMany(sr => sr.Value).ToList();
    }
    
    public async Task SeedAsync(CancellationToken token = default)
    {
        foreach (var snowboardRatingPair in _allData)
        {
            foreach (var rating in snowboardRatingPair.Value)
            {
                var snowboard = new Snowboard
                {
                    Id = rating.SnowboardId,
                    SnowboardBrand = _faker.Company.CompanyName(),
                    YearOfRelease = _faker.Random.Int(1965, 2025),
                    SnowboardLineup = ["Maverick LTD"]
                };
                
                await _snowboardRepository.CreateAsync(snowboard, token);
                _snowboardIdsToClear.Add(snowboard.Id);
                
                await _ratingRepository.RateSnowboardAsync(
                    rating.SnowboardId, rating.Rating, snowboardRatingPair.Key, token);
            }
        }
    }

    public async Task ClearAsync(CancellationToken token = default)
    {
        foreach (var snowboardRatingPair in _allData)
        {
            foreach (var rating in snowboardRatingPair.Value)
            {
                await _ratingRepository.DeleteRatingAsync(
                    rating.SnowboardId, snowboardRatingPair.Key, token);
            }
        }

        foreach (var guid in _snowboardIdsToClear)
        {
            await _snowboardRepository.DeleteByIdAsync(guid, token);
        }
    }
}
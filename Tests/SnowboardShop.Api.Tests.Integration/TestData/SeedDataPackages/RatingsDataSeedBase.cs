using Bogus;
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages;

public abstract class RatingsDataSeedBase : IDataSeed<SnowboardRating>
{
    private readonly IRatingRepository _ratingRepository;
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly Dictionary<Guid, List<SnowboardRating>> _allData;
    private readonly Faker _snowboardFaker = new();
    private readonly List<Guid> _snowboardIdsToClear = new();
    public const string AutoGen = null!; // Placeholder for the Slug property in SnowboardRating

    protected RatingsDataSeedBase(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository)
    {
        _ratingRepository = ratingRepository;
        _snowboardRepository = snowboardRepository;
        _allData = CreateData();
    }

    public abstract string Name { get; }

    protected abstract Dictionary<Guid, List<SnowboardRating>> CreateData();
    
    public IReadOnlyCollection<SnowboardRating> GetAllData()
    {
        return _allData.SelectMany(snowboardRating 
            => snowboardRating.Value).ToList();
    }
    
    public IReadOnlyCollection<SnowboardRating> GetAllDataForUserId(Guid userId)
    {
        return _allData[userId];
    }
    
    public virtual async Task SeedAsync()
    {
        foreach (var snowboardRatingPair in _allData)
        {
            foreach (var rating in snowboardRatingPair.Value)
            {
                var snowboard = new Snowboard
                {
                    Id = rating.SnowboardId,
                    SnowboardBrand = _snowboardFaker.Company.CompanyName(),
                    YearOfRelease = _snowboardFaker.Random.Int(1965, 2025),
                    SnowboardLineup = ["Maverick LTD"]
                };
                
                await _snowboardRepository.CreateAsync(snowboard);
                _snowboardIdsToClear.Add(snowboard.Id);
                
                await _ratingRepository.RateSnowboardAsync(
                    rating.SnowboardId, rating.Rating, snowboardRatingPair.Key);
            }
        }
    }

    public virtual async Task ClearAsync()
    {
        foreach (var snowboardRatingPair in _allData)
        {
            foreach (var rating in snowboardRatingPair.Value)
            {
                await _ratingRepository.DeleteRatingAsync(
                    rating.SnowboardId, snowboardRatingPair.Key);
            }
        }

        foreach (var guid in _snowboardIdsToClear)
        {
            await _snowboardRepository.DeleteByIdAsync(guid);
        }
    }
}
using SnowboardShop.Api.Tests.Integration.TestData.Common.Contracts;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Api.Tests.Integration.TestData.SeedDataPackages;

public abstract class SnowboardsDataSeedBase : IDataSeed<Snowboard>
{
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly Dictionary<Guid, List<Snowboard>> _allData;

    protected SnowboardsDataSeedBase(ISnowboardRepository snowboardRepository, IRatingRepository ratingRepository)
    {
        _snowboardRepository = snowboardRepository;
        _ratingRepository = ratingRepository;
        _allData = CreateData();
    }

    public abstract string Name { get; }

    protected abstract Dictionary<Guid, List<Snowboard>> CreateData();


    public IReadOnlyCollection<Snowboard> GetAllData()
    {
        return _allData.SelectMany(snowboard
            => snowboard.Value).ToList();
    }

    public IReadOnlyCollection<Snowboard> GetAllDataForUserId(Guid userId)
    {
        return _allData[userId];
    }


    public virtual async Task SeedAsync()
    {
        foreach (var snowboardPair in _allData)
        {
            foreach (var snowboard in snowboardPair.Value)
            {
                await _snowboardRepository.CreateAsync(snowboard);
            }
        }
    }


    public virtual async Task ClearAsync()
    {
        foreach (var snowboardPair in _allData)
        {
            foreach (var snowboard in snowboardPair.Value)
            {
                await _ratingRepository.DeleteRatingAsync(snowboard.Id,
                    snowboardPair.Key);
                
                await _snowboardRepository.DeleteByIdAsync(
                    snowboard.Id);
            }
        }
    }
}
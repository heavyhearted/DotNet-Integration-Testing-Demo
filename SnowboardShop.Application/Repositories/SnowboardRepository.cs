using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Repositories;

public class SnowboardRepository : ISnowboardRepository
{
    /* Temporary usage of In-Memory Database before implementing a real PostgreSQL DB later in Docker.
    Tasks are implemented to future-proof the upcoming asynchronous implementation of the real database.*/
    private readonly List<Snowboard> _snowboards = new(); 
    
    public Task<bool> CreateAsync(Snowboard snowboard)
    {
        _snowboards.Add(snowboard);
        return Task.FromResult(true);
    }

    public Task<Snowboard?> GetByIdAsync(Guid id)
    {
        var snowboard = _snowboards.SingleOrDefault(x => x.Id == id);
        return Task.FromResult(snowboard);
    }

    public Task<Snowboard?> GetBySlugAsync(string slug)
    {
        var snowboard = _snowboards.SingleOrDefault(x => x.Slug == slug);
        return Task.FromResult(snowboard);
    }
    
    public Task<IEnumerable<Snowboard>> GetAllAsync()
    {
        return Task.FromResult(_snowboards.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Snowboard snowboard)
    {
        var snowBoardIndex = _snowboards.FindIndex(x => x.Id == snowboard.Id);
        if (snowBoardIndex == -1)
        {
            return Task.FromResult(false);
        }
        
        _snowboards[snowBoardIndex] = snowboard;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        var removedCount = _snowboards.RemoveAll(x => x.Id == id);
        var snowboardRemoved = removedCount > 0;
        return Task.FromResult(snowboardRemoved);
    }
}
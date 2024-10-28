using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Services;

public interface ISnowboardService
{
    Task<bool> CreateAsync(Snowboard snowboard, CancellationToken token = default);
    
    Task<Snowboard?> GetByIdAsync(Guid id, CancellationToken token = default);
    
    Task<Snowboard?> GetBySlugAsync(string slug, CancellationToken token = default);
    
    Task<IEnumerable<Snowboard>> GetAllAsync(CancellationToken token = default);
    
    Task<Snowboard?> UpdateAsync(Snowboard snowboard, CancellationToken token = default);
    
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
}
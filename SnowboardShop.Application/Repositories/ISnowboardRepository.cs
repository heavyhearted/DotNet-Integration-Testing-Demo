using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Repositories;

public interface ISnowboardRepository
{
    Task<bool> CreateAsync(Snowboard snowboard, CancellationToken token = default);
    
    Task<Snowboard?> GetByIdAsync(Guid id, CancellationToken token = default);
    
    Task<Snowboard?> GetBySlugAsync(string slug, CancellationToken token = default);
    
    Task<IEnumerable<Snowboard>> GetAllAsync(CancellationToken token = default);
    
    Task<bool> UpdateAsync(Snowboard snowboard, CancellationToken token = default);
    
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
}
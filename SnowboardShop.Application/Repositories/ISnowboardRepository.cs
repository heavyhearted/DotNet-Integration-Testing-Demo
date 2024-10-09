using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Repositories;

public interface ISnowboardRepository
{
    Task<bool> CreateAsync(Snowboard snowboard);
    
    Task<Snowboard?> GetByIdAsync(Guid id);
    
    Task<IEnumerable<Snowboard>> GetAllAsync();
    
    Task<bool> UpdateAsync(Snowboard snowboard);
    
    Task<bool> DeleteByIdAsync(Guid id);
}
using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Services;

public interface ISnowboardService
{
    Task<bool> CreateAsync(Snowboard snowboard);
    
    Task<Snowboard?> GetByIdAsync(Guid id);
    
    Task<Snowboard?> GetBySlugAsync(string slug);
    
    Task<IEnumerable<Snowboard>> GetAllAsync();
    
    Task<Snowboard?> UpdateAsync(Snowboard movie);
    
    Task<bool> DeleteByIdAsync(Guid id);
}
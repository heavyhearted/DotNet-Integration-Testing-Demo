using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Repositories;

public interface IRatingRepository
{
    Task<bool> RateSnowboardAsync(Guid snowboardId, int rating, Guid userId, CancellationToken token = default);

    Task<float?> GetRatingAsync(Guid snowboardId, CancellationToken token = default);
    
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid snowboardId, Guid userId, CancellationToken token = default);
    
    
    Task<bool> DeleteRatingAsync(Guid snowboardId, Guid userId, CancellationToken token = default);
    
    Task<IEnumerable<SnowboardRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}
namespace SnowboardShop.Application.Repositories;

public interface IRatingRepository
{
    // Task<bool> RateSnowboardBrandAsync(Guid snowboardId, int rating, Guid userId, CancellationToken token = default);

    Task<float?> GetRatingAsync(Guid snowboardId, CancellationToken token = default);
    
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid snowboardId, Guid userId, CancellationToken token = default);
    
    //
    // Task<bool> DeleteRatingAsync(Guid snowboardId, Guid userId, CancellationToken token = default);
    //
    // Task<IEnumerable<SnowboardBrandRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}
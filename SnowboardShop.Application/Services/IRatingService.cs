using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Services;

public interface IRatingService
{
    Task<bool> RateSnowboardAsync(Guid snowboardId, int rating, Guid userId, CancellationToken token = default);

    Task<bool> DeleteRatingAsync(Guid snowboardId, Guid userId, CancellationToken token = default);

    Task<IEnumerable<SnowboardRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}
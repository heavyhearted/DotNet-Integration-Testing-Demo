using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Services;

public interface IRatingService
{
    Task<bool> RateSnowboardAsync(Guid snowboardId, int rating, CancellationToken token = default);

    Task<bool> DeleteRatingAsync(Guid snowboardId, CancellationToken token = default);

    Task<IEnumerable<SnowboardRating>> GetRatingsForUserAsync(CancellationToken token = default);
}
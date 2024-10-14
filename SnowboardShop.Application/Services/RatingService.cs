using FluentValidation;
using FluentValidation.Results;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly ISnowboardRepository _snowboardRepository;

    public RatingService(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository)
    {
        _ratingRepository = ratingRepository;
        _snowboardRepository = snowboardRepository;
    }

    public async Task<bool> RateSnowboardAsync(Guid snowboardId, int rating, Guid userId, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            });
        }

        var snowboardExists = await _snowboardRepository.ExistsByIdAsync(snowboardId, token);
        if (!snowboardExists)
        {
            return false;
        }

        return await _ratingRepository.RateSnowboardAsync(snowboardId, rating, userId, token);
    }
    
    public Task<IEnumerable<SnowboardRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, token);
    }
    
    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }
}
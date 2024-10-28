using FluentValidation;
using FluentValidation.Results;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly IUserContextService _userContextService;

    public RatingService(IRatingRepository ratingRepository, ISnowboardRepository snowboardRepository, IUserContextService userContextService)
    {
        _ratingRepository = ratingRepository;
        _snowboardRepository = snowboardRepository;
        _userContextService = userContextService;
    }

    public async Task<bool> RateSnowboardAsync(Guid snowboardId, int rating, CancellationToken token = default)
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

        return await _ratingRepository.RateSnowboardAsync(snowboardId, rating, _userContextService.UserId!.Value, token);
    }
    
    public Task<IEnumerable<SnowboardRating>> GetRatingsForUserAsync(CancellationToken token = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(_userContextService.UserId!.Value, token);
    }
    
    public Task<bool> DeleteRatingAsync(Guid movieId, CancellationToken token = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, _userContextService.UserId!.Value, token);
    }
}
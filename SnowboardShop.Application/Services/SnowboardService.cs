using FluentValidation;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application.Services;

public class SnowboardService : ISnowboardService
{
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly IValidator<Snowboard> _snowboardValidator;
    private readonly IRatingRepository _ratingRepository;
    private readonly IUserContextService _userContextService;

    public SnowboardService(ISnowboardRepository snowboardRepository, IValidator<Snowboard> snowboardValidator, IRatingRepository ratingRepository, IUserContextService userContextService)
    {
        _snowboardRepository = snowboardRepository;
        _snowboardValidator = snowboardValidator;
        _ratingRepository = ratingRepository;
        _userContextService = userContextService;
    }

    public async Task<bool> CreateAsync(Snowboard snowboard, CancellationToken token = default)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard, cancellationToken: token);
        return await _snowboardRepository.CreateAsync(snowboard, token);
    }

    public Task<Snowboard?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return _snowboardRepository.GetByIdAsync(id, _userContextService.UserId, token);
    }

    public Task<Snowboard?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return _snowboardRepository.GetBySlugAsync(slug, _userContextService.UserId, token);
    }

    public Task<IEnumerable<Snowboard>> GetAllAsync(CancellationToken token = default)
    {
        return _snowboardRepository.GetAllAsync(_userContextService.UserId, token);
    }

    public async Task<Snowboard?> UpdateAsync(Snowboard snowboard, CancellationToken token = default)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard, cancellationToken: token);
        var snowboardExists = await _snowboardRepository.ExistsByIdAsync(snowboard.Id);
        if (!snowboardExists)
        {
            return null;
        }

        await _snowboardRepository.UpdateAsync(snowboard, token);
        
        if (!_userContextService.UserId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(snowboard.Id, token);
            snowboard.Rating = rating;
            return snowboard;
        }
        
        var ratings = await _ratingRepository.GetRatingAsync(snowboard.Id, _userContextService.UserId.Value, token);
        snowboard.Rating = ratings.Rating;
        snowboard.UserRating = ratings.UserRating;
        
        return snowboard;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _snowboardRepository.DeleteByIdAsync(id, token);
    }
}
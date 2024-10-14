using FluentValidation;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application.Services;

public class SnowboardService : ISnowboardService
{
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly IValidator<Snowboard> _snowboardValidator;
    private readonly IRatingRepository _ratingRepository;

    public SnowboardService(ISnowboardRepository snowboardRepository, IValidator<Snowboard> snowboardValidator, IRatingRepository ratingRepository)
    {
        _snowboardRepository = snowboardRepository;
        _snowboardValidator = snowboardValidator;
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> CreateAsync(Snowboard snowboard, CancellationToken token = default)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard, cancellationToken: token);
        return await _snowboardRepository.CreateAsync(snowboard, token);
    }

    public Task<Snowboard?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default)
    {
        return _snowboardRepository.GetByIdAsync(id, userid, token);
    }

    public Task<Snowboard?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default)
    {
        return _snowboardRepository.GetBySlugAsync(slug, userid, token);
    }

    public Task<IEnumerable<Snowboard>> GetAllAsync(Guid? userid = default, CancellationToken token = default)
    {
        return _snowboardRepository.GetAllAsync(userid, token);
    }

    public async Task<Snowboard?> UpdateAsync(Snowboard snowboard, Guid? userid = default, CancellationToken token = default)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard, cancellationToken: token);
        var snowboardExists = await _snowboardRepository.ExistsByIdAsync(snowboard.Id);
        if (!snowboardExists)
        {
            return null;
        }

        await _snowboardRepository.UpdateAsync(snowboard, token);
        
        if (!userid.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(snowboard.Id, token);
            snowboard.Rating = rating;
            return snowboard;
        }
        
        var ratings = await _ratingRepository.GetRatingAsync(snowboard.Id, userid.Value, token);
        snowboard.Rating = ratings.Rating;
        snowboard.UserRating = ratings.UserRating;
        
        return snowboard;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _snowboardRepository.DeleteByIdAsync(id, token);
    }
}
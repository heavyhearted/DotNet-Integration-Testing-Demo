using FluentValidation;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application.Services;

public class SnowboardService : ISnowboardService
{
    private readonly ISnowboardRepository _snowboardRepository;
    private readonly IValidator<Snowboard> _snowboardValidator;

    public SnowboardService(ISnowboardRepository snowboardRepository, IValidator<Snowboard> snowboardValidator)
    {
        _snowboardRepository = snowboardRepository;
        _snowboardValidator = snowboardValidator;
    }

    public async Task<bool> CreateAsync(Snowboard snowboard, CancellationToken token = default)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard, cancellationToken: token);
        return await _snowboardRepository.CreateAsync(snowboard, token);
    }

    public Task<Snowboard?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return _snowboardRepository.GetByIdAsync(id, token);
    }

    public Task<Snowboard?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return _snowboardRepository.GetBySlugAsync(slug, token);
    }

    public Task<IEnumerable<Snowboard>> GetAllAsync(CancellationToken token = default)
    {
        return _snowboardRepository.GetAllAsync(token);
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
        return snowboard;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _snowboardRepository.DeleteByIdAsync(id, token);
    }
}
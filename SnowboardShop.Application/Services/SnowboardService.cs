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

    public async Task<bool> CreateAsync(Snowboard snowboard)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard);
        return await _snowboardRepository.CreateAsync(snowboard);
    }

    public Task<Snowboard?> GetByIdAsync(Guid id)
    {
        return _snowboardRepository.GetByIdAsync(id);
    }

    public Task<Snowboard?> GetBySlugAsync(string slug)
    {
        return _snowboardRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Snowboard>> GetAllAsync()
    {
        return _snowboardRepository.GetAllAsync();
    }

    public async Task<Snowboard?> UpdateAsync(Snowboard snowboard)
    {
        await _snowboardValidator.ValidateAndThrowAsync(snowboard);
        var snowboardExists = await _snowboardRepository.ExistsByIdAsync(snowboard.Id);
        if (!snowboardExists)
        {
            return null;
        }

        await _snowboardRepository.UpdateAsync(snowboard);
        return snowboard;
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return _snowboardRepository.DeleteByIdAsync(id);
    }
}
using FluentValidation;
using SnowboardShop.Application.Models;
using SnowboardShop.Application.Repositories;

namespace SnowboardShop.Application.Validators;

public class SnowboardValidator : AbstractValidator<Snowboard>
{
    private readonly ISnowboardRepository _snowboardRepository;

    public SnowboardValidator(ISnowboardRepository snowboardRepository)
    {
        _snowboardRepository = snowboardRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty();
      
        RuleFor(x => x.SnowboardLineup)
            .NotEmpty();
        
        RuleFor(x => x.SnowboardBrand)
            .NotEmpty();
        
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
            .WithMessage("Year of release must be a valid Snowboard Season year, up to and including next year.");
        
        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This Snowboard Collection already exists in the system.");
    }

    private async Task<bool> ValidateSlug(Snowboard snowboard, string slug, CancellationToken token = default)
    {
        var existingSnowboard = await _snowboardRepository.GetBySlugAsync(slug);

        if (existingSnowboard is not null)
        {
            return existingSnowboard.Id == snowboard.Id;
        }

        return existingSnowboard is null;
    }
}
using System.Text.RegularExpressions;

namespace SnowboardShop.Application.Models;

public partial class Snowboard
{
    public required Guid Id { get; init; }
    
    public required string SnowboardBrand { get; set; }
    
    public string Slug => GenerateSlug();

    
    public required int YearOfRelease { get; set; }
    
    public required List<string> SnowboardLineup { get; init; } = new();
    
    private string GenerateSlug()
    {
        var sluggedSnowboardBrand = SlugRegex().Replace(SnowboardBrand, string.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedSnowboardBrand}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}
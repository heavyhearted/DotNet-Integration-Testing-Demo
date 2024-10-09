namespace SnowboardShop.Application.Models;

public class Snowboard
{
    public required Guid Id { get; init; }
    
    public required string SnowboardBrand { get; set; }
    
    public required int YearOfRelease { get; set; }
    
    public required List<string> SnowboardProfile { get; init; } = new();
}
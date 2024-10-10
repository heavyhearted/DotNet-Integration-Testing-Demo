namespace SnowboardShop.Contracts.Responses;

public class SnowboardResponse
{
    public required Guid Id { get; init; }
    
    public required string SnowboardBrand { get; init; }
    
    public required string Slug { get; init; }

    public required int YearOfRelease { get; init; }

    public required IEnumerable<string> SnowboardLineup { get; init; } = Enumerable.Empty<string>();
}
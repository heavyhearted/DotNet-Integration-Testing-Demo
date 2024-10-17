using System.Text.Json.Serialization;

namespace SnowboardShop.Contracts.Responses;

public class SnowboardResponse
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("snowboardBrand")]
    public required string SnowboardBrand { get; init; }
    
    [JsonPropertyName("slug")]
    public required string Slug { get; init; }
    
    [JsonPropertyName("rating")]
    public float? Rating { get; init; }
    
    [JsonPropertyName("userRating")]
    public int? UserRating { get; init; }

    [JsonPropertyName("yearOfRelease")]
    public required int YearOfRelease { get; init; }

    [JsonPropertyName("snowboardLineup")]
    public required IEnumerable<string> SnowboardLineup { get; init; } = Enumerable.Empty<string>();
}
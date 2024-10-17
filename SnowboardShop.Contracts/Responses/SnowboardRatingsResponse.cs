using System.Text.Json.Serialization;

namespace SnowboardShop.Contracts.Responses;

public class SnowboardRatingsResponse
{
    [JsonPropertyName("snowboardId")]
    public required Guid SnowboardId { get; init; }
    
    [JsonPropertyName("slug")]
    public required string Slug { get; init; }
    
    [JsonPropertyName("rating")]
    public required int Rating { get; init; }
}
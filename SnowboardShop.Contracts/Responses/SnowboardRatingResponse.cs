using System.Text.Json.Serialization;

namespace SnowboardShop.Contracts.Responses;

public class SnowboardRatingResponse
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }
    
    [JsonPropertyName("snowboardId")]
    public required Guid SnowboardId { get; init; }
    
    [JsonPropertyName("rating")]
    public required int Rating { get; init; }
}
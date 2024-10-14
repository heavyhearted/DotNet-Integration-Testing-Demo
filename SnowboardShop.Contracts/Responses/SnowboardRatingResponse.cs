namespace SnowboardShop.Contracts.Responses;

public class SnowboardRatingResponse
{
    public required Guid SnowboardId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
}
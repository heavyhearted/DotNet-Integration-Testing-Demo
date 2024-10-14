namespace SnowboardShop.Application.Models;

public class SnowboardRating
{
    public required Guid SnowboardId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
}
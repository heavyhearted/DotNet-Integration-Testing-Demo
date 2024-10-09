namespace SnowboardShop.Contracts.Responses;

public class SnowboardsResponse
{
    public required IEnumerable<SnowboardResponse> Items { get; init; } = Enumerable.Empty<SnowboardResponse>();
}
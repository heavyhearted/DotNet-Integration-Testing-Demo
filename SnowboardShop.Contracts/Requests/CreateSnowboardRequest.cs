namespace SnowboardShop.Contracts.Requests;

public class CreateSnowboardRequest
{
    public required string SnowboardBrand { get; init; }

    public required int YearOfRelease { get; init; }

    public required IEnumerable<string> SnowboardLineup { get; init; } = Enumerable.Empty<string>();
}
using System.Text.Json.Serialization;

namespace SnowboardShop.Contracts.Responses;

public class ValidationFailureResponse
{
    [JsonPropertyName("errors")] 
    public required IEnumerable<ValidationResponse> Errors { get; init; }
}

public class ValidationResponse
{
    [JsonPropertyName("propertyName")] 
    public required string PropertyName { get; init; }

    [JsonPropertyName("message")] 
    public required string Message { get; init; }
}

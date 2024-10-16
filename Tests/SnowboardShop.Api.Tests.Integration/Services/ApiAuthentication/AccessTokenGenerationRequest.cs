namespace SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;

public class AccessTokenGenerationRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public Dictionary<string, object> CustomClaims { get; set; } = new();
}
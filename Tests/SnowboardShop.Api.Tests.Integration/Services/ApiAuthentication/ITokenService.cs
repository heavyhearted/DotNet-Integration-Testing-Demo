namespace SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;

public interface ITokenService
{
    Task<string> GetTokenAsync();
}
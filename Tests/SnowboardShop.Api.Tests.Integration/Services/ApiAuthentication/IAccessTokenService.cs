namespace SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;

public interface IAccessTokenService
{
    Task<string> GetTokenAsync();
}
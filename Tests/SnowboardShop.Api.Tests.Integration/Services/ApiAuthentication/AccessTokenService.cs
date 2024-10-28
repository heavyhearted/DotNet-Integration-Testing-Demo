using System.Net;
using FluentAssertions;
using RestSharp;
using SnowboardShop.Api.Tests.Integration.Core.Factories;

namespace SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;

public class AccessTokenService : IAccessTokenService
{
    private readonly RestClient _restClient;

    public AccessTokenService(IdentityApiSettings settings)
    {
        var url = $"https://localhost:{settings.HttpPort}";
        var options = new RestClientOptions(url)
        {
            RemoteCertificateValidationCallback = 
                (sender, certificate, chain, sslPolicyErrors) => true 
            // Ignore SSL errors due to the demo self-signed certificate in the Docker container. 
        };
        _restClient = new RestClient(options);
    }

    public async Task<string> GetTokenAsync(UserRoles roles = UserRoles.Admin | UserRoles.TrustedMember)
    {
        var request = new RestRequest("/token", Method.Post);
        
        var isAdmin = roles.HasFlag(UserRoles.Admin);
        var isTrustedMember = roles.HasFlag(UserRoles.TrustedMember); 
        
        if (isAdmin) isTrustedMember = true;

        request.AddJsonBody(new AccessTokenGenerationRequest
        {
            UserId = Guid.NewGuid(),
            Email = "dessy.tests@snowboardshop.com",
            CustomClaims = new Dictionary<string, object>
            {
                { "admin", isAdmin },
                { "trusted_member", isTrustedMember }
            }
        });

        var response = await _restClient.ExecuteAsync<AccessTokenResponse>(request);
        
        return response.Data!.AccessToken;
    }
}



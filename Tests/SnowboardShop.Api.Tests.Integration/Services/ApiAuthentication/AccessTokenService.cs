using System.Net;
using FluentAssertions;
using RestSharp;

namespace SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;

public class AccessTokenService : IAccessTokenService
{
    private readonly RestClient _restClient;
    private const string BaseUrl = "https://localhost:5003";


    public AccessTokenService()
    {
        var options = new RestClientOptions(BaseUrl)
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true // Ignore SSL errors due to the demo self-signed certificate in the Docker container. 
        };
        _restClient = new RestClient(options);
    }

    public async Task<string> GetTokenAsync()
    {
        var request = new RestRequest("/token", Method.Post);
        request.AddJsonBody(new AccessTokenGenerationRequest
        {
            UserId = Guid.NewGuid(),
            Email = "dessy.tests@snowboardshop.com",
            CustomClaims = new Dictionary<string, object>
            {
                { "admin", true },
                { "trusted_member", true }
            }
        });

        var response = await _restClient.ExecuteAsync<AccessTokenResponse>(request);

        return response.Data!.AccessToken;
    }
}



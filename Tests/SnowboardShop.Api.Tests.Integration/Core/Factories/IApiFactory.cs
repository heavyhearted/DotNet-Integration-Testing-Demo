using RestSharp;
using SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
using Xunit.Abstractions;

namespace SnowboardShop.Api.Tests.Integration.Core.Factories;

public interface IApiFactory
{
    Task<RestClient> CreateAuthenticatedRestClientAsync(
        UserRoles roles = UserRoles.Admin | UserRoles.TrustedMember,
        ITestOutputHelper? testOutputHelper = default);
}
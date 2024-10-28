using SnowboardShop.Api.Auth;
using SnowboardShop.Application.Services;

namespace SnowboardShop.Api.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => _httpContextAccessor.HttpContext!.GetUserId();
}
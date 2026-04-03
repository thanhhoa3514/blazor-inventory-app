using System.Security.Claims;
using MyApp.Server.Application.Common;

namespace MyApp.Server.Persistence.Auditing;

public sealed class HttpContextCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserInfo GetRequiredCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("Authenticated user context is required for this operation.");

        var userName = user.Identity.Name;
        if (string.IsNullOrWhiteSpace(userName))
            throw new InvalidOperationException("Authenticated user name is missing from the current request.");

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return new CurrentUserInfo(userId, userName);
    }
}

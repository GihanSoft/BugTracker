using BugTracker.Main.Common.Security;

using Microsoft.AspNetCore.Identity;

namespace BugTracker.Main.Features.Identity.Services;

internal sealed class CurrentUserInfo : ICurrentUserInfo
{
    private readonly HttpContext _httpContext;
    private readonly UserManager<AppUser> _userManager;

    public CurrentUserInfo(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException();
        _userManager = userManager;
    }

    public string UserKey => _userManager.GetUserName(_httpContext.User)
        ?? throw new InvalidOperationException();
}

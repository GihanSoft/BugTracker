using BugTracker.Main.Common.UI.Layout.Menu;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Main.Features.Backlog.UI;

internal class BacklogMenuProvider : IMenuProvider
{
    private readonly IAuthorizationService _authorizationService;
    private readonly HttpContext _httpContext;

    public BacklogMenuProvider(IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
    {
        _httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException();
        _authorizationService = authorizationService;

        IsAuthenticated = _httpContext.User.Identity != null &&
            _httpContext.User.Identities.Any(x => x.IsAuthenticated);
    }

    private bool IsAuthenticated { get; }

    public ValueTask<IReadOnlyCollection<MenuItemData>> GetEndMenuItemsAsync()
    {
        return ValueTask.FromResult<IReadOnlyCollection<MenuItemData>>([]);
    }

    public ValueTask<IReadOnlyCollection<MenuItemData>> GetStartMenuItemsAsync()
    {
        List<MenuItemData> items = [];
        if (IsAuthenticated)
        {
            items.Add(new MenuItemData("بکلاگ", "/Backlog", null, 0));
        }

        return ValueTask.FromResult<IReadOnlyCollection<MenuItemData>>(items.AsReadOnly());
    }
}

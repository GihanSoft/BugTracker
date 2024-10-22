using BugTracker.Main.Common.UI.Components.Icons;
using BugTracker.Main.Common.UI.Menu;

using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Main.Features.Identity.UI;

internal class IdentityMenuProvider : IMenuProvider
{
    private readonly IAuthorizationService _authorizationService;
    private readonly HttpContext _httpContext;

    public IdentityMenuProvider(IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
    {
        _httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException();
        _authorizationService = authorizationService;

        IsAuthenticated = _httpContext.User.Identity != null &&
            _httpContext.User.Identities.Any(x => x.IsAuthenticated);
    }

    private bool IsAuthenticated { get; }

    public ValueTask<IReadOnlyCollection<MenuItemData>> GetEndMenuItemsAsync(string url)
    {
        List<MenuItemData> items = [];
        if (IsAuthenticated)
        {
            items.Add(new LeafMenuItemData("اطلاعات", null, 0, "/Identity/Info"));
        }

        return ValueTask.FromResult<IReadOnlyCollection<MenuItemData>>(items.AsReadOnly());
    }

    public async ValueTask<IReadOnlyCollection<MenuItemData>> GetStartMenuItemsAsync(string url)
    {
        List<MenuItemData> items = [];

        var isSarvar = await _authorizationService.AuthorizeAsync(_httpContext.User, "sarvar");
        if (isSarvar.Succeeded)
        {
            items.Add(new LeafMenuItemData("کاربران", BootstrapIconKind.People, -1, "/Identity/Users"));
        }

        return items.AsReadOnly();
    }
}

using System.Text.RegularExpressions;

using BugTracker.Main.Common.Security;
using BugTracker.Main.Common.UI.Components.Icons;
using BugTracker.Main.Common.UI.Menu;

using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Main.Features.Backlog.UI;

internal partial class BacklogMenuProvider : IMenuProvider
{
    private static readonly Regex _projectUrlRegex = ProjectUrlRegex();

    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserInfo _currentUserInfo;
    private readonly HttpContext _httpContext;

    public BacklogMenuProvider(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationService authorizationService,
        ICurrentUserInfo currentUserInfo)
    {
        _httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException();
        _authorizationService = authorizationService;
        IsAuthenticated = _httpContext.User.Identity != null &&
            _httpContext.User.Identities.Any(x => x.IsAuthenticated);
        _currentUserInfo = currentUserInfo;
    }

    private bool IsAuthenticated { get; }

    public ValueTask<IReadOnlyCollection<MenuItemData>> GetEndMenuItemsAsync(string url)
    {
        return ValueTask.FromResult<IReadOnlyCollection<MenuItemData>>([]);
    }

    public ValueTask<IReadOnlyCollection<MenuItemData>> GetStartMenuItemsAsync(string url)
    {
        List<MenuItemData> items = [];
        if (IsAuthenticated)
        {
            items.Add(new LeafMenuItemData("پروژه‌ها", null, 0, $"/_/{_currentUserInfo.UserKey}"));
            items.AddRange(GetProjectMenuItems(url));
        }

        return ValueTask.FromResult<IReadOnlyCollection<MenuItemData>>(items.AsReadOnly());
    }

    private IReadOnlyCollection<MenuItemData> GetProjectMenuItems(string url)
    {
        var match = _projectUrlRegex.Match(url);
        if (match.Success)
        {
            var projectKey = match.Groups[1].Value;
            return
            [
                new BranchMenuItemData(
                    "تنظیمات پروژه",
                    BootstrapIconKind.Gear,
                    1,
                    [
                        new LeafMenuItemData(
                            "برچسب‌ها",
                            null,
                            0,
                            $"/_/{_currentUserInfo.UserKey}/{projectKey}/settings/tags"),
                        new LeafMenuItemData(
                            "export",
                            null,
                            0,
                            $"/_/{_currentUserInfo.UserKey}/{projectKey}/settings/export"),
                    ]),
            ];
        }

        return [];
    }

    [GeneratedRegex("""^_\/[a-zA-Z0-9-]+\/([a-zA-Z0-9-]+)""", RegexOptions.Compiled)]
    private static partial Regex ProjectUrlRegex();
}

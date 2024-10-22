using BugTracker.Main.Common.UI.Components.Icons;

namespace BugTracker.Main.Common.UI.Menu;

public abstract record MenuItemData(string Title, BootstrapIconKind? IconKind, int Order);
public sealed record LeafMenuItemData(string Title, BootstrapIconKind? IconKind, int Order, string Url)
    : MenuItemData(Title, IconKind, Order);
public sealed record BranchMenuItemData(string Title, BootstrapIconKind? IconKind, int Order, IReadOnlyCollection<LeafMenuItemData> Leaves)
    : MenuItemData(Title, IconKind, Order);

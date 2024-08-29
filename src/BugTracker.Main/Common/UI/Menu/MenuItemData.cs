using BugTracker.Main.Common.UI.Components.Icons;

namespace BugTracker.Main.Common.UI.Menu;

public record MenuItemData(string Title, string Url, BootstrapIconKind? IconKind, int Order);

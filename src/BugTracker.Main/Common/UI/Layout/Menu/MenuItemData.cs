﻿using BugTracker.Main.Common.UI.Components.Icons;

namespace BugTracker.Main.Common.UI.Layout.Menu;

public record MenuItemData(string Title, string Url, BootstrapIconKind? IconKind, int Order);

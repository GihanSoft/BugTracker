namespace BugTracker.Main.Common.UI.Layout.Menu;

internal interface IMenuProvider
{
    ValueTask<IReadOnlyCollection<MenuItemData>> GetStartMenuItemsAsync();
    ValueTask<IReadOnlyCollection<MenuItemData>> GetEndMenuItemsAsync();
}

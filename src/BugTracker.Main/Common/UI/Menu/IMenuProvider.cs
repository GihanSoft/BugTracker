namespace BugTracker.Main.Common.UI.Menu;

internal interface IMenuProvider
{
    ValueTask<IReadOnlyCollection<MenuItemData>> GetStartMenuItemsAsync(string url);
    ValueTask<IReadOnlyCollection<MenuItemData>> GetEndMenuItemsAsync(string url);
}

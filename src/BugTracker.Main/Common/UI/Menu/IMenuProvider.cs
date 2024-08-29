namespace BugTracker.Main.Common.UI.Menu;

internal interface IMenuProvider
{
    ValueTask<IReadOnlyCollection<MenuItemData>> GetStartMenuItemsAsync();
    ValueTask<IReadOnlyCollection<MenuItemData>> GetEndMenuItemsAsync();
}

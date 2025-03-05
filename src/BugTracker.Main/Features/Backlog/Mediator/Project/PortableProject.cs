namespace BugTracker.Main.Features.Backlog.Mediator.Project;

internal sealed record PortableProject
(
    string Key,
    DateTime CreationMoment,
    IReadOnlyList<PortableTag> Tags,
    IReadOnlyList<PortableBacklogItem> BacklogItems
);

internal sealed record PortableTag
(
    string Key,
    DateTime CreationMoment
);

internal sealed record PortableBacklogItem
(
    string Title,
    string Description,
    DateTime CreationMoment,
    IReadOnlyList<string> Tags
);

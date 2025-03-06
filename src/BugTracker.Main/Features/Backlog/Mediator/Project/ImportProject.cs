using System.Text.Json;

using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

namespace BugTracker.Main.Features.Backlog.Mediator.Project;

internal static class Import
{
    public record Request(string OwnerKey, Stream Stream) : IRequest<IResult>;

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext db)
        : IRequestHandler<Request, IResult>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = db;

        public async Task<IResult> Handle(Request request, CancellationToken ct)
        {
            if (request.OwnerKey != _currentUserInfo.UserKey)
            {
                return TypedResults.Forbid();
            }

            var portableProject = await ReadJsonAsync(request.Stream, ct);
            if (portableProject is null)
            {
                return TypedResults.BadRequest();
            }

            var tags = portableProject.Tags.Select(a => new Backlog.Tag(a.Key)).ToDictionary(a => a.Key);
            var backlogItems = portableProject.BacklogItems.Select(a => new ProductBacklogItem(a.Title, a.Description)
            {
                Tags = [.. a.Tags.Select(b => tags[b])],
            }).ToList();

            var project = new Backlog.Project(request.OwnerKey, portableProject.Key)
            {
                Tags = tags.Values,
                BacklogItems = backlogItems,
            };

            await _db.Projects.AddAsync(project, ct);
            await _db.SaveChangesAsync(ct);

            return TypedResults.Ok();
        }

        private static async ValueTask<PortableProject?> ReadJsonAsync(Stream stream, CancellationToken ct)
        {
            var portableProject = await JsonSerializer.DeserializeAsync<PortableProject>(stream, cancellationToken: ct);
            return portableProject;
        }
    }
}

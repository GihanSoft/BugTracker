using System.Text.Json;

using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

namespace BugTracker.Main.Features.Backlog.Mediator.Project;

internal static class Import
{
    public record Request(string OwnerKey, Stream Stream) : IRequest<Either<Error, LanguageExt.Unit>>;

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext db)
        : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = db;

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken ct)
        {
            if (request.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var portableProject = await ReadJsonAsync(request.Stream, ct);
            if (portableProject is null)
            {
                return Error.New("bad request");
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

            return unit;
        }

        private static async ValueTask<PortableProject?> ReadJsonAsync(Stream stream, CancellationToken ct)
        {
            var portableProject = await JsonSerializer.DeserializeAsync<PortableProject>(stream, cancellationToken: ct);
            return portableProject;
        }
    }
}

using System.Text.Json;

using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Project;

internal static class ExportProject
{
    public record Request(string OwnerKey, string ProjectKey) : IRequest<IResult>;

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext db)
        : IRequestHandler<Request, IResult>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = db;

        public async Task<IResult> Handle(Request request, CancellationToken ct)
        {
            var baseQuery = _db.Projects.Where(x => x.OwnerKey == _currentUserInfo.UserKey);
            var portableProject = await baseQuery
                .Where(a => a.Key == request.ProjectKey)
                .Include(a => a.Tags)
                .Include(a => a.BacklogItems)
                .OrderByDescending(x => x.CreationMoment)
                .Select(a => new PortableProject
                (
                    a.Key,
                    a.CreationMoment,
                    a.Tags.Select(b => new PortableTag(b.Key, b.CreationMoment)).ToList(),
                    a.BacklogItems.Select(b => new PortableBacklogItem
                    (
                        b.Title,
                        b.Description,
                        b.CreationMoment,
                        b.Tags.Select(c => c.Key).ToList()
                    )).ToList()
                )).FirstOrDefaultAsync(ct);
            var bin = JsonSerializer.SerializeToUtf8Bytes(portableProject);
            return TypedResults.File(bin, "application/json", request.ProjectKey + ".json");
        }
    }
}

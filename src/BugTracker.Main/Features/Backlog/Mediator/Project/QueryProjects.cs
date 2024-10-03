using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Backlog.Mediator;

[Mapper]
internal static partial class QueryProjects
{
    public record Request : IRequest<Either<Error, Response>>;
    public record Response(IReadOnlyCollection<Response.Project> Projects)
    {
        public record Project(string OwnerKey, string Key);
    }

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken ct)
        {
            var baseQuery = _db.Projects.Where(x => x.OwnerKey == _currentUserInfo.UserKey);
            var projects = await baseQuery
                .OrderByDescending(x => x.CreationMoment)
                .Map()
                .ToListAsync(ct);
            return new Response(projects.AsReadOnly());
        }
    }

    private static partial IQueryable<Response.Project> Map(this IQueryable<Backlog.Project> project);
}

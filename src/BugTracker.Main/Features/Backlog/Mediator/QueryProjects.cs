using System.Diagnostics;

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
        IHttpContextAccessor httpContextAccessor,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly HttpContext _httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken ct)
        {
            if (_httpContext.User.Identity?.Name is not string username)
            {
                return Error.New(new UnreachableException());
            }

            var baseQuery = _db.Projects.Where(x => x.OwnerKey == username);
            var projects = await baseQuery
                .OrderByDescending(x => x.CreationMoment)
                .Map()
                .ToListAsync(ct);
            return new Response(projects.AsReadOnly());
        }
    }

    private static partial IQueryable<Response.Project> Map(this IQueryable<Project> project);
}

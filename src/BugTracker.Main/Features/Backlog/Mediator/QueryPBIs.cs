using System.Diagnostics;

using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Backlog.Mediator;

[Mapper]
internal static partial class QueryPBIs
{
    public record Request(string ProjectOwner, string ProjectKey) : IRequest<Either<Error, Response>>;
    public record Response(IReadOnlyCollection<Response.PBI> PBIs)
    {
        public record PBI(ProductBacklogItemId Id, string Title, string Description);
    }

    public class Handler(
        IHttpContextAccessor httpContextAccessor,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly HttpContext _httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (_httpContext.User.Identity?.Name is not string username)
            {
                return Error.New(new UnreachableException());
            }

            var baseQuery = _db.ProductBacklogItems.Where(x => x.Project.OwnerKey == username);
            var pbiList = await baseQuery.Where(x => x.Project.Key == request.ProjectKey)
                .Map()
                .ToListAsync(cancellationToken);

            return new Response(pbiList.AsReadOnly());
        }
    }

    private static partial IQueryable<Response.PBI> Map(this IQueryable<ProductBacklogItem> query);
}

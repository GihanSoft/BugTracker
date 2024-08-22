
using System.Diagnostics;

using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator;

internal static class ReadPBI
{
    public record Request(ProductBacklogItemId Id) : IRequest<Either<Error, Response>>;
    public record Response(string Title, string Description);

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

            var pbi = await _db.ProductBacklogItems.Include(x => x.Project).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (pbi?.Project.OwnerKey != username)
            {
                return Error.New("access denied");
            }

            return new Response(pbi.Title, pbi.Description);
        }
    }
}

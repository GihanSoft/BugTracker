using BugTracker.Main.Features.Backlog.Data;
using MediatR;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace BugTracker.Main.Features.Backlog.Mediator;

internal static class DeletePBI
{
    public record Request(ProductBacklogItemId Id) : IRequest<Either<Error, LanguageExt.Unit>>;

    public class Handler(
        IHttpContextAccessor httpContextAccessor,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly HttpContext _httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (_httpContext.User.Identity?.Name is not string username)
            {
                return Error.New(new UnreachableException());
            }

            var pbi = await _db.ProductBacklogItems
                .Include(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (pbi?.Project.OwnerKey != username)
            {
                return Error.New("access denied");
            }

            _db.Remove(pbi);
            await _db.SaveChangesAsync(cancellationToken);

            return unit;
        }
    }
}

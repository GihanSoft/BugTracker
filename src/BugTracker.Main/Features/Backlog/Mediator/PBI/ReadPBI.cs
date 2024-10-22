using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.PBI;

internal static class ReadPBI
{
    public record Request(ProductBacklogItemId Id) : IRequest<Either<Error, Response>>;
    public record Response(string Title, string Description, IReadOnlyCollection<Response.Tag> Tags)
    {
        public sealed record Tag(string Key);
    }

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var pbi = await _db.ProductBacklogItems
                .Include(x => x.Project)
                .Include(x => x.Tags).ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (pbi?.Project.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var tags = pbi.Tags.Select(x => new Response.Tag(x.Tag.Key)).ToList().AsReadOnly();
            return new Response(pbi.Title, pbi.Description, tags);
        }
    }
}

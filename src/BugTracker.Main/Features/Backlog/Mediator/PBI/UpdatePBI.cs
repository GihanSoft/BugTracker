using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.PBI;

internal static class UpdatePBI
{
    public record Request(
        ProductBacklogItemId Id,
        string Title,
        string Description,
        IReadOnlyCollection<Request.Tag> Tags) : IRequest<Either<Error, LanguageExt.Unit>>
    {
        public sealed record Tag(string Key);
    }

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken cancellationToken)
        {
            var pbi = await _db.ProductBacklogItems
                .AsTracking()
                .Include(x => x.Tags)
                .Include(x => x.Project).ThenInclude(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (pbi?.Project.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            pbi.Title = request.Title;
            pbi.Description = request.Description;

            var toDelete = pbi.Tags.Select(x => x.Key).Except(request.Tags.Select(x => x.Key)).ToList();
            foreach (var item in toDelete)
            {
                var m = pbi.Tags.First(x => x.Key == item);
                pbi.Tags.Remove(m);
            }

            var toAdd = request.Tags.Select(x => x.Key).Except(pbi.Tags.Select(x => x.Key)).ToList();
            foreach (var item in toAdd)
            {
                var tag = pbi.Project.Tags.First(x => x.Key == item);
                pbi.Tags.Add(tag);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return unit;
        }
    }
}

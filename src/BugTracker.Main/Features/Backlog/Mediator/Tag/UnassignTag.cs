using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal class UnassignTag
{
    public sealed record Request(string TagKey, ProductBacklogItemId BacklogItemId) : IRequest<Either<Error, LanguageExt.Unit>>;

    public sealed class Handler : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly ICurrentUserInfo _currentUserInfo;
        private readonly BacklogDbContext _db;

        public Handler(ICurrentUserInfo currentUserInfo, BacklogDbContext db)
        {
            _currentUserInfo = currentUserInfo;
            _db = db;
        }

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken cancellationToken)
        {
            var pbi = await _db.ProductBacklogItems.FindAsync([request.BacklogItemId], cancellationToken);
            if (pbi is null)
            {
                return Error.New("PBI not found");
            }

            var pbiEntry = _db.Entry(pbi);
            await pbiEntry.Reference(x => x.Project).LoadAsync(cancellationToken);

            if (pbi.Project.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            await pbiEntry.Collection(x => x.Tags).LoadAsync(cancellationToken);
            if (pbi.Tags.FirstOrDefault(x => x.Key == request.TagKey) is Backlog.Tag tag)
            {
                pbi.Tags.Remove(tag);
                await _db.SaveChangesAsync(cancellationToken);
            }

            return unit;
        }
    }
}

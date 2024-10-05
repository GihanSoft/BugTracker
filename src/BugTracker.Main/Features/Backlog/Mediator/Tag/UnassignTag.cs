using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal class UnassignTag
{
    public sealed record Request(TagFullKey TagFullKey, ProductBacklogItemId BacklogItemId) : IRequest<Either<Error, LanguageExt.Unit>>;

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
            if (request.TagFullKey.ProjectFullKey.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var pbiTag = await _db.PbiTags.FirstOrDefaultAsync(x =>
                x.Tag.Key == request.TagFullKey.TagKey &&
                x.Tag.Project.OwnerKey == request.TagFullKey.ProjectFullKey.OwnerKey &&
                x.Tag.Project.Key == request.TagFullKey.ProjectFullKey.ProjectKey &&
                x.PbiId == request.BacklogItemId, cancellationToken);

            if (pbiTag is null)
            {
                return Error.New("tag not found");
            }

            _db.Remove(pbiTag);
            await _db.SaveChangesAsync(cancellationToken);
            return unit;
        }
    }
}

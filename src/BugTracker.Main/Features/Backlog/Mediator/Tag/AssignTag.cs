using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal class AssignTag
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

            var tag = await _db.Tags.AsTracking().FirstOrDefaultAsync(x =>
                x.Key == request.TagFullKey.TagKey &&
                x.Project.OwnerKey == request.TagFullKey.ProjectFullKey.OwnerKey &&
                x.Project.Key == request.TagFullKey.ProjectFullKey.ProjectKey, cancellationToken);

            if (tag is null)
            {
                return Error.New("tag not found");
            }

            PbiTag pbiTag = new()
            {
                PbiId = request.BacklogItemId,
                Pbi = null!,

                TagId = tag.Id,
                Tag = tag,

                CreationMoment = default,
            };

            tag.Tags.Add(pbiTag);

            await _db.SaveChangesAsync(cancellationToken);
            return unit;
        }
    }
}

using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal class DeleteTag
{
    public sealed record Request(ProjectFullKey ProjectFullKey, string TagKey) : IRequest<Either<Error, LanguageExt.Unit>>;

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
            if (request.ProjectFullKey.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var tag = await _db.Tags.Include(x => x.PBIs).FirstOrDefaultAsync(x =>
                x.Key == request.TagKey &&
                x.Project.OwnerKey == request.ProjectFullKey.OwnerKey &&
                x.Project.Key == request.ProjectFullKey.ProjectKey, cancellationToken);

            if (tag is null)
            {
                return Error.New("tag not found");
            }

            if (tag.PBIs.Count > 0)
            {
                return Error.New("tag is used");
            }

            _db.Remove(tag);
            await _db.SaveChangesAsync(cancellationToken);
            return unit;
        }
    }
}

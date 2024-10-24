using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal static class CreateTag
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

            var project = await _db.Projects
                .AsTracking()
                .FirstOrDefaultAsync(
                    x => x.OwnerKey == request.ProjectFullKey.OwnerKey && x.Key == request.ProjectFullKey.ProjectKey,
                    cancellationToken);

            if (project is null)
            {
                return Error.New("project not found");
            }

            project.Tags.Add(new Backlog.Tag(request.TagKey));

            await _db.SaveChangesAsync(cancellationToken);
            return unit;
        }
    }
}

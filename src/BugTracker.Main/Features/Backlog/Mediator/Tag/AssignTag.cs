using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal class AssignTag
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

            await _db.Entry(pbi).Reference(x => x.Project).LoadAsync(cancellationToken);
            await _db.Entry(pbi.Project).Collection(x => x.Tags).LoadAsync(cancellationToken);

            if (pbi.Project.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var tag = pbi.Project.Tags.FirstOrDefault(x => x.Key == request.TagKey);

            if (tag is null)
            {
                return Error.New("tag not found");
            }

            pbi.Tags.Add(tag);

            await _db.SaveChangesAsync(cancellationToken);
            return unit;
        }
    }
}

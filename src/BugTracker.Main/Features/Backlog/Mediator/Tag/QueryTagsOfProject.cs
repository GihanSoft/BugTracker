
using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Tag;

internal static class QueryTagsOfProject
{
    public sealed record Request(ProjectFullKey ProjectFullKey) : IRequest<Either<Error, Response>>;

    public sealed record Response(IReadOnlyCollection<Response.Tag> Tags)
    {
        public sealed record Tag(string Key);
    };

    public sealed class Handler : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly ICurrentUserInfo _currentUserInfo;
        private readonly BacklogDbContext _db;

        public Handler(ICurrentUserInfo currentUserInfo, BacklogDbContext db)
        {
            _currentUserInfo = currentUserInfo;
            _db = db;
        }

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.ProjectFullKey.OwnerKey != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var project = await _db.Projects.Include(x => x.Tags).FirstOrDefaultAsync(x =>
                x.OwnerKey == _currentUserInfo.UserKey &&
                x.Key == request.ProjectFullKey.ProjectKey,
                cancellationToken);

            if (project is null)
            {
                return Error.New("project not found");
            }

            var tags = project.Tags.Select(x => new Response.Tag(x.Key)).ToList().AsReadOnly();
            return new Response(tags);
        }
    }
}

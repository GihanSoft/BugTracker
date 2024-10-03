using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator.Project;

internal static class CreateProject
{
    public record Request(string Key) : IRequest<Either<Error, LanguageExt.Unit>>;

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext db)
        : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = db;

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken ct)
        {
            Backlog.Project project = new(_currentUserInfo.UserKey, request.Key);
            await _db.AddAsync(project, ct);
            try
            {
                await _db.SaveChangesAsync(ct);
                return unit;
            }
            catch (DbUpdateException ex)
            {
                return Error.New(ex);
            }
        }
    }
}

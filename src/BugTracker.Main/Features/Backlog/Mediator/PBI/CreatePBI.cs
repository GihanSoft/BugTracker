using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Backlog.Mediator;

[Mapper]
internal static partial class CreatePBI
{
    public record Request(string ProjectOwner, string ProjectKey, string Title, string Description) : IRequest<Either<Error, Response>>;
    public record Response(ProductBacklogItemId Id);

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken ct)
        {
            if (request.ProjectOwner != _currentUserInfo.UserKey)
            {
                return Error.New("access denied");
            }

            var project = await _db.Projects
                .AsTracking()
                .FirstOrDefaultAsync(
                    x => x.OwnerKey == request.ProjectOwner && x.Key == request.ProjectKey,
                    ct);

            if (project is null)
            {
                return Error.New("project not found");
            }

            ProductBacklogItem pbi = new()
            {
                Project = project,
                ProjectId = project.Id,

                Title = "",
                Description = "",
                CreationMoment = default!,
            };

            request.Map(pbi);

            await _db.AddAsync(pbi, ct);
            await _db.SaveChangesAsync(ct);
            return new Response(pbi.Id);
        }
    }

    private static partial void Map(this Request request, ProductBacklogItem destination);
}

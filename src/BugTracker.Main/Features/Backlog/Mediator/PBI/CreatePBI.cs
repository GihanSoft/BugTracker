using System.Collections.Immutable;

using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator;

internal static partial class CreatePBI
{
    public record Request(
        string ProjectOwner,
        string ProjectKey,
        string Title,
        string Description,
        IReadOnlyCollection<Request.Tag> Tags)
        : IRequest<Either<Error, Response>>
    {
        public sealed record Tag(string Key);
    }

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
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(
                    x => x.OwnerKey == request.ProjectOwner && x.Key == request.ProjectKey,
                    ct);

            if (project is null)
            {
                return Error.New("project not found");
            }

            ProductBacklogItem pbi = new(request.Title, request.Description, project);
            pbi.Tags = request.Tags.Select(x => new PbiTag
            {
                CreationMoment = default,
                PbiId = default,
                Pbi = default!,
                TagId = default,
                Tag = project.Tags.First(projectTag => projectTag.Key == x.Key),
            }).ToImmutableList();

            await _db.AddAsync(pbi, ct);
            await _db.SaveChangesAsync(ct);
            return new Response(pbi.Id);
        }
    }
}

﻿using System.Collections.Immutable;
using System.Text;

using BugTracker.Main.Common.Security;
using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Backlog.Mediator;

[Mapper]
internal static partial class QueryPBIs
{
    public record Request(string ProjectOwner, string ProjectKey) : IRequest<Either<Error, Response>>;
    public record Response(IReadOnlyCollection<Response.PBI> PBIs)
    {
        public record PBI(ProductBacklogItemId Id, string Title, IReadOnlyList<PBI.Tag> Tags)
        {
            public sealed record Tag(string Key);
        }
    }

    public class Handler(
        ICurrentUserInfo _currentUserInfo,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly ICurrentUserInfo _currentUserInfo = _currentUserInfo;
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var baseQuery = _db.ProductBacklogItems.Where(x => x.Project.OwnerKey == _currentUserInfo.UserKey);
            var pbiList = await baseQuery.Where(x => x.Project.Key == request.ProjectKey)
                .OrderByDescending(x => x.CreationMoment)
                .Select(x => new Response.PBI(x.Id, x.Title, x.Tags.Select(y => new Response.PBI.Tag(y.Key)).ToImmutableList()))
                .ToListAsync(cancellationToken);

            return new Response(pbiList.AsReadOnly());
        }
    }
}
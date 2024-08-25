﻿using System.Diagnostics;

using BugTracker.Main.Features.Backlog.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Mediator;

internal static class CreateProject
{
    public record Request(string Key) : IRequest<Either<Error, LanguageExt.Unit>>;

    public class Handler(
        IHttpContextAccessor httpContextAccessor,
        BacklogDbContext _db)
        : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly HttpContext _httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();
        private readonly BacklogDbContext _db = _db;

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken ct)
        {
            if (_httpContext.User.Identity?.Name is not string username)
            {
                return Error.New(new UnreachableException());
            }

            Project project = new()
            {
                OwnerKey = username,
                Key = request.Key,

                CreationMoment = default!,
            };

            await _db.AddAsync(project, ct);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
            {
                return Error.New(ex);
            }

            return unit;
        }
    }
}

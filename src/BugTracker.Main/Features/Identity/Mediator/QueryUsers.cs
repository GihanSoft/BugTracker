
using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Identity.Mediator;

[Mapper]
internal static partial class QueryUsers
{
    public record Request() : IRequest<Either<Error, Response>>;
    public record Response(IReadOnlyCollection<Response.User> Users)
    {
        public record User(
            string Id,
            string Username,
            string? Email,
            bool EmailConfirmed,
            string? PhoneNumber,
            bool PhoneNumberConfirmed,
            string? DisplayName,
            string? Avatar);
    }

    public class Handler(
        UserManager<AppUser> _userManager)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly UserManager<AppUser> _userManager = _userManager;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users
                .Map().ToListAsync(cancellationToken);
            return new Response(users.AsReadOnly());
        }
    }

#pragma warning disable RMG020 // Source member is not mapped to any target member
    private static partial IQueryable<Response.User> Map(this IQueryable<AppUser> query);
#pragma warning restore RMG020 // Source member is not mapped to any target member
}

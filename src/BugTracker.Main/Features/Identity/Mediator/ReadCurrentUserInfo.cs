using System.Diagnostics;
using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Identity;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Identity.Mediator;

[Mapper]
internal static partial class ReadCurrentUserInfo
{
    public record Request() : IRequest<Either<Error, Response>>;
    public record Response(
        string UserName,
        string? Email,
        bool EmailConfirmed,
        string? PhoneNumber,
        bool PhoneNumberConfirmed,
        string? DisplayName,
        string? Avatar,
        IReadOnlyCollection<Claim> Claims);

    public class Handler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> _userManager)
        : IRequestHandler<Request, Either<Error, Response>>
    {
        private readonly HttpContext _httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException();
        private readonly UserManager<AppUser> _userManager = _userManager;

        public async Task<Either<Error, Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (_httpContext.User.Identity?.Name is not string username)
            {
                return Error.New(new UnreachableException());
            }

            var user = await _userManager.GetUserAsync(_httpContext.User);
            if (user is null)
            {
                return Error.New(new UnreachableException());
            }

            return new Response(
                username,
                user.Email,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.DisplayName,
                user.Avatar,
                _httpContext.User.Claims.ToList().AsReadOnly());
        }
    }
}

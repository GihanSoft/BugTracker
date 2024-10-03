using MediatR;

using Microsoft.AspNetCore.Identity;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Identity.Mediator;

[Mapper]
internal static partial class CreateUser
{
    public record Request(
        string UserName,
        string Password,
        string Email,
        string? Avatar) : IRequest<Either<Error, IdentityResult>>;

    public class Handler(
        UserManager<AppUser> _userManager)
        : IRequestHandler<Request, Either<Error, IdentityResult>>
    {
        private readonly UserManager<AppUser> _userManager = _userManager;

        public async Task<Either<Error, IdentityResult>> Handle(Request request, CancellationToken cancellationToken)
        {
            AppUser user = new();

            _ = await _userManager.SetUserNameAsync(user, request.UserName);
            _ = await _userManager.SetEmailAsync(user, request.Email);

            user.Avatar = request.Avatar;
            var result = await _userManager.CreateAsync(user, request.Password);
            return result;
        }
    }
}

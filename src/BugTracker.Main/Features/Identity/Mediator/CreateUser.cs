
using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

namespace BugTracker.Main.Features.Identity.Mediator;

[Mapper]
internal static partial class CreateUser
{
    public record Request(string UserName, string Password, string Email) : IRequest<Either<Error, IdentityResult>>;

    public class Handler(
        UserManager<AppUser> _userManager)
        : IRequestHandler<Request, Either<Error, IdentityResult>>
    {
        private readonly UserManager<AppUser> _userManager = _userManager;

        public async Task<Either<Error, IdentityResult>> Handle(Request request, CancellationToken cancellationToken)
        {
            AppUser user = new();

            var result = await _userManager.SetUserNameAsync(user, request.UserName);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _userManager.SetEmailAsync(user, request.Email);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _userManager.CreateAsync(user, request.Password);
            return result;
        }
    }
}

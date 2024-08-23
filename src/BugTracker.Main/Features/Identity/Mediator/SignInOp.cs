
using MediatR;

using Microsoft.AspNetCore.Identity;

namespace BugTracker.Main.Features.Identity.Mediator;

internal static class SignInOp
{
    public record Request(string Username, string Password, bool Remember) : IRequest<Either<Error, SignInResult>>;
    public class Handler(SignInManager<AppUser> _signInManager)
        : IRequestHandler<Request, Either<Error, SignInResult>>
    {
        private readonly SignInManager<AppUser> _signInManager = _signInManager;

        public async Task<Either<Error, SignInResult>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(
                request.Username,
                request.Password,
                request.Remember,
                true);

            return result;
        }
    }
}

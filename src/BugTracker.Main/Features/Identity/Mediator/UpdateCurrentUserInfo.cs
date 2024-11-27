using System.Diagnostics;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace BugTracker.Main.Features.Identity.Mediator;

internal static class UpdateCurrentUserInfo
{
    public record Request(string? Avatar, string? DisplayName) : IRequest<Either<Error, LanguageExt.Unit>>;

    public class Handler : IRequestHandler<Request, Either<Error, LanguageExt.Unit>>
    {
        private readonly HttpContext _httpContext;
        private readonly UserManager<AppUser> _userManager;

        public Handler(IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _httpContext = httpContextAccessor.HttpContext
                           ?? throw new InvalidOperationException();
            _userManager = userManager;
        }

        public async Task<Either<Error, LanguageExt.Unit>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (_httpContext.User.Identity?.Name is not { } username)
            {
                return Error.New(new UnreachableException());
            }

            var user = await _userManager.GetUserAsync(_httpContext.User);
            if (user is null)
            {
                return Error.New(new UnreachableException());
            }

            user.Avatar = request.Avatar;
            user.DisplayName = request.DisplayName;
            await _userManager.UpdateAsync(user);

            return Prelude.unit;
        }
    }
}

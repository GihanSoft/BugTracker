using System.Text.Json;

using GihanSoft.Framework.Web.Bootstrap.Initialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BugTracker.Main.Features.Identity.Startup;

internal class SarvarAccountInitializer(IOptions<SarvarOptions> _sarvarOptions, UserManager<AppUser> _userManager) : IInitializer
{
    private readonly UserManager<AppUser> _userManager = _userManager;
    private readonly SarvarOptions _sarvarOptions = _sarvarOptions.Value;

    public int Priority { get; } = -1;

    public async Task InitializeAsync()
    {
        if (_sarvarOptions.SkipInitializer)
        {
            return;
        }

        var sarvar = await _userManager.FindByNameAsync("sarvar");
        if (sarvar is null)
        {
            sarvar = new()
            {
                UserName = "sarvar",
                Email = _sarvarOptions.Email,
                EmailConfirmed = true,
            };
            var result = await _userManager.CreateAsync(sarvar, _sarvarOptions.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException(JsonSerializer.Serialize(result.Errors));
            }
        }
        else
        {
            if (await _userManager.CheckPasswordAsync(sarvar, _sarvarOptions.Password) is false)
            {
                await _userManager.RemovePasswordAsync(sarvar);
                await _userManager.AddPasswordAsync(sarvar, _sarvarOptions.Password);
            }
        }
    }
}

internal class SarvarOptions
{
    public required bool SkipInitializer { get; set; }

    public required string Password { get; set; }
    public required string Email { get; set; }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Identity.Data;

internal class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityUserContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(IAssemblyMarker).Assembly,
            t => t.Namespace?.StartsWith("BugTracker.Main.Features.Identity") ?? false);

        builder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims", "identity");
        builder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins", "identity");
        builder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens", "identity");
    }
}


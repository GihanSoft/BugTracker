using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTracker.Main.Features.Identity;

internal class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; }
}

internal class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("asp_net_users", "identity");
    }
}

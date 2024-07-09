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
        builder.Property(x => x.DisplayName).HasMaxLength(256);
        builder.Property(x => x.Avatar).HasMaxLength(10485760); // 10 MB
        builder.ToTable("asp_net_users", "identity");
    }
}

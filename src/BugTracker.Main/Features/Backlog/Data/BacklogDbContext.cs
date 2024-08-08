using Microsoft.EntityFrameworkCore;

namespace BugTracker.Main.Features.Backlog.Data;

internal class BacklogDbContext(DbContextOptions<BacklogDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProductBacklogItem> ProductBacklogItems => Set<ProductBacklogItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(IAssemblyMarker).Assembly,
            t => t.Namespace?.StartsWith("BugTracker.Main.Features.Backlog") ?? false);
    }
}

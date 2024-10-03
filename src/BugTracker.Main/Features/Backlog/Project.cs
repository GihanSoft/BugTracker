using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-int", "gs-int-ef")]
internal readonly partial struct ProjectId { }

internal sealed class Project
{
    // used by ef
    private Project(ProjectId id, string ownerKey, string key, DateTime creationMoment)
        => (Id, OwnerKey, Key, CreationMoment) = (id, ownerKey, key, creationMoment);

    public Project(string ownerKey, string key)
        : this(default, ownerKey, key, default) { }

    public ProjectId Id { get; private set; }

    public string OwnerKey { get; private set; }
    public string Key { get; private set; }

    public DateTime CreationMoment { get; private set; }

    public ICollection<ProductBacklogItem> BacklogItems { get; } = [];
}

internal class ProjectConfig : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("project", "backlog");
        builder.HasIndex([nameof(Project.OwnerKey), nameof(Project.Key)])
            .IsUnique(true);

        builder.Property(x => x.Id).HasConversion<ProjectId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();
        builder.Property(x => x.OwnerKey).HasMaxLength(256);
        builder.Property(x => x.Key).HasMaxLength(256);

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();
    }
}

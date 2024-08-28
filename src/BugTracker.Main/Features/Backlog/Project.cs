using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-int", "gs-int-ef")]
internal readonly partial struct ProjectId { }

internal class Project
{
    public ProjectId Id { get; set; }

    public required string OwnerKey { get; set; }
    public required string Key { get; set; }

    public required DateTime CreationMoment { get; set; }

    public ICollection<ProductBacklogItem> BacklogItems { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
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

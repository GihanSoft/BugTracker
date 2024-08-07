﻿using Microsoft.EntityFrameworkCore;
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

    public ICollection<ProductBacklogItem> BacklogItems { get; set; } = [];
}

internal class ProjectConfig : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("project", "backlog");
        builder.HasIndex([nameof(Project.OwnerKey), nameof(Project.Key)]);

        builder.Property(x => x.Id).HasConversion<ProjectId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();
        builder.Property(x => x.OwnerKey).HasMaxLength(256);
        builder.Property(x => x.Key).HasMaxLength(256);
    }
}

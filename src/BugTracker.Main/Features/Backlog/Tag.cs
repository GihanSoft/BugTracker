using BugTracker.Main.Common.Ef.ValueGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-long", "gs-long-ef")]
internal readonly partial struct TagId { }

internal class Tag
{
    public TagId Id { get; set; }

    public required string Key { get; set; }

    public required DateTime CreationMoment { get; set; }

    public required ProjectId ProjectId { get; set; }
    public required Project Project { get; set; }

    public ICollection<PbiTag> Tags { get; set; } = [];
}

internal class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tag", "backlog");
        builder.HasIndex([nameof(Tag.Key), nameof(Tag.ProjectId)])
            .IsUnique(true);

        builder.Property(x => x.Id).HasConversion<TagId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Key).HasMaxLength(256);
        builder.Property(x => x.ProjectId).HasConversion<ProjectId.EfCoreValueConverter>();

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();
    }
}


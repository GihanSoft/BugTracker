using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-long", "gs-long-ef")]
internal readonly partial struct PbiTagId { }

internal class PbiTag
{
    public PbiTagId Id { get; set; }

    public required DateTime CreationMoment { get; set; }

    public required ProductBacklogItemId PbiId { get; set; }
    public required ProductBacklogItem Pbi { get; set; }

    public required TagId TagId { get; set; }
    public required Tag Tag { get; set; }
}

internal class PbiTagConfig : IEntityTypeConfiguration<PbiTag>
{
    public void Configure(EntityTypeBuilder<PbiTag> builder)
    {
        builder.ToTable("pbi_tag", "backlog");
        builder.HasIndex([nameof(PbiTag.PbiId), nameof(PbiTag.TagId)])
            .IsUnique(true);

        builder.Property(x => x.Id).HasConversion<PbiTagId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();
    }
}

using BugTracker.Main.Common.Ef.ValueGenerators;
using BugTracker.Main.Features.Backlog.Data.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTracker.Main.Features.Backlog;

internal class PbiTag
{
    private PbiTag(DateTime creationMoment) => (CreationMoment, Pbi, Tag) = (creationMoment, null!, null!);

    public DateTime CreationMoment { get; private set; }

    public ProductBacklogItem Pbi { get; private set; }
    public Tag Tag { get; private set; }
}

internal class PbiTagConfig : IEntityTypeConfiguration<PbiTag>
{
    public void Configure(EntityTypeBuilder<PbiTag> builder)
    {
        builder.ToTable("pbi_tag", "backlog");

        builder.Property<long>("Id").UseIdentityAlwaysColumn();

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();
    }
}

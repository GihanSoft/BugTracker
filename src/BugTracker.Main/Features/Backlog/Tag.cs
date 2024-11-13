using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-long", "gs-long-ef")]
internal readonly partial struct TagId { }

internal sealed record Tag
{
    // used by ef
    private Tag(TagId id, string key, DateTime creationMoment)
        => (Id, Key, CreationMoment, Project) = (id, key, creationMoment, null!);

    public Tag(string key) : this(default, key, default) { }

    public TagId Id { get; private init; }

    public string Key { get; private init; }

    public DateTime CreationMoment { get; private init; }

    public Project Project { get; private init; }

    public ICollection<ProductBacklogItem> PBIs { get; init; } = [];
}

internal class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tag", "backlog");

        builder.Property(x => x.Id).HasConversion<TagId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Key).HasMaxLength(256);

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();

        builder
            .HasMany(x => x.PBIs)
            .WithMany(x => x.Tags)
            .UsingEntity<PbiTag>();
    }
}


using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-long", "gs-long-parsable", "gs-long-ef")]
internal readonly partial struct ProductBacklogItemId { }

internal sealed record ProductBacklogItem
{
    // used by ef
    private ProductBacklogItem(ProductBacklogItemId id, string title, string description, DateTime creationMoment)
        => (Id, Title, Description, CreationMoment, Project) = (id, title, description, creationMoment, null!);

    public ProductBacklogItem(string title, string description)
        : this(default, title, description, default) { }

    public ProductBacklogItemId Id { get; private init; }

    public string Title { get; init; }
    public string Description { get; init; }

    public DateTime CreationMoment { get; private init; }

    public Project Project { get; private init; }

    public ICollection<Tag> Tags { get; init; } = [];
}

internal class ProductBacklogItemConfig : IEntityTypeConfiguration<ProductBacklogItem>
{
    public void Configure(EntityTypeBuilder<ProductBacklogItem> builder)
    {
        builder.ToTable("product_backlog_item", "backlog");

        builder.Property(x => x.Id).HasConversion<ProductBacklogItemId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();
        builder.Property(x => x.Title).HasMaxLength(1024);

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();

        builder
            .HasMany(x => x.Tags)
            .WithMany(x => x.PBIs)
            .UsingEntity<PbiTag>();
    }
}

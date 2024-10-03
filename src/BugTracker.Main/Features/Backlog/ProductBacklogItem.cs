using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-long", "gs-long-ef")]
internal readonly partial struct ProductBacklogItemId { }

internal class ProductBacklogItem
{
    // used by ef
    private ProductBacklogItem(ProductBacklogItemId id, string title, string description, DateTime creationMoment)
        => (Id, Title, Description, CreationMoment, Project) = (id, title, description, creationMoment, null!);

    public ProductBacklogItem(string title, string description, Project project)
        : this(default, title, description, default)
        => Project = project;

    public ProductBacklogItemId Id { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime CreationMoment { get; set; }

    public Project Project { get; set; }
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
    }
}

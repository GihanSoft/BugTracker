using BugTracker.Main.Common.Ef.ValueGenerators;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StronglyTypedIds;

namespace BugTracker.Main.Features.Backlog;

[StronglyTypedId("gs-long", "gs-long-ef")]
internal readonly partial struct ProductBacklogItemId { }

internal class ProductBacklogItem
{
    public ProductBacklogItemId Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required DateTime CreationMoment { get; set; }

    public required ProjectId ProjectId { get; set; }
    public required Project Project { get; set; }
}

internal class ProductBacklogItemConfig : IEntityTypeConfiguration<ProductBacklogItem>
{
    public void Configure(EntityTypeBuilder<ProductBacklogItem> builder)
    {
        builder.ToTable("product_backlog_item", "backlog");

        builder.Property(x => x.Id).HasConversion<ProductBacklogItemId.EfCoreValueConverter>()
            .UseIdentityAlwaysColumn();
        builder.Property(x => x.Title).HasMaxLength(1024);
        builder.Property(x => x.ProjectId).HasConversion<ProjectId.EfCoreValueConverter>();

        builder.Property(x => x.CreationMoment)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UtcNowDateTimeValueGenerator>();
    }
}

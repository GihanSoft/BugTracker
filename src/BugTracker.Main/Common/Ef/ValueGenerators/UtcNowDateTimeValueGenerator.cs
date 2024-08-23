using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace BugTracker.Main.Common.Ef.ValueGenerators;

internal class UtcNowDateTimeValueGenerator : ValueGenerator<DateTime>
{
    public override bool GeneratesTemporaryValues { get; }

    public override DateTime Next(EntityEntry entry) => DateTime.UtcNow;
}

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BugTracker.Main.Features.Backlog.Data;

namespace BugTracker.Main.Features.Backlog.Startup;

internal static class BacklogServices
{
    public static IServiceCollection AddBacklogService(this IServiceCollection services)
    {
        AddDbContext(services);

        return services;
    }

    private static ValueTuple AddDbContext(IServiceCollection services)
    {
        services.AddDbContext<BacklogDbContext>((sp, builder) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var environment = sp.GetRequiredService<IHostEnvironment>();

            var connectionString = configuration.GetConnectionString("backlog")
                ?? configuration.GetConnectionString("default");
            builder.UseNpgsql(connectionString, opt =>
            {
                opt.MigrationsHistoryTable("__ef_migrations_history", "backlog");
            });

            builder.UseSnakeCaseNamingConvention();
            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            builder.ConfigureWarnings(opt =>
                opt.Log([
                    (CoreEventId.FirstWithoutOrderByAndFilterWarning, LogLevel.Warning),
                    (CoreEventId.RowLimitingOperationWithoutOrderByWarning, LogLevel.Warning),
                    (CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning, LogLevel.Warning),
                ]));

            if (environment.IsDevelopment())
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
            }
        });
        return default;
    }
}

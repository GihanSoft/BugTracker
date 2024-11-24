using BugTracker.Main.Features.Backlog.Data;
using BugTracker.Main.Features.Identity.Data;
using BugTracker.MigrationsApplier;

using EntityFramework.Exceptions.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddDbContext<IdentityDbContext>((sp, builder) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var environment = sp.GetRequiredService<IHostEnvironment>();

    var connectionString = configuration.GetConnectionString("identity")
        ?? configuration.GetConnectionString("default");
    builder.UseNpgsql(connectionString, opt =>
    {
        opt.MigrationsHistoryTable("__ef_migrations_history", "identity");
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
builder.EnrichNpgsqlDbContext<IdentityDbContext>();
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

builder.Services.AddDbContext<BacklogDbContext>((sp, builder) =>
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

    builder.UseExceptionProcessor();

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
builder.EnrichNpgsqlDbContext<BacklogDbContext>();
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<BacklogDbContext>());

var host = builder.Build();

host.Run();

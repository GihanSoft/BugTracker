using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using OpenTelemetry.Trace;

namespace BugTracker.MigrationsApplier;

public sealed class Worker : BackgroundService
{
    public const string ActivitySourceName = "MigrationsApplier";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public Worker(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);
        using var scope = _serviceProvider.CreateScope();
        var dbContextEnumerable = scope.ServiceProvider.GetRequiredService<IEnumerable<DbContext>>();

        foreach (var dbContext in dbContextEnumerable)
        {
            try
            {
                await EnsureDatabaseAsync(dbContext, stoppingToken);
                await RunMigrationAsync(dbContext, stoppingToken);
                await SeedDataAsync(dbContext, stoppingToken);
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);
                throw;
            }
        }

        _hostApplicationLifetime.StopApplication();
    }

    private async ValueTask EnsureDatabaseAsync(DbContext dbContext, CancellationToken stoppingToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(stoppingToken))
            {
                await dbCreator.CreateAsync(stoppingToken);
            }
        });
    }

    private async ValueTask RunMigrationAsync(DbContext dbContext, CancellationToken stoppingToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);
            await dbContext.Database.MigrateAsync(stoppingToken);
            await transaction.CommitAsync(stoppingToken);
        });
    }

    private async ValueTask SeedDataAsync(DbContext dbContext, CancellationToken stoppingToken)
    {
        /* // sample
        SupportTicket firstTicket = new()
        {
            Title = "Test Ticket",
            Description = "Default ticket, please ignore!",
            Completed = true
        };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.Tickets.AddAsync(firstTicket, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
        */
    }
}

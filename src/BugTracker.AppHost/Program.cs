var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin(b => b.WithLifetime(ContainerLifetime.Persistent))
    ;
var postgres_default = postgres.AddDatabase("postgres-default", "postgres");

var migration_applier = builder.AddProject<Projects.BugTracker_MigrationsApplier>("bugtracker-migrations-applier")
    .WithReference(postgres_default, "default")
    .WaitFor(postgres)
    ;

builder.AddProject<Projects.BugTracker_Main>("bugtracker-main")
    .WithReference(postgres_default, "default")
    .WaitForCompletion(migration_applier)
    ;

builder.Build().Run();

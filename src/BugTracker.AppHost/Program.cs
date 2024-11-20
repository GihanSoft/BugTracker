var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BugTracker_Main>("bugtracker-main");

builder.Build().Run();

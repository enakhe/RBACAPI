var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebAPI>("web-api");

builder.AddProject<Projects.UI>("ui");

builder.Build().Run();

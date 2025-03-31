var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisCommander();

var webApi = builder.AddProject<Projects.WebAPI>("web-api")
    .WithReference(cache);

builder.AddProject<Projects.UI>("ui")
    .WithReference(webApi);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisCommander();
var sqldb = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .AddDatabase("db");

var webApi = builder.AddProject<Projects.WebAPI>("web-api")
    .WithReference(sqldb)
    .WithReference(cache)
    .WaitFor(sqldb);

builder.AddProject<Projects.UI>("ui")
    .WithReference(webApi);

builder.Build().Run();

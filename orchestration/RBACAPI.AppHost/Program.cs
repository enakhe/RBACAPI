var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisCommander();

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var db = sql.AddDatabase("RBAC");

var webApi = builder.AddProject<Projects.WebAPI>("web-api")
    .WithReference(db)
    .WithReference(cache)
    .WaitFor(db);

builder.AddProject<Projects.UI>("ui")
    .WithReference(webApi);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisCommander();

var password = builder.AddParameter("DM(wpZ12(PC6QC{!7bV)rQ", secret: true);
var sql = builder.AddSqlServer("sql", password)
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

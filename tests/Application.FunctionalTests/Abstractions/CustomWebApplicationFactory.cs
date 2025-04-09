using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Infrastructure.Data;
using Testcontainers.MsSql;
using Xunit;

namespace RBACAPI.Application.FunctionalTests.Abstractions;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU10-ubuntu-22.04")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_container.GetConnectionString(), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.UseRelationalNulls();
            }));
        });
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public async new Task DisposeAsync()
    {
        await _container.StopAsync();
        await base.DisposeAsync();
    }
}

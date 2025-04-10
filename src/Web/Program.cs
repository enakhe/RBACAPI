using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Infrastructure.Data;
using RBACAPI.Infrastructure.Middleware;
using RBACAPI.Application.Common.Exceptions;
using RBACAPI.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Custom service registrations
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");
builder.AddRedisClient("cache");

builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddHttpContextAccessor();

// Configure the ProblemDetails middleware.
builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();

    options.Map<ValidationException>(ex => new ProblemDetails
    {
        Title = "Validation Failed",
        Status = StatusCodes.Status400BadRequest,
        Detail = ex.Message,
        Extensions = { ["errors"] = ex.Errors }
    });

    options.Map<UnauthorizedAccessException>(ex => new ProblemDetails
    {
        Title = "Unauthorized",
        Status = StatusCodes.Status401Unauthorized,
        Detail = ex.Message,
    });

    options.Map<KeyNotFoundException>(ex => new ProblemDetails
    {
        Title = "Not Found",
        Status = StatusCodes.Status404NotFound,
        Detail = ex.Message
    });

    options.Map<BadHttpRequestException>(ex => new ProblemDetails
    {
        Title = "Bad Request",
        Status = StatusCodes.Status400BadRequest,
        Detail = ex.Message
    });

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

var app = builder.Build();


app.MapDefaultEndpoints();
app.MapEndpoints();

// Development tools (Swagger & DB seeding)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RBACAPI API v1");
    });
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

// Middleware setup
app.UseProblemDetails();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.UseOutputCache();
app.UseMiddleware<JwtCookieAuthMiddleware>();

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

// MVC Controllers and Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

// SPA & Swagger fallback
app.MapFallbackToFile("index.html");
app.Map("/", () => Results.Redirect("/swagger"));

app.Run();
public partial class Program { }

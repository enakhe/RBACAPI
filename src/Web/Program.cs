using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using RBACAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Infrastructure.Middleware;
using RBACAPI.Application.Common.Exceptions;
using RBACAPI.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");
builder.AddRedisClient("cache");

// Add services to the container.
builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddHttpContextAccessor();

// Configure ProblemDetails middleware
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

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Role Based Access Control API",
        Description = "A full Role Based Access Controle API for performing administration activities, priviledges, and permissions",
        Contact = new OpenApiContact { Name = "Samuel Izuagbe", Email = "izuagbesam@gmail.com" }
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "Auth.JWT.AccessToken",
        In = ParameterLocation.Header
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseProblemDetails();
app.UseRouting();
app.UseOutputCache();

app.UseAuthentication();
app.UseMiddleware<JwtCookieAuthMiddleware>();  // Custom middleware for JWT cookie handling

app.UseAuthorization();

// Health checks
app.UseHealthChecks("/health");

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Static file handling
app.UseStaticFiles();

// Caching and output optimization (if needed)

// Swagger UI and API documentation
app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

// Route setup
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

// Fallback route for Single Page Application (SPA)
app.MapFallbackToFile("index.html");

app.Map("/", () => Results.Redirect("/api"));
// Minimal API route mapping (if applicable)
app.MapEndpoints();

app.Run();


public partial class Program { }

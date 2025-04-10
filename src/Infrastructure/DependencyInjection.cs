using System.Text;
using EcommerceAPI.Infrastructure.Repository;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RBACAPI.Application.Auth.Commands.SignIn;
using RBACAPI.Application.Common.Behaviours;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Domain.Constants;
using RBACAPI.Infrastructure.Data;
using RBACAPI.Infrastructure.Data.Interceptors;
using RBACAPI.Infrastructure.Identity;
using RBACAPI.Infrastructure.Interface;
using RBACAPI.Infrastructure.Repository;
using StackExchange.Redis;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("sql");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });

        // JWT Authentication configuration
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["Auth.JWT.AccessToken"];
                    return Task.CompletedTask;
                }
            };
        });

        // Social Login Configuration (Google, Facebook)
        services.AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            })
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Authentication:Facebook:AppId"]!;
                facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"]!;
            });

        // Redis Cache
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConfiguration = configuration.GetConnectionString("cache");
            return ConnectionMultiplexer.Connect(redisConfiguration!);
        });

        // Identity configuration
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddSingleton(TimeProvider.System);

        // Add services for identity and account management
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IJWTService, JWTRepository>();
        services.AddScoped<IOAuthService, OAuthService>();
        services.AddScoped<IOTPService, OTPService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddTransient<IUserEmailStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SignInCommandHandler>());
        services.AddValidatorsFromAssemblyContaining<SignInCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }


}

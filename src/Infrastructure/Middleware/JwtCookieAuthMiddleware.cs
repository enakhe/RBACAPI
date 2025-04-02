using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RBACAPI.Infrastructure.Identity;
using RBACAPI.Infrastructure.Interface;

namespace RBACAPI.Infrastructure.Middleware;
public class JwtCookieAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtCookieAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager, IJWTService jWTService)
    {
        var accessToken = context.Request.Cookies["Auth.JWT.AccessToken"];
        var refreshToken = context.Request.Cookies["Auth.JWT.RefreshToken"];

        bool isExpired = true;
        string? userId = null;

        if (!string.IsNullOrEmpty(accessToken))
        {
            userId = jWTService.ValidateJWTToken(accessToken, out isExpired);
        }

        if (isExpired && !string.IsNullOrEmpty(refreshToken))
        {
            var refreshResult = await jWTService.RefreshTokenAsync(refreshToken);

            if (refreshResult != null)
            {
                var tokens = (dynamic)refreshResult!;
                accessToken = tokens.accessToken;
                userId = jWTService.ValidateJWTToken(accessToken, out isExpired);
            }
        }

        if (!string.IsNullOrEmpty(userId))
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!)
                };

                var identity = new ClaimsIdentity(claims, "Bearer");
                var principal = new ClaimsPrincipal(identity);

                context.User = principal;
            }
        }

        await _next(context);
    }
}

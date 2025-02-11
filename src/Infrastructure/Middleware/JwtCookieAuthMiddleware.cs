using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RBACAPI.Infrastructure.Identity;
using RBACAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

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

        var userId = jWTService.ValidateJWTToken(accessToken!);
        if (string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(refreshToken))
        {
            var newAccessToken = await jWTService.RefreshTokenAsync(refreshToken);

            if (!string.IsNullOrEmpty(newAccessToken))
            {
                userId = jWTService.ValidateJWTToken(newAccessToken);
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

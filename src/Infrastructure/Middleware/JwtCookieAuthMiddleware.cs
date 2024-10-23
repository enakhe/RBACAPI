using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Infrastructure.Identity;
using EcommerceAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EcommerceAPI.Infrastructure.Middleware;
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
        var token = context.Request.Cookies["JWT"];
        if (string.IsNullOrEmpty(token))
        {
            await _next(context);
            return;
        }

        var userId = jWTService.ValidateJWTToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

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

        await _next(context);
    }

}

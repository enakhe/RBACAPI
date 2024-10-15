using EcommerceAPI.Infrastructure.Identity;
using EcommerceAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Infrastructure.Repository;

public class JWTRepository : IJWTService
{
    private readonly IConfiguration configuration;
    public JWTRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public string GenerateJWTToken(HttpContext context, ApplicationUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt.Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        context.Response.Cookies.Append("JWT", jwt, new CookieOptions()
        {
            Expires = DateTimeOffset.Now.AddHours(1),
            MaxAge = TimeSpan.FromHours(1),
            SameSite = SameSiteMode.Strict,
            Secure = true,
        });
        return jwt;
    }
}

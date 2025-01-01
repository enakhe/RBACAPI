using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Infrastructure.Identity;
using EcommerceAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Infrastructure.Repository;

public class JWTRepository : IJWTService
{
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public JWTRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        this.configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }
    public string GenerateToken(HttpContext context, ApplicationUser user, string tokenName, DateTimeOffset validTime)
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
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //context.Response.Cookies.Append($"Auth.JWT.{tokenName}", jwt, new CookieOptions()
        //{
        //    Expires = validTime,
        //    HttpOnly = true,
        //    Secure = true,
        //    SameSite = SameSiteMode.Strict,
        //    IsEssential = true
        //});
        return jwt;
    }

    public string? ValidateJWTToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<string?> RefreshTokenAsync(string refreshToken)
    {
        var tokenValidationResult = ValidateJWTToken(refreshToken);
        if (string.IsNullOrEmpty(tokenValidationResult))
            return null;

        var user = await _userManager.FindByIdAsync(tokenValidationResult!);
        return GenerateToken(_httpContextAccessor.HttpContext!, user!, "AccessToken", DateTimeOffset.UtcNow.AddMinutes(30));
    }
}


using RBACAPI.Infrastructure.Identity;

namespace RBACAPI.Infrastructure.Interface;
public interface IJWTService
{
    string GenerateToken(ApplicationUser user, DateTimeOffset validTime);
    string? ValidateJWTToken(string token, out bool isExpired);
    Task<string?> RefreshTokenAsync(string refreshToken);
}

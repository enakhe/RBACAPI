using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RBACAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace RBACAPI.Infrastructure.Interface;
public interface IJWTService
{
    string GenerateToken(ApplicationUser user, string tokenName, DateTimeOffset validTime);
    string? ValidateJWTToken(string token);
    Task<string?> RefreshTokenAsync(string refreshToken);
}

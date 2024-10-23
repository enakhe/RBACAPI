using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EcommerceAPI.Infrastructure.Interface;
public interface IJWTService
{
    string GenerateJWTToken(HttpContext context, ApplicationUser user);
    string? ValidateJWTToken(string token);
}

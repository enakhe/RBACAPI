using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Infrastructure.Repository;
public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetCookie(string data, string cookieName)
    {
        var context = _httpContextAccessor.HttpContext!;
        context.Response.Cookies.Append(cookieName, data, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            IsEssential = true
        });
    }
}

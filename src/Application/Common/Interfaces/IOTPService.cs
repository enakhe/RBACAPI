using Microsoft.AspNetCore.Http;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IOTPService
{
    Task<string> GenerateOTPAsync(string userId, string email, string token, DateTimeOffset expiryDate);
    Task<Result> ValidateOTPAsync(string email, string otp);
    OtpCookieData GetOtpCookieData(HttpContext context);
}

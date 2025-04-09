#nullable disable

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;
using StackExchange.Redis;

namespace RBACAPI.Infrastructure.Repository;
public class OTPService : IOTPService
{
    private readonly IConfiguration configuration;
    private readonly IDatabase _redisDb;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public OTPService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer redis)
    {
        this.configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _redisDb = redis.GetDatabase();
    }

    private static string GetRedisKey(string email) => $"otp:{email.ToLower()}";

    public async Task<string> GenerateOTPAsync(string userId, string email, string token, DateTimeOffset expiryDate)
    {
        string data = $"{userId}{email}{expiryDate:yyyy-MM-dd HH:mm:ss}{token}";

        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)))
        {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            long hashValue = BitConverter.ToInt64(hashBytes, 0);
            int otp = Math.Abs((int)(hashValue % 100000));
            await _redisDb.StringSetAsync(GetRedisKey(email), otp, TimeSpan.FromMinutes(5));
            return otp.ToString("D5");
        }
    }

    public async Task<Result> ValidateOTPAsync(string email, string otp)
    {
        var redisKey = GetRedisKey(email);
        var savedOtp = await _redisDb.StringGetAsync(redisKey);

        if (string.IsNullOrEmpty(savedOtp))
        {
            return Result.Failure(
                "One or more validation failures have occurred",
                ["OTP code has expired or was never issued, kindly request another one"]
            );
        }

        if (otp != savedOtp)
            return Result.Failure(
                "One or more validation failures have occurred",
                ["Verification of OTP failed, kindly request another one"]);

        await _redisDb.KeyDeleteAsync(redisKey);

        return Result.Success(
            "Sucessfully Verified Email",
            "Email verification passed. You can now login and access your account",
            new { }
        );
    }

    public void SetOtpCookie(HttpContext context, string otp)
    {
        var otpData = new OtpCookieData
        {
            Otp = otp,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
        };

        var json = JsonSerializer.Serialize(otpData);

        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddMinutes(5),
            HttpOnly = true,
            Secure = true
        };

        context.Response.Cookies.Append("OTP", json, cookieOptions);
    }

    public OtpCookieData GetOtpCookieData(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("OTP", out var cookieValue))
        {
            return JsonSerializer.Deserialize<OtpCookieData>(cookieValue);
        }

        return null;
    }
}

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using EcommerceAPI.Application.User.Commands.VerifyEmail;
using EcommerceAPI.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace EcommerceAPI.Infrastructure.Repository;
public class OTPService : IOTPService
{
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public OTPService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        this.configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateOTP(string userId, string email, string token, DateTime expiryDate)
    {
        string data = $"{userId}{email}{expiryDate:yyyy-MM-dd HH:mm:ss}{token}";

        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)))
        {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            long hashValue = BitConverter.ToInt64(hashBytes, 0);
            int otp = Math.Abs((int)(hashValue % 100000));
            SetOtpCookie(_httpContextAccessor.HttpContext, otp.ToString("D5"));
            return otp.ToString("D5");
        }
    }

    public VerifyEmailResponse ValidateOTP(string userId, string email, OtpCookieData otpData, string token)
    {
        var result = new VerifyEmailResponse();

        if (DateTime.UtcNow > otpData.ExpiresAt)
        {
            Console.WriteLine("OTP has expired.");
            result.IsValid = false;
            result.Message = "Invalid OTP code";
            return result;
        }

        string expectedOtp = GenerateOTP(userId, email, token, otpData.IssuedAt);

        result.IsValid = otpData.Otp == expectedOtp;

        if (result.IsValid)
        {
            result.UserId = userId;
            result.Message = "Sucessfully validated OTP";
        }

        return result;
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

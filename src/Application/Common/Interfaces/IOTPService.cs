﻿using Microsoft.AspNetCore.Http;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IOTPService
{
    string GenerateOTP(string userId, string email, string token, DateTime expiryDate);
    Result ValidateOTP(string userId, string email, OtpCookieData otpData, string token);
    OtpCookieData GetOtpCookieData(HttpContext context);
}

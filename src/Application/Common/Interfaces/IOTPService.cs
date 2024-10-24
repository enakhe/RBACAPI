using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Application.User.Commands.VerifyEmail;
using Microsoft.AspNetCore.Http;

namespace EcommerceAPI.Application.Common.Interfaces;
public interface IOTPService
{
    string GenerateOTP(string userId, string email, string token, DateTime expiryDate);
    VerifyEmailResponse ValidateOTP(string userId, string email, OtpCookieData otpData, string token);
    OtpCookieData GetOtpCookieData(HttpContext context);
}

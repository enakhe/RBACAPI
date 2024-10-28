using EcommerceAPI.Application.Auth.Commands.GetPasswordResetToken;
using EcommerceAPI.Application.Auth.EventHandlers;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Application.User.Commands.SendOTP;
using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Application.User.Commands.VerifyEmail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<Result> SignInAsync(string email, string password);

    Task<Result> SignUpAsync(string email, string password);

    Task<Result> SendOTPAsync(string email);

    Task<Result> VerifyEmailAsync(string email, string otp);

    Task<Result> GetPasswordResetTokenAsync(string email);

    Task<Result> RestPasswordAsync(string email, string code, string password);
}

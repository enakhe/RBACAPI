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

    Task<SignInResponse> SignInAsync(string email, string password, bool rememberMe);

    Task<SignUpResponse> SignUpAsync(string email, string password);

    Task<SendOTPResponse> SendOTP(string email);

    Task<VerifyEmailResponse> VerifyEmail(string email, string otp);
}

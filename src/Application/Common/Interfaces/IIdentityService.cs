﻿using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<AuthResult> SignInAsync(string email, string password);

    Task<AuthResult> SignUpAsync(string email, string password);

    Task<Result> SendOTPAsync(string email);

    Task<Result> VerifyEmailAsync(string email, string otp);

    Task<Result> GetPasswordResetTokenAsync(string email);

    Task<Result> ResetPasswordAsync(string email, string code, string password);

    Task<Result> LogOut(string userId);

    Task<Result> ChangePassword(string userId, string password, string confirmPassword);

    Task<Result> ChangeEmail(string userId, string email);

    string GetUserId();
}

using EcommerceAPI.Application.Common.Models;

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

    Task<Result> ResetPasswordAsync(string email, string code, string password);

    Task<Result> LogOut(string userId);

    Task<Result> ChangePassword(string userId, string password, string confirmPassword);

    Task<Result> ChangeEmail(string userId, string email);

    string GetUserId();
}

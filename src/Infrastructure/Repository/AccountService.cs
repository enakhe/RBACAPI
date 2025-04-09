using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;
using RBACAPI.Domain.Enums;
using RBACAPI.Infrastructure.Identity;

namespace RBACAPI.Infrastructure.Repository;
public class AccountService(UserManager<ApplicationUser> userManager) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    public async Task<Result> ChangeEmail(string userId, string email)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return UserNotAuthenticated();

        var code = await _userManager.GenerateChangeEmailTokenAsync(user, email);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ChangeEmailAsync(user, email, code);

        if (!result.Succeeded)
            return Result.Failure(
                "One or more validation failures have occurred",
                result.Errors.Select(e => e.Description)
            );

        var setUserNameResult = await _userManager.SetUserNameAsync(user, email);

        if (!setUserNameResult.Succeeded)
            return Result.Failure(
                "One or more validation failures have occurred",
                setUserNameResult.Errors.Select(e => e.Description)
            );

        return Result.Success(
            "Succesfully Changed Email",
            "Email change verification passed. You can now login and access your account",
            new { }
        );
    }

    public async Task<Result> ChangePassword(string userId, string password, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return UserNotAuthenticated();

        var result = await _userManager.ChangePasswordAsync(user, password, newPassword);

        if (!result.Succeeded)
            return Result.Failure(
                "One or more validation failures have occurred", 
                result.Errors.Select(e => e.Description)
            );

        await _userManager.UpdateAsync(user);

        return Result.Success(
            "Succesfully Changed Password",
            "Password change verification passed. You will be loged out of your account shortly",
            new { }
        );
    }

    public async Task<Result> ProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null
            ? UserNotAuthenticated()
            : Result.Success(
                "Profile Retrieval Successful",
                "Success! User profile has been successfully retrieved",
                new {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.FullName,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.Gender,
                    user.ProfilePicture,
                    user.LastLoginDate,
                    user.EmailConfirmed,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.LockoutEnabled,
                    user.AccessFailedCount,
                    user.LockoutEnd,
                }
            );
    }

    public async Task<Result> GenerateRecoveryCodesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return UserNotAuthenticated();

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
            return Result.Failure(
                "One or more validation failures have occurred",
                ["Cannot generate recovery codes because you do not have 2FA enabled."]
            );

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return Result.Success(
            "Recovery Codes Generated",
            "Your new 2FA recovery codes have been generated! Be sure to store them securely",
            new {
                recoveryCodes,
            }
        );
    }

    public async Task<Result> EnableAuthenticator(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return UserNotAuthenticated();

        var enable2fa = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!enable2fa.Succeeded)
            return Result.Failure(
                "One or more validation failures have occurred",
                ["Cannot enable Two Factor Authentication. Please check your account settings and try again"]
            );

        bool generateToken = await _userManager.CountRecoveryCodesAsync(user) == 0;
        if (generateToken)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return Result.Success(
                "Recovery Codes Generated",
                "Your new 2FA recovery codes have been generated! Be sure to store them securely",
                new
                {
                    recoveryCodes,
                }
            );
        }

        return Result.Success(
            "Two Factor Authentication Enabled",
            "Your authenticator has been successfully enabled. Please follow the instructions to complete the setup and ensure your account is securely protected",
            new{ }
        );
    }

    public async Task<Result> Disable2FAuthentication(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return UserNotAuthenticated();

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return Result.Failure(
                "One or more validation failures have occurred",
                ["2FA is not currently active on this account, so disabling it isn’t necessary."]
            );

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        return !disable2faResult.Succeeded
            ? Result.Failure(
                "One or more validation failures have occurred",
                disable2faResult.Errors.Select(e => e.Description)
            )
            : Result.Success(
                "Two Factor Authentication Disabled",
                "Two-Factor Authentication has been successfully disabled on your account. Your account is no longer protected by 2FA. Please ensure your security settings are updated if needed",
                new{}
            );
    }

    public async Task<Result> UpdateProfileAsync(string userId, string firstName, string lastName, IFormFile file, GenderData gender, string email, string phoneNumber)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return UserNotAuthenticated();

        if (user.FirstName != firstName)
            user.FirstName = firstName;
        
        if (user.LastName != lastName)
            user.LastName = lastName;

        if (user.Gender != gender)
            user.Gender = gender;

        if (user.Email != email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, email);
            if (!setEmailResult.Succeeded)
                return Result.Failure(
                    "Error updating email address",
                    setEmailResult.Errors.Select(e => e.Description)
                );
        }

        if (user.PhoneNumber != phoneNumber)
        {
            var setPhoneNumberResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
            if (!setPhoneNumberResult.Succeeded)
                return Result.Failure(
                    "Error updating phone number",
                    setPhoneNumberResult.Errors.Select(e => e.Description)
                );
        }

        if (file != null && file.Length > 0)
        {
            using var dataStream = new MemoryStream();
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                return Result.Failure(
                    "Invalid File Type",
                    ["The file type is not supported. Please upload an image in JPEG, PNG, or GIF format"]
                );

            await file.CopyToAsync(dataStream);

            if (dataStream.Length < 2097152)
                user.ProfilePicture = dataStream.ToArray();

            else
            {
                return Result.Failure(
                     "Profile Picture Update Failed",
                     ["Unable to update your profile picture. The image size is too large. Please upload an image that is below 2MB"]
                );
            }
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result.Failure(
                "One or more validation failures have occurred",
                result.Errors.Select(e => e.Description)
            );

        return Result.Success(
            "Profile Update Successful",
            "Success! User profile has been successfully updated",
            new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.FullName,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Gender,
                user.ProfilePicture,
                user.LastLoginDate,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.LockoutEnd,
            }
        );
    }

    public static Result UserNotAuthenticated()
    {
        return Result.Failure(
            "User Not Found",
            ["We couldn’t find your profile. Please ensure you are authenticated"]
        );
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Infrastructure.Repository;
public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AccountService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> ProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "We couldn’t find your profile. Please ensure you've entered the correct information" };
            return Result.Failure(errors);
        }

        return Result.Success(new
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
            user.EmailConfirmed
        });
    }

    public async Task<Result> GenerateRecoveryCodesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "We couldn’t find your profile. Please ensure you've entered the correct information" };
            return Result.Failure(errors);
        }

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            IEnumerable<string> errors = new List<string> { "Cannot generate recovery codes because you do not have 2FA enabled." };
            return Result.Failure(errors);
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        return Result.Success(new
        {
            recoveryCodes,
            Message = "Your new 2FA recovery codes have been generated! Be sure to store them securely"
        });
    }

    public async Task<Result> EnableAuthenticator(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "We couldn’t find your profile. Please ensure you are authenticated" };
            return Result.Failure(errors);
        }

        var enable2fa = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if(!enable2fa.Succeeded)
        {
            IEnumerable<string> errors = new List<string> { "Cannot enable Two Factor Authentication. Please check your account settings and try again" };
            return Result.Failure(errors);
        }

        bool generateToken = await _userManager.CountRecoveryCodesAsync(user) == 0;
        if (generateToken)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return Result.Success(new
            {
                recoveryCodes,
                Message = "Your new 2FA recovery codes have been generated! Be sure to store them securely"
            });
        }

        return Result.Success(new
        {
            Message = "Your account is now more secure! Two Factor Authentication has been enabled."
        });
    }

    public async Task<Result> Disable2FAuthentication(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "We couldn’t find your profile. Please ensure you are authenticated" };
            return Result.Failure(errors);
        }

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
        {
            IEnumerable<string> errors = new List<string> { "2FA is not currently active on this account, so disabling it isn’t necessary." };
            return Result.Failure(errors);
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            return Result.Failure(disable2faResult.Errors.Select(e => e.Description));
        }

        return Result.Success(new
        {
            Message = "2fa has been disabled.You can reenable 2fa when you setup an authenticator app"
        });
    }
}

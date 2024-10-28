using System.Text;
using EcommerceAPI.Application.Auth.Commands.GetPasswordResetToken;
using EcommerceAPI.Application.Auth.EventHandlers;
using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Application.User.Commands.SendOTP;
using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Application.User.Commands.VerifyEmail;
using EcommerceAPI.Infrastructure.Interface;
using EcommerceAPI.Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcommerceAPI.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IJWTService _jWTService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IOTPService _otpService;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IJWTService jwtService,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        IUserStore<ApplicationUser> userStore,
        IUserEmailStore<ApplicationUser> emailStore,
        IOTPService otpService,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _signInManager = signInManager;
        _jWTService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _userStore = userStore;
        _emailStore = emailStore;
        _otpService = otpService;
        _logger = logger;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        IEnumerable<string> data = new List<string> { "Successfully deleted user" };
        return user != null ? await DeleteUserAsync(user) : Result.Success(data);
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<Result> SignInAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "Invalid sign-in attempt. The email or password is incorrect" };
            return Result.Failure(errors);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user!, password);
        if (!passwordValid)
        {
            IEnumerable<string> errors = new List<string> { "Invalid sign-in attempt. The email or password is incorrect" };
            return Result.Failure(errors);
        }

        var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);
        return Result.Success(new
        {
            Token = token,
            Message = "User logged in successfully"
        });
    }

    public async Task<Result> SignUpAsync(string email, string password)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        if (foundUser != null)
        {
            IEnumerable<string> errors = new List<string> { "The provided email is already used" };
            return Result.Failure(errors);
        }

        var user = CreateUser();
        user.Id = Guid.NewGuid().ToString();
        user.UserName = email;

        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors.Select(e => e.Description));
        }

        var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);
        return Result.Success(new
        {
            Token = token,
            Message = "User logged in successfully"
        });
    }

    public async Task<Result> SendOTP(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "Invalid login attempt" };
            return Result.Failure(errors);
        }
            
        var userId = await _userManager.GetUserIdAsync(user);
        var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);
        var code = _otpService.GenerateOTP(userId, email, token, DateTime.UtcNow);

        return Result.Success(new
        {
            Code = code,
            Message = "Successfully sent OTP code"
        });
    }

    public async Task<Result> VerifyEmail(string email, string otp)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "Invalid login attempt" };
            return Result.Failure(errors);
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var token = _httpContextAccessor.HttpContext!.Request.Cookies["JWT"];
        var otpData = _otpService.GetOtpCookieData(_httpContextAccessor.HttpContext);
        if (otpData == null)
        {
            IEnumerable<string> errors = new List<string> { "Invalid code" };
            return Result.Failure(errors);
        }

        var verifyEmailResponse = _otpService.ValidateOTP(userId, email, otpData, token!);

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        return Result.Success(new
        {
            IsValid = true,
            UserId = userId,
            Message = "Sucessfully validated email"
        });
    }

    public async Task<Result> GetPasswordResetToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            IEnumerable<string> errors = new List<string> { "Invalid attempt" };
            return Result.Failure(errors);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Result.Success(new
        {
            Succeeded = true,
            Code = code,
            Messgae = "Sucessfully sent reset token"
        });
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}

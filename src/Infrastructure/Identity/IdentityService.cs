using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Application.User.Commands.SendOTP;
using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Application.User.Commands.VerifyEmail;
using EcommerceAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

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

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IJWTService jwtService,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        IUserStore<ApplicationUser> userStore,
        IUserEmailStore<ApplicationUser> emailStore,
        IOTPService otpService)
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

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<SignInResponse> SignInAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new SignInResponse { Succeeded = false };
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            return new SignInResponse { Succeeded = false };
        }

        var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);

        return new SignInResponse
        {
            Succeeded = true,
            UserId = user.Id,
            UserName = user.UserName,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<SignUpResponse> SignUpAsync(string email, string password)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        if (foundUser != null)
        {
            throw new InvalidOperationException("Email already exist");
        }

        var user = CreateUser();
        user.Id = Guid.NewGuid().ToString();
        user.UserName = email;


        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return new SignUpResponse { Succeeded = false };
        }

        var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);

        return new SignUpResponse
        {
            Succeeded = true,
            UserId = user.Id,
            UserName = user.UserName!,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber!,
            Email = user.Email!,
            Token = token
        };
    }

    public async Task<SendOTPResponse> SendOTP(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new KeyNotFoundException("The email you provided is not registered");

        var userId = await _userManager.GetUserIdAsync(user);
        var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);
        var code = _otpService.GenerateOTP(userId, email, token, DateTime.UtcNow);

        return new SendOTPResponse
        {
            Succeeded = true,
            UserId = userId,
            Code = code,
            Email = email
        };
    }

    public async Task<VerifyEmailResponse> VerifyEmail(string email, string otp)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new KeyNotFoundException("The email you provided is not registered");

        var userId = await _userManager.GetUserIdAsync(user);
        var token = _httpContextAccessor.HttpContext!.Request.Cookies["JWT"];
        var otpData = _otpService.GetOtpCookieData(_httpContextAccessor.HttpContext);
        if (otpData == null)
            throw new InvalidOperationException("Invalid code");

        var verifyEmailResponse = _otpService.ValidateOTP(userId, email, otpData, token!);
        if(!verifyEmailResponse.IsValid)
            throw new InvalidOperationException("Something unexpected happened");

        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        return new VerifyEmailResponse
        {
            IsValid = true,
            UserId = userId,
            Message = "Email sucessfully validated"
        };
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

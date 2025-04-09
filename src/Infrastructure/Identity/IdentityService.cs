using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;
using RBACAPI.Infrastructure.Interface;
using StackExchange.Redis;

namespace RBACAPI.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    IJWTService jwtService,
    SignInManager<ApplicationUser> signInManager,
    IHttpContextAccessor httpContextAccessor,
    IUserStore<ApplicationUser> userStore,
    IUserEmailStore<ApplicationUser> emailStore,
    IOTPService otpService,
    IConnectionMultiplexer redis) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IJWTService _jWTService = jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserStore<ApplicationUser> _userStore = userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore = emailStore;
    private readonly IOTPService _otpService = otpService;
    private readonly IDatabase _redisDb = redis.GetDatabase();

    private static string GetRedisKey(string userId) => userId;

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
        return user != null ? await DeleteUserAsync(user) : Result.Success("User Account Deleted", "", data);
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public string GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return userId!;
    }

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
            return AuthResult.Failure(
                "One or more validation failures have occurred",
                ["Invalid sign-in attempt. The email or password is incorrect"]
            );

        SaveUserToRedisCacheAsync(user);

        if (!user.EmailConfirmed)
            return AuthResult.Failure(
                "One or more validation failures have occurred", 
                ["Invalid sign-in attempt. Your email isn't verified. Kindly verify your email address"]
            );

        var accessToken = _jWTService.GenerateToken(user, DateTimeOffset.UtcNow.AddMinutes(30));
        var refreshToken = _jWTService.GenerateToken(user, DateTimeOffset.UtcNow.AddDays(7));

        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return AuthResult.Success(
            "Login Successful",
            "You have successfully logged in. Welcome back!",
            new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        );
    }


    public async Task<Result> SignUpAsync(string email, string password)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        if (foundUser != null)
            return Result.Failure(
                "Invalid credentials. Please check your email and password", 
                ["The provided email is already used"]
            );

        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return Result.Failure(
                "Invalid credentials. Please check your email and password", 
                result.Errors.Select(e => e.Description)
            );

        SaveUserToRedisCacheAsync(user);

        return Result.Success(
            "Account Created Sucessfully",
            "Welcome aboard! Your account has been created successfully",
            new { }
        );
    }

    public async Task<Result> SendOTPAsync(string email)
    {
        static Result FailedSendOTPResult() =>
            Result.Failure(
                "Unable to generate and send otp",
                ["Unable to send OTP. Please check the provided email address and try again."]
            );

        var user = await _userManager.FindByEmailAsync(email.Trim().ToLowerInvariant());
        if (user == null)
            return FailedSendOTPResult();

        var cachedUser = await GetUserFromRedisCacheAsync(user.Id);

        if (cachedUser == null || cachedUser.Id != user.Id)
            return FailedSendOTPResult();

        if (cachedUser.EmailConfirmed)
            return Result.Failure(
                "Unable to generate and send otp",
                ["Unable to send OTP. Email already verified"]
            );

        var otpToken = _jWTService.GenerateToken(user, DateTimeOffset.UtcNow.AddMinutes(5));
        var code = _otpService.GenerateOTPAsync(cachedUser.Id, email.Trim().ToLowerInvariant(), otpToken, DateTimeOffset.UtcNow);

        return Result.Success(
            "OTP Generated Sucessfully",
            "Great news! Your one-time password (OTP) has been generated and sent to your email inbox",
            new {
                code = code.Result,
            }
        );
    }

    public async Task<Result> SendEmailConfirmationTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(
                "Unable to generate and send token",
                ["Unable to send token. Please check the provided email address and try again."]
            );

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Result.Success(
            "Token Generated Sucessfully",
            "Great news! Your one-time password (OTP) has been generated and sent to your email inbox",
            new {
                code,
            }
        );
    }

    public async Task<Result> VerifyEmailAsync(string email, string otp)
    {
        static Result FailedVerifyemailResult() =>
            Result.Failure(
                "One or more validation failures have occurred",
                ["Unable tovery email. Please check the provided email address or code and try again."]
            );

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return FailedVerifyemailResult();

        var cachedUser = await GetUserFromRedisCacheAsync(user.Id);
        if (cachedUser == null || cachedUser.Id != user.Id)
            return FailedVerifyemailResult();

        var verifyEmailResponse = await _otpService.ValidateOTPAsync(email, otp);

        if (!verifyEmailResponse.Succeeded)
            return Result.Failure(
                "One or more validation failures have occurred",
                verifyEmailResponse.Errors
            );

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        return Result.Success(
            "Sucessfully Verified Email",
            "Email verification passed. You can now login and access your account",
            new {}
        );
    }

    public async Task<Result> GetPasswordResetTokenAsync(string email)
    {
        static Result FailedResetTokenResult() =>
            Result.Failure(
                "One or more validation failures have occurred",
                ["Unable to send password reset token. Please check the provided email address or check if it verified and try again."]
            );

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            return FailedResetTokenResult();

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Result.Success(
            "Reset Token Generated Sucessfully",
            "Great news! Your reset token has been generated and sent to your email inbox",
            new {
                code
            }
        );
    }

    public async Task<Result> ResetPasswordAsync(string email, string code, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            Result.Failure(
                "One or more validation failures have occurred",
                ["Unable to reset password. Please check the provided email address or passwords and try again."]
            );

        var decodeCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ResetPasswordAsync(user!, decodeCode, password);

        return !result.Succeeded
            ? Result.Failure(
                "One or more validation failures have occurred",
                result.Errors.Select(e => e.Description))
            : Result.Success(
                "Sucessfully Reset Password",
                "Password reset verification passed. You can now login and access your account",
                new {}
            );
    }

    public async Task<Result> LogOut(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(
                "One or more validation failures have occurred",
                ["We couldn’t find your profile. Please ensure you are authenticated"]
            );

        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user!);

        _httpContextAccessor!.HttpContext!.Response.Cookies.Delete("Auth.JWT.AccessToken");
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("Auth.JWT.RefreshToken");

        return Result.Success(
            "Succesfully Logged Out User",
            "You have been logged out successfully",
            new {}
        );
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

    private async void SaveUserToRedisCacheAsync(ApplicationUser user)
    {
        var serializedUser = JsonConvert.SerializeObject(user);
        await _redisDb.StringSetAsync(GetRedisKey(user.Id), serializedUser, TimeSpan.FromDays(7));
    }

    public async Task<ApplicationUser?> GetUserFromRedisCacheAsync(string userId)
    {
        var userData = await _redisDb.StringGetAsync(userId);
        return !userData.IsNullOrEmpty ? JsonConvert.DeserializeObject<ApplicationUser>(userData!) : null;
    }
}

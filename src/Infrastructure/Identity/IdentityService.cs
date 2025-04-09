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
    private readonly IDatabase _redisDb;

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
        IConnectionMultiplexer redis)
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
        _redisDb = redis.GetDatabase();
    }

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
        return user != null ? await DeleteUserAsync(user) : Result.Success(data);
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
            return AuthResult.Failure(["Invalid sign-in attempt. The email or password is incorrect"]);

        SaveUserToRedisCacheAsync(user);

        if (!user.EmailConfirmed)
            return AuthResult.Failure(["Invalid sign-in attempt. Your email isn't verified. Kindly verify your email address"]);

        var accessToken = _jWTService.GenerateToken(user, DateTimeOffset.UtcNow.AddMinutes(30));
        var refreshToken = _jWTService.GenerateToken(user, DateTimeOffset.UtcNow.AddDays(7));

        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return AuthResult.Success(accessToken, refreshToken);
    }


    public async Task<Result> SignUpAsync(string email, string password)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        if (foundUser != null)
            return Result.Failure(new List<string> { "The provided email is already used" });

        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        SaveUserToRedisCacheAsync(user);

        return Result.Success(new
        {
            title = "Account created sucessfully",
            message = "Welcome aboard! Your account has been created successfully",
        });
    }

    public async Task<Result> SendOTPAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim().ToLowerInvariant());
        if (user == null)
            return Result.Failure(["Unable to send OTP. Please check the provided email address and try again."]);

        var cachedUser = await GetUserFromRedisCacheAsync(user.Id);

        if (cachedUser == null || cachedUser.Id != user.Id)
            return Result.Failure(["Unable to send OTP. Please check the provided email address and try again."]);

        var otpToken = _jWTService.GenerateToken(user, DateTimeOffset.UtcNow.AddMinutes(5));
        var code = _otpService.GenerateOTPAsync(cachedUser.Id, email.Trim().ToLowerInvariant(), otpToken, DateTimeOffset.UtcNow);

        return Result.Success(new
        {
            message = "Great news! Your one-time password (OTP) has been generated and sent to your email inbox. " +
                      "Please check your email (and spam folder, if necessary) and use the OTP to verify your identity.",
            code = code.Result,
        });
    }

    public async Task<Result> SendToken(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure(["Unable to send token. Please check the provided email address and try again."]);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Result.Success(new
        {
            code,
            Message = "Succesfully send token"
        });
    }

    public async Task<Result> VerifyEmailAsync(string email, string otp)
    {
        static Result FailedVerifyemailResult() =>
            Result.Failure(["Unable tovery email. Please check the provided email address or code and try again."]);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return FailedVerifyemailResult();

        var cachedUser = await GetUserFromRedisCacheAsync(user.Id);
        if (cachedUser == null || cachedUser.Id != user.Id)
            return FailedVerifyemailResult();

        var verifyEmailResponse = await _otpService.ValidateOTPAsync(email, otp);
        if (!verifyEmailResponse.Succeeded)
        {
            return Result.Failure(verifyEmailResponse.Errors);
        }

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        return Result.Success(new
        {
            message = "Sucessfully validated email"
        });
    }

    public async Task<Result> GetPasswordResetTokenAsync(string email)
    {
        static Result FailedResetTokenResult() =>
            Result.Failure(["Unable to send password reset token. Please check the provided email address and try again."]);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            return FailedResetTokenResult();

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Result.Success(new
        {
            code,
            message = "Successfully sent reset token"
        });
    }

    public async Task<Result> ResetPasswordAsync(string email, string code, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            Result.Failure(["Unable to reset password. Please check the provided email address or passwords and try again."]);

        var decodeCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ResetPasswordAsync(user!, decodeCode, password);

        return !result.Succeeded
            ? Result.Failure(result.Errors.Select(e => e.Description))
            : Result.Success(new
            {
                message = "Succesfully reset password, kindly login"
            });
    }

    public async Task<Result> ChangeEmail(string userId, string email)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "We couldn’t find your profile. Please ensure you are authenticated" };
            return Result.Failure(errors);
        }

        var code = await _userManager.GenerateChangeEmailTokenAsync(user, email);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ChangeEmailAsync(user, email, code);
        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors.Select(e => e.Description));
        }

        var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
        if (!setUserNameResult.Succeeded)
        {
            return Result.Failure(setUserNameResult.Errors.Select(e => e.Description));
        }

        return Result.Success(new
        {
            Message = "Succesfully changed email, kindly login"
        });
    }

    public async Task<Result> ChangePassword(string userId, string password, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "Invalid attempt" };
            return Result.Failure(errors);
        }

        var result = await _userManager.ChangePasswordAsync(user, password, newPassword);
        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors.Select(e => e.Description));
        }

        await _userManager.UpdateAsync(user);
        return Result.Success(new
        {
            Message = "Succesfully changed password, you will be logged out shortly to reauthenticate"
        });
    }

    public async Task<Result> LogOut(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.UpdateAsync(user!);
        _httpContextAccessor!.HttpContext!.Response.Cookies.Delete("Auth.JWT.AccessToken");
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("Auth.JWT.RefreshToken");

        return Result.Success(new
        {
            Message = "Succesfully logged out user"
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

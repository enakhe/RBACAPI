using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.OAuth.Commands.GoogleSignIn;
using EcommerceAPI.Infrastructure.Identity;
using EcommerceAPI.Application.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using EcommerceAPI.Infrastructure.Utils;
using EcommerceAPI.Domain.Common;
using EcommerceAPI.Domain.Enums;
using EcommerceAPI.Infrastructure.Data;
using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
using Newtonsoft.Json;
using System.Net.Http;

namespace EcommerceAPI.Infrastructure.Repository;
public class OAuthService : IOAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration configuration;
    private readonly IJWTService _jWTService;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OAuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration, IJWTService jWTService, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _context = context;
        _jWTService = jWTService;
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClientFactory.CreateClient("Facebook");
        this.configuration = configuration;
    }

    public async Task<GoogleSignInResponse> GoogleSignIn(string code)
    {
        Payload payload = new();

        try
        {
            payload = await ValidateAsync(code, new ValidationSettings
            {
                Audience = new[] { configuration["Authentication:Google:ClientId"] }
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get a response, {ex.Message}");
        }

        var userToBeCreated = new CreateUserFromSocialLogin
        {
            Email = payload.Email,
            LoginProviderSubject = payload.Subject,
        };

        var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Google);

        if (user is not null)
        {
            var token = _jWTService.GenerateToken(_httpContextAccessor.HttpContext!, user, "AccessToken", DateTimeOffset.UtcNow.AddMinutes(30));
            return new GoogleSignInResponse
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
        return new GoogleSignInResponse { Succeeded = false };
    }

    public async Task<BaseResponse<FacebookTokenValidationResponse>> ValidateFacebookToken(string accessToken)
    {
        try
        {
            string TokenValidationUrl = configuration["Authentication:Facebook:TokenValidationUrl"]!;
            var url = string.Format(TokenValidationUrl, accessToken, configuration["Authentication:Facebook:AppId"]!, configuration["Authentication:Facebook:AppSecret"]!);
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();

                var tokenValidationResponse = JsonConvert.DeserializeObject<FacebookTokenValidationResponse>(responseAsString);
                return new BaseResponse<FacebookTokenValidationResponse>(tokenValidationResponse!);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get a response, {ex.Message}");
        }

        return new BaseResponse<FacebookTokenValidationResponse>("Failed to get response");

    }

    public async Task<BaseResponse<FacebookUserInfoResponse>> GetFacebookUserInformation(string accessToken)
    {
        try
        {
            string userInfoUrl = configuration["Authentication:Facebook:UserInfoUrl"]!;
            string url = string.Format(userInfoUrl, accessToken);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var userInfoResponse = JsonConvert.DeserializeObject<FacebookUserInfoResponse>(responseAsString);
                return new BaseResponse<FacebookUserInfoResponse>(userInfoResponse!);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get a response, {ex.Message}");
        }

        return new BaseResponse<FacebookUserInfoResponse>("Failed to get response");

    }

    public async Task<BaseResponse<FacebookSignInResponse>> FacebookSignIn(string accessToken)
    {
        var validatedFbToken = await ValidateFacebookToken(accessToken);

        if (validatedFbToken.Errors.Any())
            return new BaseResponse<FacebookSignInResponse>(validatedFbToken.ResponseMessage, validatedFbToken.Errors);

        var userInfo = await GetFacebookUserInformation(accessToken);

        if (userInfo.Errors.Any())
            return new BaseResponse<FacebookSignInResponse>(null, userInfo.Errors);

        var userToBeCreated = new CreateUserFromSocialLogin
        {
            Email = userInfo.Data.Email,
            LoginProviderSubject = userInfo.Data.Id,
        };

        var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Facebook);

        if (user is not null)
        {
            var token = _jWTService.GenerateToken(_httpContextAccessor.HttpContext!, user, "AccessToken", DateTimeOffset.UtcNow.AddMinutes(30));
            var data = new FacebookSignInResponse
            {
                Succeeded = true,
                UserId = user.Id,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = token
            };

            return new BaseResponse<FacebookSignInResponse>(data);
        }

        var failed = new FacebookSignInResponse
        {
            Succeeded = false,
            Errors = userInfo.Errors
        };
        return new BaseResponse<FacebookSignInResponse>(failed);

    }
}

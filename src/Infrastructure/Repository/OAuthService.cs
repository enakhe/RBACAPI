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
using EcommerceAPI.Domain.Enums;
using EcommerceAPI.Infrastructure.Data;
using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using EcommerceAPI.Application.User.Commands.SignUp;

namespace EcommerceAPI.Infrastructure.Repository;
public class OAuthService : IOAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration configuration;
    private readonly IJWTService _jWTService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OAuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration, IJWTService jWTService, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _context = context;
        _jWTService = jWTService;
        _httpContextAccessor = httpContextAccessor;
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
            var token = _jWTService.GenerateJWTToken(_httpContextAccessor.HttpContext!, user);
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
}

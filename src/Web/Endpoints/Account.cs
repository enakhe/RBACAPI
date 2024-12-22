using EcommerceAPI.Application.Account.Commands.ChangeEmail;
using EcommerceAPI.Application.Account.Commands.Disable2FAuthentication;
using EcommerceAPI.Application.Account.Commands.EnableAuthenticator;
using EcommerceAPI.Application.Account.Commands.GenerateRecoveryCodes;
using EcommerceAPI.Application.Account.Queries.Profile;
using EcommerceAPI.Application.Auth.Commands.ChangePassword;
using EcommerceAPI.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Endpoints;

[AuthorizeUser]
public class Account : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(UserProfile, "profile")
            .MapPost(EnableAuthenticator, "enable-2fa")
            .MapPost(Disable2FAuthentication, "disable-2fa")
            .MapPost(GenerateRecoveryCodes, "generate-recovery-codes")
            .MapPost(ChangePassword, "change-password")
            .MapPost(ChangeEmail, "change-email");
    }
 
    public Task<IActionResult> UserProfile(ISender sender)
    {
        return sender.Send(new ProfileQuery());
    }

    public Task<IActionResult> GenerateRecoveryCodes(ISender sender)
    {
        return sender.Send(new GenerateRecoveryCodesCommand());
    }

    public Task<IActionResult> ChangeEmail(ISender sender, ChangeEmailCommand command)
    {
        return sender.Send(command);
    }

    public Task<IActionResult> ChangePassword(ISender sender, ChangePasswordCommand command)
    {
        return sender.Send(command);
    }

    public Task<IActionResult> EnableAuthenticator(ISender sender)
    {
        return sender.Send(new EnableAuthenticatorCommand());
    }

    public Task<IActionResult> Disable2FAuthentication(ISender sender)
    {
        return sender.Send(new Disable2FAuthenticationCommand());
    }
}



using RBACAPI.Application.Account.Commands.ChangeEmail;
using RBACAPI.Application.Account.Commands.Disable2FAuthentication;
using RBACAPI.Application.Account.Commands.EnableAuthenticator;
using RBACAPI.Application.Account.Commands.GenerateRecoveryCodes;
using RBACAPI.Application.Account.Queries.Profile;
using RBACAPI.Application.Auth.Commands.ChangePassword;
using RBACAPI.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace RBACAPI.Web.Endpoints;

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



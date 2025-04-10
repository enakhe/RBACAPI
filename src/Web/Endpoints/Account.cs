using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RBACAPI.Application.Account.Commands.ChangeEmail;
using RBACAPI.Application.Account.Commands.ChangePassword;
using RBACAPI.Application.Account.Commands.Disable2FAuthentication;
using RBACAPI.Application.Account.Commands.EnableAuthenticator;
using RBACAPI.Application.Account.Commands.GenerateRecoveryCodes;
using RBACAPI.Application.Account.Commands.UpdateProfile;
using RBACAPI.Application.Account.Queries.Profile;
using RBACAPI.Application.Common.Security;

namespace RBACAPI.WebAPI.Endpoints;

[AuthorizeUser]
public class Account : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup("/account")
            .WithTags("Account")
            .MapGet(UserProfile, "/profile")
            .MapPost(EnableAuthenticator, "/enable-2fa")
            .MapPost(Disable2FAuthentication, "/disable-2fa")
            .MapPost(GenerateRecoveryCodes, "/generate-recovery-codes")
            .MapPost(ChangePassword, "/change-password")
            .MapPost(ChangeEmail, "/change-email")
            .MapPost(UpdateProfile, "/update-profile");
    }



    [OutputCache]
    [AuthorizeUser]
    public Task<IActionResult> UserProfile(ISender sender)
    {
        return sender.Send(new ProfileQuery());
    }

    [AuthorizeUser]
    public Task<IActionResult> GenerateRecoveryCodes(ISender sender)
    {
        return sender.Send(new GenerateRecoveryCodesCommand());
    }

    [AuthorizeUser]
    public Task<IActionResult> ChangeEmail(ISender sender, ChangeEmailCommand command)
    {
        return sender.Send(command);
    }

    [AuthorizeUser]
    public Task<IActionResult> ChangePassword(ISender sender, ChangePasswordCommand command)
    {
        return sender.Send(command);
    }

    [AuthorizeUser]
    public Task<IActionResult> EnableAuthenticator(ISender sender)
    {
        return sender.Send(new EnableAuthenticatorCommand());
    }

    [AuthorizeUser]
    public Task<IActionResult> Disable2FAuthentication(ISender sender)
    {
        return sender.Send(new Disable2FAuthenticationCommand());
    }

    [AuthorizeUser]
    [IgnoreAntiforgeryToken]
    [Consumes("multipart/form-data")]
    public Task<IActionResult> UpdateProfile(ISender sender, [FromForm] UpdateProfileCommand command)
    {
        return sender.Send(command);
    }
}

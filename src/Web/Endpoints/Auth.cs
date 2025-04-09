using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RBACAPI.Application.Auth.Commands.GetPasswordResetToken;
using RBACAPI.Application.Auth.Commands.LogOut;
using RBACAPI.Application.Auth.Commands.ResetPassword;
using RBACAPI.Application.Auth.Commands.SendOTP;
using RBACAPI.Application.Auth.Commands.SignIn;
using RBACAPI.Application.Common.Security;
using RBACAPI.Application.User.Commands.SignUp;
using RBACAPI.Application.User.Commands.VerifyEmail;

namespace RBACAPI.WebAPI.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignIn, "signin")
            .MapPost(SignUp, "signup")
            .MapPost(SendOTP, "send-otp")
            .MapPost(VerifyEmail, "verify-email")
            .MapPost(GetPasswordRestToken, "password-reset-token")
            .MapPost(ResetPassword, "reset-password")
            .MapPost(LogOut, "logout");
    }

    [OutputCache]
    public Task<ActionResult> SignIn(ISender send, SignInCommand command)
    {
        return send.Send(command);
    }

    public Task<ActionResult> SignUp(ISender send, SignUpCommand command)
    {
        return send.Send(command);
    }

    [AuthorizeUser]
    public Task<ActionResult> SendOTP(ISender send, SendOTPCommand command)
    {
        return send.Send(command);
    }

    [AuthorizeUser]
    public Task<IActionResult> VerifyEmail(ISender sender, VerifyEmailCommand command)
    {
        return sender.Send(command);
    }

    public Task<IActionResult> GetPasswordRestToken(ISender sender, GetPasswordResetTokenCommand command)
    {
        return sender.Send(command);
    }

    public Task<IActionResult> ResetPassword(ISender sender, ResetPasswordCommand command)
    {
        return sender.Send(command);
    }

    [AuthorizeUser]
    public Task<IActionResult> LogOut(ISender sender)
    {
        return sender.Send(new LogOutCommand());
    }
}

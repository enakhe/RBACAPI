using EcommerceAPI.Application.Common.Security;
using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Application.User.Commands.SendOTP;
using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Application.User.Commands.VerifyEmail;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignIn, "signin")
            .MapPost(SignUp, "signup")
            .MapPost(SendOTP, "send-otp")
            .MapPost(VerifyEmail, "verify-email");
    }

    public Task<IActionResult> SignIn(ISender send, SignInCommand command)
    {
        return send.Send(command);
    }

    public Task<IActionResult> SignUp(ISender send, SignUpCommand command)
    {
        return send.Send(command);
    }

    [AuthorizeUser]
    public Task<IActionResult> SendOTP(ISender send, SendOTPCommand command)
    {
        return send.Send(command);
    }

    [AuthorizeUser]
    public Task<IActionResult> VerifyEmail(ISender sender, VerifyEmailCommand comand)
    {
        return sender.Send(comand);
    }
}

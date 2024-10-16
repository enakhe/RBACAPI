using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Application.User.Commands.SignUp;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignIn, "signin")
            .MapPost(SignUp, "signup");
    }

    public Task<IActionResult> SignIn(ISender send, SignInCommand command)
    {
        return send.Send(command);
    }

    public Task<IActionResult> SignUp(ISender send, SignUpCommand command)
    {
        return send.Send(command);
    }
}

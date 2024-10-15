using EcommerceAPI.Application.User.Commands.Login;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login);
    }

    public Task<IActionResult> Login(ISender send, SignInCommand command)
    {
        return send.Send(command);
    }
}

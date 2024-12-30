
using EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
using EcommerceAPI.Application.OAuth.Commands.GoogleSignIn;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Endpoints;

public class OAuth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GoogleSignIn, "google")
            .MapPost(FacebookSignIn, "facebook");
    }

    public Task<IActionResult> GoogleSignIn(ISender send, GoogleSignInCommand command)
    {
        return send.Send(command);
    }

    public Task<IActionResult> FacebookSignIn(ISender send, FacebookSignInCommand command)
    {
        return send.Send(command);
    }
}

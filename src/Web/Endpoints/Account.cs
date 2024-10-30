using EcommerceAPI.Application.Account.Queries.Profile;
using EcommerceAPI.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Web.Endpoints;

public class Account : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(UserProfile, "profile"); 
    }

    [AuthorizeUser]
    public Task<IActionResult> UserProfile(ISender sender)
    {
        return sender.Send(new ProfileQuery());
    }
}



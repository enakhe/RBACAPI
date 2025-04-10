using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Models;
using RBACAPI.Application.Common.Security;
using RBACAPI.Application.Role.Commands.CreateRole;

namespace RBACAPI.Web.Endpoints;

[AuthorizeUser]
public class Role : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup("/role")
            .WithTags("Role")
            .MapPost(CreateRole, "create");
    }

    [AuthorizeUser]
    public Task<IActionResult> CreateRole(ISender sender, CreateRoleCommand command)
    {
        return sender.Send(command);
    }
}

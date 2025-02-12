using RBACAPI.Application.Common.Models;
using RBACAPI.Application.Role.Commands.CreateRole;
namespace RBACAPI.Web.Endpoints;

public class Role : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateRole);
    }

    public Task<Result> CreateRole(ISender sender, CreateRoleCommand command)
    {
        return sender.Send(command);
    }
}

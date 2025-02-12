<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Models;
using RBACAPI.Application.Common.Security;
using RBACAPI.Application.TodoItems.Commands.CreateTodoItem;
using RBACAPI.Application.TodoItems.Commands.DeleteTodoItem;
using RBACAPI.Application.TodoItems.Commands.UpdateTodoItem;
using RBACAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
using RBACAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace RBACAPI.Web.Endpoints;
=======
﻿using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Application.Common.Security;
using EcommerceAPI.Application.TodoItems.Commands.CreateTodoItem;
using EcommerceAPI.Application.TodoItems.Commands.DeleteTodoItem;
using EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItem;
using EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
using EcommerceAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace EcommerceAPI.Web.Endpoints;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoItemsWithPagination)
            .MapPost(CreateTodoItem)
            .MapPut(UpdateTodoItem, "{id}")
            .MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
            .MapDelete(DeleteTodoItem, "{id}");
    }

    public Task<PaginatedList<TodoItemBriefDto>> GetTodoItemsWithPagination(ISender sender, [AsParameters] GetTodoItemsWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public Task<int> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateTodoItem(ISender sender, int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> UpdateTodoItemDetail(ISender sender, int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoItem(ISender sender, int id)
    {
        await sender.Send(new DeleteTodoItemCommand(id));
        return Results.NoContent();
    }
}

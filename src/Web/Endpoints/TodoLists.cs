<<<<<<< HEAD
﻿using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Application.TodoLists.Commands.DeleteTodoList;
using RBACAPI.Application.TodoLists.Commands.UpdateTodoList;
using RBACAPI.Application.TodoLists.Queries.GetTodos;

namespace RBACAPI.Web.Endpoints;
=======
﻿using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Application.TodoLists.Commands.DeleteTodoList;
using EcommerceAPI.Application.TodoLists.Commands.UpdateTodoList;
using EcommerceAPI.Application.TodoLists.Queries.GetTodos;

namespace EcommerceAPI.Web.Endpoints;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetTodoLists)
            .MapPost(CreateTodoList)
            .MapPut(UpdateTodoList, "{id}")
            .MapDelete(DeleteTodoList, "{id}");
    }

    public Task<TodosVm> GetTodoLists(ISender sender)
    {
        return sender.Send(new GetTodosQuery());
    }

    public Task<int> CreateTodoList(ISender sender, CreateTodoListCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateTodoList(ISender sender, int id, UpdateTodoListCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoList(ISender sender, int id)
    {
        await sender.Send(new DeleteTodoListCommand(id));
        return Results.NoContent();
    }
}

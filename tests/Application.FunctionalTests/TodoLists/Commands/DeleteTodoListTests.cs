<<<<<<< HEAD
﻿using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Application.TodoLists.Commands.DeleteTodoList;
using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.FunctionalTests.TodoLists.Commands;
=======
﻿using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Application.TodoLists.Commands.DeleteTodoList;
using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.FunctionalTests.TodoLists.Commands;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

using static Testing;

public class DeleteTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new DeleteTodoListCommand(99);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoList()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await SendAsync(new DeleteTodoListCommand(listId));

        var list = await FindAsync<TodoList>(listId);

        list.Should().BeNull();
    }
}

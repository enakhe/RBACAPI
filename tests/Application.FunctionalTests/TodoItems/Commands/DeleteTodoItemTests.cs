<<<<<<< HEAD
﻿using RBACAPI.Application.TodoItems.Commands.CreateTodoItem;
using RBACAPI.Application.TodoItems.Commands.DeleteTodoItem;
using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.FunctionalTests.TodoItems.Commands;
=======
﻿using EcommerceAPI.Application.TodoItems.Commands.CreateTodoItem;
using EcommerceAPI.Application.TodoItems.Commands.DeleteTodoItem;
using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.FunctionalTests.TodoItems.Commands;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

using static Testing;

public class DeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(99);

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        await SendAsync(new DeleteTodoItemCommand(itemId));

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().BeNull();
    }
}

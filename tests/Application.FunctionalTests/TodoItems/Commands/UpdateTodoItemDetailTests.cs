<<<<<<< HEAD
﻿using RBACAPI.Application.TodoItems.Commands.CreateTodoItem;
using RBACAPI.Application.TodoItems.Commands.UpdateTodoItem;
using RBACAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Domain.Entities;
using RBACAPI.Domain.Enums;

namespace RBACAPI.Application.FunctionalTests.TodoItems.Commands;
=======
﻿using EcommerceAPI.Application.TodoItems.Commands.CreateTodoItem;
using EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItem;
using EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Domain.Entities;
using EcommerceAPI.Domain.Enums;

namespace EcommerceAPI.Application.FunctionalTests.TodoItems.Commands;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

using static Testing;

public class UpdateTodoItemDetailTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Note = "This is the note.",
            Priority = PriorityLevel.High
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Note.Should().Be(command.Note);
        item.Priority.Should().Be(command.Priority);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Exceptions;
using RBACAPI.Application.TodoItems.Commands.CreateTodoItem;
using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.FunctionalTests.TodoItems.Commands;
=======
﻿using EcommerceAPI.Application.Common.Exceptions;
using EcommerceAPI.Application.TodoItems.Commands.CreateTodoItem;
using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.FunctionalTests.TodoItems.Commands;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

using static Testing;

public class CreateTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoItemCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var command = new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Title.Should().Be(command.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

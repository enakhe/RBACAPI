<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Exceptions;
using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Application.TodoLists.Commands.UpdateTodoList;
using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.FunctionalTests.TodoLists.Commands;
=======
﻿using EcommerceAPI.Application.Common.Exceptions;
using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Application.TodoLists.Commands.UpdateTodoList;
using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.FunctionalTests.TodoLists.Commands;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

using static Testing;

public class UpdateTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new UpdateTodoListCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "Other List"
        });

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = "Other List"
        };

        (await FluentActions.Invoking(() =>
            SendAsync(command))
                .Should().ThrowAsync<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title")))
                .And.Errors["Title"].Should().Contain("'Title' must be unique.");
    }

    [Test]
    public async Task ShouldUpdateTodoList()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = "Updated List Title"
        };

        await SendAsync(command);

        var list = await FindAsync<TodoList>(listId);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.LastModifiedBy.Should().NotBeNull();
        list.LastModifiedBy.Should().Be(userId);
        list.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

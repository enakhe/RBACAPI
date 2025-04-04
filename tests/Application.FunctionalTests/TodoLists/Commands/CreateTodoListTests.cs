<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Exceptions;
using RBACAPI.Application.TodoLists.Commands.CreateTodoList;
using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.FunctionalTests.TodoLists.Commands;
=======
﻿using EcommerceAPI.Application.Common.Exceptions;
using EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.FunctionalTests.TodoLists.Commands;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

using static Testing;

public class CreateTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoListCommand();
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        await SendAsync(new CreateTodoListCommand
        {
            Title = "Shopping"
        });

        var command = new CreateTodoListCommand
        {
            Title = "Shopping"
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTodoList()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateTodoListCommand
        {
            Title = "Tasks"
        };

        var id = await SendAsync(command);

        var list = await FindAsync<TodoList>(id);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.CreatedBy.Should().Be(userId);
        list.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}

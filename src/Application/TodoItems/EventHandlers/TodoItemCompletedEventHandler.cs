<<<<<<< HEAD
﻿using RBACAPI.Domain.Events;
using Microsoft.Extensions.Logging;

namespace RBACAPI.Application.TodoItems.EventHandlers;
=======
﻿using EcommerceAPI.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EcommerceAPI.Application.TodoItems.EventHandlers;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoItemCompletedEventHandler : INotificationHandler<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
<<<<<<< HEAD
        _logger.LogInformation("RBACAPI Domain Event: {DomainEvent}", notification.GetType().Name);
=======
        _logger.LogInformation("EcommerceAPI Domain Event: {DomainEvent}", notification.GetType().Name);
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

        return Task.CompletedTask;
    }
}

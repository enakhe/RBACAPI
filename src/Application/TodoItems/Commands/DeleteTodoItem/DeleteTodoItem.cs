<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Domain.Events;

namespace RBACAPI.Application.TodoItems.Commands.DeleteTodoItem;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Domain.Events;

namespace EcommerceAPI.Application.TodoItems.Commands.DeleteTodoItem;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record DeleteTodoItemCommand(int Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.TodoItems.Remove(entity);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }

}

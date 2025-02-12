<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.TodoLists.Commands.DeleteTodoList;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;

namespace EcommerceAPI.Application.TodoLists.Commands.DeleteTodoList;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record DeleteTodoListCommand(int Id) : IRequest;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.TodoLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

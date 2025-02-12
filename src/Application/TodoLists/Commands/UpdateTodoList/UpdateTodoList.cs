<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.TodoLists.Commands.UpdateTodoList;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;

namespace EcommerceAPI.Application.TodoLists.Commands.UpdateTodoList;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record UpdateTodoListCommand : IRequest
{
    public int Id { get; init; }

    public string? Title { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;

        await _context.SaveChangesAsync(cancellationToken);

    }
}

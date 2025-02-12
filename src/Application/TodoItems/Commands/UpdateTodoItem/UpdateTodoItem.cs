<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.TodoItems.Commands.UpdateTodoItem;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;

namespace EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItem;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record UpdateTodoItemCommand : IRequest
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Done = request.Done;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

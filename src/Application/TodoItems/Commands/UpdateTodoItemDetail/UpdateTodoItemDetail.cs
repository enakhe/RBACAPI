<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Domain.Enums;

namespace RBACAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Domain.Enums;

namespace EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record UpdateTodoItemDetailCommand : IRequest
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public PriorityLevel Priority { get; init; }

    public string? Note { get; init; }
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoItemDetailCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

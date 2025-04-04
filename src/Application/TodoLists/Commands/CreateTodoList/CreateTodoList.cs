<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.TodoLists.Commands.CreateTodoList;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.TodoLists.Commands.CreateTodoList;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record CreateTodoListCommand : IRequest<int>
{
    public string? Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

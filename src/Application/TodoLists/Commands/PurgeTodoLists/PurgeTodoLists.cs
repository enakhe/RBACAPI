<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Security;
using RBACAPI.Domain.Constants;

namespace RBACAPI.Application.TodoLists.Commands.PurgeTodoLists;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Security;
using EcommerceAPI.Domain.Constants;

namespace EcommerceAPI.Application.TodoLists.Commands.PurgeTodoLists;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

[Authorize(Roles = Roles.Administrator)]
[Authorize(Policy = Policies.CanPurge)]
public record PurgeTodoListsCommand : IRequest;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeTodoListsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        _context.TodoLists.RemoveRange(_context.TodoLists);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

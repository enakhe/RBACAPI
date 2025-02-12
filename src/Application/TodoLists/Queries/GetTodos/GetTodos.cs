<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;
using RBACAPI.Application.Common.Security;
using RBACAPI.Domain.Enums;

namespace RBACAPI.Application.TodoLists.Queries.GetTodos;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Application.Common.Security;
using EcommerceAPI.Domain.Enums;

namespace EcommerceAPI.Application.TodoLists.Queries.GetTodos;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

[Authorize]
public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),

            Lists = await _context.TodoLists
                .AsNoTracking()
                .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };
    }
}

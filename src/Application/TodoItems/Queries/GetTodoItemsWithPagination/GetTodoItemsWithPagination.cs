<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Mappings;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;
=======
﻿using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Mappings;
using EcommerceAPI.Application.Common.Models;

namespace EcommerceAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.TodoItems
            .Where(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

<<<<<<< HEAD
﻿using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.TodoLists.Queries.GetTodos;
=======
﻿using EcommerceAPI.Application.Common.Models;

namespace EcommerceAPI.Application.TodoLists.Queries.GetTodos;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodosVm
{
    public IReadOnlyCollection<LookupDto> PriorityLevels { get; init; } = Array.Empty<LookupDto>();

    public IReadOnlyCollection<TodoListDto> Lists { get; init; } = Array.Empty<TodoListDto>();
}

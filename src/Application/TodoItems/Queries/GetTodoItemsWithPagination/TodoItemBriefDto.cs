<<<<<<< HEAD
﻿using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;
=======
﻿using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoItemBriefDto
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoItem, TodoItemBriefDto>();
        }
    }
}

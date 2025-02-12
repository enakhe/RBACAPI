<<<<<<< HEAD
﻿using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.TodoLists.Queries.GetTodos;
=======
﻿using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.TodoLists.Queries.GetTodos;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoItemDto
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }

    public int Priority { get; init; }

    public string? Note { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoItem, TodoItemDto>().ForMember(d => d.Priority,
                opt => opt.MapFrom(s => (int)s.Priority));
        }
    }
}

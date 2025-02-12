<<<<<<< HEAD
﻿using RBACAPI.Domain.Entities;

namespace RBACAPI.Application.TodoLists.Queries.GetTodos;
=======
﻿using EcommerceAPI.Domain.Entities;

namespace EcommerceAPI.Application.TodoLists.Queries.GetTodos;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoListDto
{
    public TodoListDto()
    {
        Items = Array.Empty<TodoItemDto>();
    }

    public int Id { get; init; }

    public string? Title { get; init; }

    public string? Colour { get; init; }

    public IReadOnlyCollection<TodoItemDto> Items { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoList, TodoListDto>();
        }
    }
}

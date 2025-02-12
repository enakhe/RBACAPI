<<<<<<< HEAD
﻿namespace RBACAPI.Domain.Entities;
=======
﻿namespace EcommerceAPI.Domain.Entities;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}

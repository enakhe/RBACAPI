<<<<<<< HEAD
﻿namespace RBACAPI.Domain.Events;
=======
﻿namespace EcommerceAPI.Domain.Events;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class TodoItemCompletedEvent : BaseEvent
{
    public TodoItemCompletedEvent(TodoItem item)
    {
        Item = item;
    }

    public TodoItem Item { get; }
}

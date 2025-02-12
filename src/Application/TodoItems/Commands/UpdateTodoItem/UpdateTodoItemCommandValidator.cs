<<<<<<< HEAD
﻿namespace RBACAPI.Application.TodoItems.Commands.UpdateTodoItem;
=======
﻿namespace EcommerceAPI.Application.TodoItems.Commands.UpdateTodoItem;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}

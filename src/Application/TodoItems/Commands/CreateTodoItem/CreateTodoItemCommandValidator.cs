<<<<<<< HEAD
﻿namespace RBACAPI.Application.TodoItems.Commands.CreateTodoItem;
=======
﻿namespace EcommerceAPI.Application.TodoItems.Commands.CreateTodoItem;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}

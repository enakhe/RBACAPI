<<<<<<< HEAD
﻿namespace RBACAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;
=======
﻿namespace EcommerceAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class GetTodoItemsWithPaginationQueryValidator : AbstractValidator<GetTodoItemsWithPaginationQuery>
{
    public GetTodoItemsWithPaginationQueryValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("ListId is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

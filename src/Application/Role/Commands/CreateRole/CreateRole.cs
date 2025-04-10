using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Role.Commands.CreateRole;

public record CreateRoleCommand : IRequest<IActionResult>
{
    [Required]
    public required string Name { get; set; }
}

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    private readonly IApplicationDbContext _context;
    public CreateRoleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .WithMessage("The name field is reqyured");
    }
}

public class CreateRoleCommandHandler(IRoleService roleService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateRoleCommand, IActionResult>
{
    private readonly IRoleService _roleService = roleService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<IActionResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var createRoleResponse = await _roleService.CreateRole(request.Name);
        if (!createRoleResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = createRoleResponse.Message,
                succeded = createRoleResponse.Succeeded,
                errors = createRoleResponse.Errors
            });
        }

        return new ObjectResult(new
        {
            message = createRoleResponse.Message,
            succeded = createRoleResponse.Succeeded,
            errors = createRoleResponse.Errors
        })
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
}

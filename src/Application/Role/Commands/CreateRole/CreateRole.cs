using System.ComponentModel.DataAnnotations;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Role.Commands.CreateRole;

public record CreateRoleCommand : IRequest<Result>
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

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result>
{
    private readonly IRoleService _roleRepository;

    public CreateRoleCommandHandler(IRoleService roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var createRoleResponse = await _roleRepository.CreateRole(request.Name);
        return !createRoleResponse.Succeeded
            ? Result.Failure(createRoleResponse.Errors)
            : Result.Success(new { data = createRoleResponse.Response });
    }
}

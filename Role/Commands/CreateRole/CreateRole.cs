using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Role.Commands.CreateRole;

public record CreateRoleCommand : IRequest<IActionResult>
{
}

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
    }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IActionResult>
{
    private readonly IApplicationDbContext _context;

    public CreateRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

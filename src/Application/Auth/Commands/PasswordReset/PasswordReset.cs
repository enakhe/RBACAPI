using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.User.Commands.PasswordReset;

public record PasswordResetCommand : IRequest<IActionResult>
{
}

public class PasswordResetCommandValidator : AbstractValidator<PasswordResetCommand>
{
    public PasswordResetCommandValidator()
    {
    }
}

public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, IActionResult>
{
    private readonly IApplicationDbContext _context;

    public PasswordResetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<IActionResult> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using EcommerceAPI.Application.Common.Interfaces;

namespace EcommerceAPI.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest<IActionResult>
{
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IActionResult>
{
    private readonly IApplicationDbContext _context;

    public ResetPasswordCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

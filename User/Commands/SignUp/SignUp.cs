using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.User.Commands.SignUp;

public record SignUpCommand : IRequest<IActionResult>
{
}

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
    }
}

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, IActionResult>
{
    private readonly IApplicationDbContext _context;

    public SignUpCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

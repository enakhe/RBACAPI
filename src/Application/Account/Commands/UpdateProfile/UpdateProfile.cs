using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Account.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest<ActionResult>
{
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
    }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ActionResult>
{
    private readonly IApplicationDbContext _context;

    public UpdateProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActionResult> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

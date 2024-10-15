using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.User.Commands.Login;

public record SignInCommand : IRequest<IActionResult>
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    private readonly IApplicationDbContext _context;

    public SignInCommandValidator(IApplicationDbContext context)
    {
        _context = context;
    }
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, IActionResult>
{
    private readonly IIdentityService _identityService;

    public SignInCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IActionResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new BadRequestObjectResult("Invalid login request");
        }

        var loginResult = await _identityService.SignInAsync(request.Email, request.Password, request.RememberMe);

        if (!loginResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid login attempt");
        }

        return new OkObjectResult(new
        {
            loginResult.UserId,
            loginResult.UserName,
            loginResult.Email,
            loginResult.PhoneNumber,
            loginResult.EmailConfirmed
            loginResult.Token,
        });
    }
}

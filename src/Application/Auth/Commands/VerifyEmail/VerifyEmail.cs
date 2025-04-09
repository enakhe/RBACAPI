using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.User.Commands.VerifyEmail;

public record VerifyEmailCommand : IRequest<IActionResult>
{
    [Required]
    public required string Code { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }
}

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    private readonly IApplicationDbContext _context;
    public VerifyEmailCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");

        RuleFor(x => x.Code)
            .NotNull()
            .NotEmpty()
            .WithMessage("The code field is required")
            .MaximumLength(5)
            .WithMessage("Invalid code provided");
    }
}

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VerifyEmailCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {

        var verifyEmailResponse = await _identityService.VerifyEmailAsync(request.Email, request.Code);
        if (!verifyEmailResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = "One or more validation failures have occurred",
                error = verifyEmailResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            verifyEmailResponse
        });
    }
}

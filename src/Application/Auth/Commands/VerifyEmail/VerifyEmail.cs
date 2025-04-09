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
    public VerifyEmailCommandValidator()
    {

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

public class VerifyEmailCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<VerifyEmailCommand, IActionResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<IActionResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {

        var verifyEmailResponse = await _identityService.VerifyEmailAsync(request.Email, request.Code);
        if (!verifyEmailResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = verifyEmailResponse.Message,
                succeded = verifyEmailResponse.Succeeded,
                errors = verifyEmailResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                verifyEmailResponse.Title,
                verifyEmailResponse.Message,
                verifyEmailResponse.Succeeded,
                verifyEmailResponse.Response
            }
        });
    }
}

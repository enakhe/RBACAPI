using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Account.Commands.ChangeEmail;

public record ChangeEmailCommand : IRequest<IActionResult>
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}

public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");
    }
}

public class ChangeEmailCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangeEmailCommand, IActionResult>
{
    private readonly IAccountService _accountService = accountService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<IActionResult> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();

        var changeEMmailResponse = await _accountService.ChangeEmail(userId, request.Email);

        if (!changeEMmailResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = changeEMmailResponse.Message,
                succeded = changeEMmailResponse.Succeeded,
                errors = changeEMmailResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                changeEMmailResponse.Title,
                changeEMmailResponse.Message,
                changeEMmailResponse.Succeeded,
                changeEMmailResponse.Response
            }
        });
    }
}

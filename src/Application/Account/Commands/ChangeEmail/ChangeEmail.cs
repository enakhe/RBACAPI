using System.ComponentModel.DataAnnotations;
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
            .NotNull()
            .NotEmpty();
    }
}

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangeEmailCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = _identityService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid request");
        var changeEMmailResponse = await _identityService.ChangeEmail(userId, request.Email);

        if (!changeEMmailResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = changeEMmailResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            response = changeEMmailResponse
        });
    }
}

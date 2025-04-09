using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Auth.Commands.GetPasswordResetToken;

public record GetPasswordResetTokenCommand : IRequest<IActionResult>
{
    [Required, EmailAddress]
    public required string Email { get; set; }
}

public class GetPasswordResetTokenCommandValidator : AbstractValidator<GetPasswordResetTokenCommand>
{
    public GetPasswordResetTokenCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");
    }
}

public class GetPasswordResetTokenCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetPasswordResetTokenCommand, IActionResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<IActionResult> Handle(GetPasswordResetTokenCommand request, CancellationToken cancellationToken)
    {
        var getResetPasswordResponse = await _identityService.GetPasswordResetTokenAsync(request.Email);
        if (!getResetPasswordResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = getResetPasswordResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = getResetPasswordResponse
        });
    }
}

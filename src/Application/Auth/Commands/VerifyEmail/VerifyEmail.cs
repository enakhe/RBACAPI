using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.User.Commands.VerifyEmail;

public record VerifyEmailCommand : IRequest<IActionResult>
{
    [Required]
    public required string code { get; set; }
}

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
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
        var httpContext = _httpContextAccessor.HttpContext;
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new UnauthorizedObjectResult(new
            {
                error = new[] { "Not authorized" }
            });
        }

        var verifyEmailResponse = await _identityService.VerifyEmailAsync(email, request.code);
        if (!verifyEmailResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = verifyEmailResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            verifyEmailResponse
        });
    }
}

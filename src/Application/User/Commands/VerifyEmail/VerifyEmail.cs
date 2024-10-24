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
            throw new UnauthorizedAccessException("User email not found.");

        var verifyEmailResponse = await _identityService.VerifyEmail(email, request.code);
        if (!verifyEmailResponse.IsValid)
            throw new Exception("Something unexpected happened, unable to verify email");

        return new OkObjectResult(new
        {
            verifyEmailResponse.UserId,
            verifyEmailResponse.IsValid,
            verifyEmailResponse.Message,
        });
    }
}

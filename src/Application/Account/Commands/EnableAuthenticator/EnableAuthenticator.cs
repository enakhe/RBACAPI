using System.Security.Claims;
using RBACAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RBACAPI.Application.Account.Commands.EnableAuthenticator;

public record EnableAuthenticatorCommand : IRequest<IActionResult>;

public class EnableAuthenticatorCommandValidator : AbstractValidator<EnableAuthenticatorCommand>
{
    public EnableAuthenticatorCommandValidator()
    {
    }
}

public class EnableAuthenticatorCommandHandler : IRequestHandler<EnableAuthenticatorCommand, IActionResult>
{
    private readonly IAccountService _accountService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EnableAuthenticatorCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(EnableAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("We couldn’t get your recovery codes. Please ensure you're authenticated");

        var enableAuthenticatorResponse = await _accountService.EnableAuthenticator(userId);
        if (!enableAuthenticatorResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = enableAuthenticatorResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            response = enableAuthenticatorResponse
        });
    }
}

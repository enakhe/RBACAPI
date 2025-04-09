using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Account.Commands.Disable2FAuthentication;

public record Disable2FAuthenticationCommand : IRequest<IActionResult>;

public class Disable2FAuthenticationCommandValidator : AbstractValidator<Disable2FAuthenticationCommand>
{
    public Disable2FAuthenticationCommandValidator()
    {
    }
}

public class Disable2FAuthenticationCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor, IAccountService identityService) : IRequestHandler<Disable2FAuthenticationCommand, IActionResult>
{
    private readonly IAccountService _accountService = accountService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAccountService _identityService = identityService;

    public async Task<IActionResult> Handle(Disable2FAuthenticationCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();

        var disable2FAResponse = await _accountService.Disable2FAuthentication(userId);
        if (!disable2FAResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = disable2FAResponse.Message,
                succeded = disable2FAResponse.Succeeded,
                errors = disable2FAResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                disable2FAResponse.Title,
                disable2FAResponse.Message,
                disable2FAResponse.Succeeded,
                disable2FAResponse.Response
            }
        });
    }
}

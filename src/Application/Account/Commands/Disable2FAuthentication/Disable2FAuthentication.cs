using RBACAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RBACAPI.Application.Account.Commands.Disable2FAuthentication;

public record Disable2FAuthenticationCommand : IRequest<IActionResult>;

public class Disable2FAuthenticationCommandValidator : AbstractValidator<Disable2FAuthenticationCommand>
{
    public Disable2FAuthenticationCommandValidator()
    {
    }
}

public class Disable2FAuthenticationCommandHandler : IRequestHandler<Disable2FAuthenticationCommand, IActionResult>
{
    private readonly IAccountService _accountService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public Disable2FAuthenticationCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor, IIdentityService identityService)
    {
        _accountService = accountService;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<IActionResult> Handle(Disable2FAuthenticationCommand request, CancellationToken cancellationToken)
    {
        string userId = _identityService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("We couldn’t get your recovery codes. Please ensure you're authenticated");

        var disable2FAResponse = await _accountService.Disable2FAuthentication(userId);
        if (!disable2FAResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = disable2FAResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            response = disable2FAResponse
        });
    }
}
